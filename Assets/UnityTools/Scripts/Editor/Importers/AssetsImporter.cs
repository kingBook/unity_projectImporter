namespace UnityTools {
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Text.RegularExpressions;
	using UnityEditor.SceneManagement;
	using UnityEngine;

	public class AssetsImporter:Importer{
		
		/// <summary>
		/// 导入项目的Assets文件夹，并修改.cs文件解决冲突
		/// </summary>
		/// <param name="path">需要导入Assets文件夹的项目路径</param>
		/// <param name="projectImporterTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void import(string path,string projectImporterTempPath,string projectName){
			//当前项目的Assets文件夹的全路径,路径中使用"/"分隔,不是"\"。
			string assetsPath=Application.dataPath;
			//备份当前项目的所有GUID用于判断是否重复
			string[] oldGuidList=GuidUtil.getAllMetaFileGuidList(assetsPath);
			//创建子项目目录,如果目录存在则先删除
			string childProjectPath=assetsPath+"/"+projectName;
			FileUtil2.createDirectory(childProjectPath,true);
			//子项目Assets目录
			string childProjectAssetsPath=childProjectPath+"/Assets";
			//复制项目的Assets文件夹到子项目路径
			FileUtil2.copyDirectory(path+"/Assets",childProjectAssetsPath);
            //删除DOTweenSettings.asset
            deleteDOTweenSettingsAsset(childProjectAssetsPath);
			//删除不需要导入的文件夹，如Editor、Gizmos、Plugins等
			foreachAndDeleteIgnoreFolders(childProjectAssetsPath);
			//修改文件夹下的.cs文件解决冲突
			foreachAndEditCSharpFiles(childProjectAssetsPath,projectName);
			//修改文件夹下的.unity文件,修正SortingLayer等
			foreachAndEditUnityFiles(childProjectAssetsPath,projectName);
			//修改冲突的GUID
			foreachAndEditGuids(childProjectAssetsPath,oldGuidList);
		}

		/// <summary>
		/// 删除DOTween在Assets/Resources下生成的DOTweenSettings.asset
		/// </summary>
		/// <param name="childProjectAssetsPath"></param>
		private void deleteDOTweenSettingsAsset(string childProjectAssetsPath){
			string doTweenSettingsAssetPath=childProjectAssetsPath+"/Resources/DOTweenSettings.asset";
            string doTweenSettingsAssetMetaPath=childProjectAssetsPath+"/Resources/DOTweenSettings.asset.meta";
            if(File.Exists(doTweenSettingsAssetPath))File.Delete(doTweenSettingsAssetPath);
            if(File.Exists(doTweenSettingsAssetMetaPath))File.Delete(doTweenSettingsAssetMetaPath);
		}
        
        #region foreachAndDeleteIgnoreFolders
		/// <summary>
		/// 遍历删除不需要的子文件夹
		/// </summary>
		/// <param name="folderPath">遍历的文件夹目录</param>
		private void foreachAndDeleteIgnoreFolders(string folderPath){
			//只能在Assets文件夹下的一级子目录的特殊文件夹
			string[] ignoreRootFolderNames=new string[]{"EditorDefaultResources","Gizmos","Plugins","StandardAssets","StreamingAssets"};
			//可以在Assets文件夹下的任意子目录的特殊文件夹
			string[] ignoreFolderNames=new string[]{"Editor","DOTween"};
			//
			DirectoryInfo rootFolder=new DirectoryInfo(folderPath);
			foreacAndDeleteFolders(rootFolder,true,ignoreRootFolderNames,ignoreFolderNames);
		}
        /// <summary>
        /// 遍历删除不需要的子文件夹
        /// </summary>
        /// <param name="rootFolder">遍历的文件夹</param>
        /// <param name="isCheckRoot">如果true,将删除名称匹配deleteRootNames的文件夹</param>
        /// <param name="deleteRootNames">如果参数isCheckRoot为true,将删除名称匹配的文件夹</param>
        /// <param name="deleteNames">将在所有子级检测并删除名称匹配的文件夹</param>
		private void foreacAndDeleteFolders(DirectoryInfo rootFolder,bool isCheckRoot,string[] deleteRootNames,string[] deleteNames){
			DirectoryInfo[] directories=rootFolder.GetDirectories();
			int len=directories.Length;
			for(int i=0;i<len;i++){
				DirectoryInfo directory=directories[i];
                bool isDelete=Array.IndexOf(deleteNames,directory.Name)>-1;
                isDelete=isDelete|| (isCheckRoot&&Array.IndexOf(deleteRootNames,directory.Name)>-1);
                if(isDelete){
                    Directory.Delete(@directory.FullName,true);//删除
                    //对应的.meta文件也删除
                    string metaFilePath=@directory.FullName+".meta";
                    if(File.Exists(metaFilePath))File.Delete(metaFilePath);
                }else{
				    foreacAndDeleteFolders(directory,false,deleteRootNames,deleteNames);//递归,不检测root
                }
			}
		}
        #endregion

		#region foreachAndEditCSharpFiles
		/// <summary>
		/// 遍历和修改文件夹下的.cs文件
		/// </summary>
		/// <param name="folderPath">文件夹目录</param>
		/// <param name="projectName">导入的项目名称</param>
		private void foreachAndEditCSharpFiles(string folderPath,string projectName){
			var directoryInfo=new DirectoryInfo(folderPath);
			var files=directoryInfo.GetFiles("*.cs",SearchOption.AllDirectories);
			int len=files.Length;
			for(int i=0;i<len;i++){
				var file=files[i];
				//修改.cs文件
				editCSharpFile(@file.FullName,projectName);
			}
		}

		/// <summary>
		/// 修改.cs文件
		/// </summary>
		/// <param name="filePath">文件路径，如果是'\'路径,需要加@转换，如:editCSharpFile(@"E:\unity_tags\Assets\Main.cs")。</param>
		/// <param name="projectName">导入的项目名称</param>
		private void editCSharpFile(string filePath,string projectName){
			List<string> fileLines=FileUtil2.getFileLines(filePath,true);
			//修正不兼容的"SortingLayer"代码,使用"SortingLayer2"替换
			fixSortingLayerCode(fileLines);
			//修正不兼容的"LayerMask"代码,使用"LayerMask2"替换
			fixLayerMaskCode(fileLines);
			//修正不兼容的"SceneManager"代码,使用"SceneManager2"类替换
			fixSceneManagerCode(fileLines);
			//修正不兼容的"QualitySettings"代码,使用"QualitySettings2"类替换
			fixQualitySettingsCode(fileLines);
			//检测并添加以项目命名的namespace到.cs文件
			checkAndAddNameSpaceToCSharpFile(fileLines,projectName,filePath);
			//重新写入文件
			FileUtil2.writeFileLines(fileLines.ToArray(),filePath);
		}

		/// <summary>
		/// 修正"SortingLayer"代码，将使用"SortingLayer2"替换
		/// </summary>
		private void fixSortingLayerCode(List<string> fileLines){
			Regex[] matchRegexs=new Regex[]{
				//匹配"SortingLayer[] xxx=SortingLayer.layers"
				new Regex(@"SortingLayer\s*\[\s*\]\s*\S+\s*=\s*SortingLayer\s*.\s*layers",RegexOptions.Compiled),
				//匹配"var xxx=SortingLayer.layers"
				new Regex(@"var\s+\S+\s*=\s*SortingLayer\s*.\s*layers",RegexOptions.Compiled),
				//匹配"Array xxx=SortingLayer.layers"
				new Regex(@"Array\s+\S+\s*=\s*SortingLayer\s*.\s*layers",RegexOptions.Compiled),
				new Regex(@"SortingLayer\s*.\s*GetLayerValueFromID",RegexOptions.Compiled),
				new Regex(@"SortingLayer\s*.\s*GetLayerValueFromName",RegexOptions.Compiled),
				new Regex(@"SortingLayer\s*.\s*IDToName",RegexOptions.Compiled),
				new Regex(@"SortingLayer\s*.\s*IsValid",RegexOptions.Compiled),
				new Regex(@"SortingLayer\s*.\s*NameToID",RegexOptions.Compiled),
				//匹配"SortingLayer xxx=xxx"
				new Regex(@"SortingLayer\s+\S+\s*=\s*",RegexOptions.Compiled),
				//匹配"(SortingLayer)xxx"
				new Regex(@"\(\s*SortingLayer\s*\)\s*\S+",RegexOptions.Compiled)
			};
			replaceWithMatchRegexs(fileLines,matchRegexs,"SortingLayer","SortingLayer2");
		}

		/// <summary>
		/// 修正"LayerMask"代码，将使用"LayerMask2"替换
		/// </summary>
		private void fixLayerMaskCode(List<string> fileLines){
			Regex[] matchRegexs=new Regex[]{
				new Regex(@"LayerMask\s*.\s*GetMask",RegexOptions.Compiled),
				new Regex(@"LayerMask\s*.\s*LayerToName",RegexOptions.Compiled),
				new Regex(@"LayerMask\s*.\s*NameToLayer",RegexOptions.Compiled),
				//匹配"LayerMask xxx=xxx"
				//new Regex(@"LayerMask\s+\S+\s*=\s*\S+",RegexOptions.Compiled),//public LayerMask xxx;序列化时，改为LayerMask2会出错
                
				//匹配"(LayerMask)xxx"
				new Regex(@"\(\s*LayerMask\s*\)\s*\S+",RegexOptions.Compiled)
			};
			replaceWithMatchRegexs(fileLines,matchRegexs,"LayerMask","LayerMask2");
		}

		/// <summary>
		/// 修正"SceneManager"代码，将使用"SceneManager2"类替换
		/// </summary>
		/// <param name="fileLines">.cs文件读取出来的行数组</param>
		private void fixSceneManagerCode(List<string> fileLines){
			Regex[] matchRegexs=new Regex[]{
				new Regex(@"SceneManager\s*.\s*LoadSceneAsync",RegexOptions.Compiled),
				new Regex(@"SceneManager\s*.\s*LoadScene",RegexOptions.Compiled),
				new Regex(@"SceneManager\s*.\s*UnloadSceneAsync",RegexOptions.Compiled),
				new Regex(@"SceneManager\s*.\s*UnloadScene",RegexOptions.Compiled),
				new Regex(@"SceneManager\s*.\s*GetSceneByName",RegexOptions.Compiled),
				new Regex(@"SceneManager\s*.\s*GetSceneByPath",RegexOptions.Compiled)
			};
			replaceWithMatchRegexs(fileLines,matchRegexs,"SceneManager","SceneManager2");
		}
		
		private void fixQualitySettingsCode(List<string> fileLines){
			Regex[] matchRegexs=new Regex[]{
				new Regex(@"QualitySettings\s*.\s*DecreaseLevel",RegexOptions.Compiled),
				new Regex(@"QualitySettings\s*.\s*GetQualityLevel",RegexOptions.Compiled),
				new Regex(@"QualitySettings\s*.\s*IncreaseLevel",RegexOptions.Compiled),
				new Regex(@"QualitySettings\s*.\s*SetQualityLevel",RegexOptions.Compiled)
			};
			replaceWithMatchRegexs(fileLines,matchRegexs,"QualitySettings","QualitySettings2");
		}

		/// <summary>
		/// 遍历每一个行，在每一行中查找匹配正则表达式的字符串，
		/// <br>然后在匹配正则表达式的字符串中再查找并替换字符</br>
		/// </summary>
		/// <param name="fileLines">.cs文件读取出来的行数组</param>
		/// <param name="matchRegexs">需要匹配的正则表达式数组</param>
		/// <param name="oldStr">原来的字符串</param>
		/// <param name="newStr">替换的字符串</param>
		private void replaceWithMatchRegexs(List<string> fileLines,Regex[] matchRegexs,string oldStr,string newStr){
			var matchEvaluator=new MatchEvaluator((Match m)=>{
				string mStr=m.Value;
				mStr=mStr.Replace(oldStr,newStr);
				return mStr;
			});

			int matchRegexsLen=matchRegexs.Length;
			int len=fileLines.Count;
			for(int i=0;i<len;i++){
				string line=fileLines[i];
				for(int j=0;j<matchRegexsLen;j++){
					line=matchRegexs[j].Replace(line,matchEvaluator);
				}
				fileLines[i]=line;
			}
		}

		/// <summary>
		/// 检测并添加命名空间到.cs文件
		/// </summary>
		/// <param name="fileLines">.cs文件读取出来的行数组</param>
		/// <param name="namespaceStr">需要添加的命名空间字符串</param>
		/// <param name="filePath">.cs文件路径</param>
		private void checkAndAddNameSpaceToCSharpFile(List<string> fileLines,string namespaceStr,string filePath){
			if(getNameSpaceNull(fileLines.ToArray(),filePath)){ 
				addNameSpaceToCSharpFile(fileLines,namespaceStr,filePath);
			}
		}

		/// <summary>
		/// 添加命名空间到.cs文件
		/// </summary>
		/// <param name="fileLines">.cs文件读取出来的行数组</param>
		/// <param name="namespaceStr">需要添加的命名空间字符串</param>
		/// <param name="filePath">.cs文件路径</param>
		private void addNameSpaceToCSharpFile(List<string> fileLines,string namespaceStr,string filePath){
			int len=fileLines.Count;
			for(int i=0;i<len;i++){
				//每一行首加入Tab
				fileLines[i]=fileLines[i].Insert(0,"\t");
			}
			string namespaceStart="namespace "+namespaceStr+"{\n";
			string namespaceEnd="\n}";
			fileLines.Insert(0,namespaceStart);
			fileLines.Add(namespaceEnd);
		}

		/// <summary>
		/// 返回.cs文件命名空间是否为空
		/// </summary>
		/// <param name="fileLines">.cs文件读取出来的行数组</param>
		/// <param name="filePath">.cs文件路径</param>
		/// <returns></returns>
		private bool getNameSpaceNull(string[] fileLines,string filePath){
			Regex namespaceRegex=new Regex(@"namespace\s+\S+",RegexOptions.Compiled);
			Regex classRegex=new Regex(@"class\s+\S+",RegexOptions.Compiled);
			Regex structRegex=new Regex(@"struct\s+\S+",RegexOptions.Compiled);
			Regex enumRegex=new Regex(@"enum\s+\S+",RegexOptions.Compiled);
			//是否存在命名空间
			bool isNameSpaceNull=true;
			int len=fileLines.Length;
			for(int i=0;i<len;i++){
				string line=fileLines[i];
				if(namespaceRegex.IsMatch(line)){
					isNameSpaceNull=false;
					//将已存在的命名空间log出来
					string tempPath=filePath.Substring(filePath.IndexOf("Assets")+7);//去掉Asset之前的字符路径
					Debug.Log("已存在 "+namespaceRegex.Match(line)+"将不再添加命名空间，请确保此命名空间不会在项目中产生冲突。"+tempPath);
					break;
				}
				if(classRegex.IsMatch(line)||structRegex.IsMatch(line)||enumRegex.IsMatch(line)){
					//从开头找到class声明或struct声明或enum声明的位置则停止
					break;
				}
			}
			return isNameSpaceNull;
		}
		#endregion
		
		#region foreachAndEditUnityFiles
		/// <summary>
		/// 遍历和修改文件夹下的.unity文件
		/// 注意：请确保需要修改的.unity文件没有在编辑器中打开，否则会修改不成功
		/// </summary>
		/// <param name="folderPath">文件夹目录</param>
		/// <param name="projectName">导入的项目名称</param>
		private void foreachAndEditUnityFiles(string folderPath,string projectName){
			//Debug.Log(Directory.Exists(folderPath));
			var directoryInfo=new DirectoryInfo(folderPath);
			var files=directoryInfo.GetFiles("*.unity",SearchOption.AllDirectories);
			int len=files.Length;
			for(int i=0;i<len;i++){
				//Debug.Log( "FullName:" + files[i].FullName );  
				//Debug.Log( "DirectoryName:" + files[i].DirectoryName ); 
				var file=files[i];
				//修改.unity文件
				editUnityFile(@file.FullName,projectName);
			}
		}

		/// <summary>
		/// 修改.unity文件
		/// 注意：请确保需要修改的.unity文件没有在编辑器中打开，否则会修改不成功
		/// </summary>
		/// <param name="filePath">文件路径，如果是'\'路径,需要加@转换，如:editCSharpFile(@"E:\unity_tags\Assets\main.unity")。</param>
		/// <param name="projectName">导入的项目名称</param>
		private void editUnityFile(string filePath,string projectName){
			//是否在编辑器中打开.unity
			string tempFilePath=filePath.Replace('\\','/');
			int sceneCount=EditorSceneManager.sceneCount;
			for(int i=0;i<sceneCount;i++){
				var scene=EditorSceneManager.GetSceneAt(i);
				if(tempFilePath.IndexOf(scene.path)>-1){
					Debug.LogError("请关闭"+scene.path+"再重新导入");
					break;
				}
			}
			//
			List<string> fileLines=FileUtil2.getFileLines(filePath,true);
			//修正引用的SortingLayer.id
			fixSortingLayerID(fileLines);
			//重新写入文件
			FileUtil2.writeFileLines(fileLines.ToArray(),filePath);
		}

		/// <summary>
		/// 修正SortingLayer.id
		/// </summary>
		/// <param name="fileLines">.unity文件读取出来的文本行数组</param>
		private void fixSortingLayerID(List<string> fileLines){
			Regex SortingLayerIDRegex=new Regex(@"m_SortingLayerID:\s\d+",RegexOptions.Compiled);
			Regex sortingLayerValueRegex=new Regex(@"m_SortingLayer:\s\d+",RegexOptions.Compiled);
			int len=fileLines.Count;
			int maxIndex=len-1;
			for(int i=0;i<len;i++){
				string line=fileLines[i];
				//当前行匹配"m_SortingLayerID: xxx"(xxx是任意正整数)
				if(SortingLayerIDRegex.IsMatch(line)){
					if(i<maxIndex){
						//下一行匹配"m_SortingLayer: xxx"(xxx是任意正整数)
						string nextLine=fileLines[i+1];
						if(sortingLayerValueRegex.IsMatch(nextLine)){
							Regex numRegex=new Regex(@"\d+",RegexOptions.Compiled);
							//需要修改UniqueID的SortingLayer的id(在第几层)
							int orderIndex=int.Parse(numRegex.Match(nextLine).Value);
							//修改为当前编辑器SrtingLayer设置中的对应UniqueID
							uint defaultUniqueID=(uint)SortingLayer.layers[orderIndex].id;
							fileLines[i]=numRegex.Replace(line,defaultUniqueID.ToString());
						}
					}
				}
			}
		}
		#endregion

		#region foreachAndEditGuids
		/// <summary>
		/// 遍历和修改指定文件夹下各个文件有冲突的guid
		/// </summary>
		/// <param name="folderPath">要修改的文件夹</param>
		/// <param name="excludeGuidList">如果文件夹下各个文件的guid与该列表中项重复，则需要修改</param>
		private void foreachAndEditGuids(string folderPath,string[] excludeGuidList){
			//重复的guid列表
			string[] duplicateGuidList=getDuplicateGuidList(folderPath,excludeGuidList);
			//用于替换重复的guid列表
			string[] replaceGuidList=GuidUtil.getUniqueNewGuids(duplicateGuidList);
			//查找并替换guid的文件类型列表
			string[] testExtensions=new string[]{".meta",".unity",".asset",".prefab",".mat"};
			//
			DirectoryInfo directoryInfo=new DirectoryInfo(folderPath);
			FileInfo[] fileInfos=directoryInfo.GetFiles("*",SearchOption.AllDirectories);
			int len=fileInfos.Length;
			for(int i=0;i<len;i++){
				FileInfo fileInfo=fileInfos[i];
				string extension=fileInfo.Extension;
				string filePath=fileInfo.FullName;
				bool isTestFileType=Array.IndexOf(testExtensions,extension)>-1;
				if(isTestFileType){
					replaceFileDuplicateGuid(filePath,duplicateGuidList,replaceGuidList);
				}
			}
		}

		/// <summary>
		/// 替换文件冲突的guid
		/// </summary>
		/// <param name="filePath">文件路径</param>
		/// <param name="duplicateGuidList">重复有冲突的guid列表</param>
		/// <param name="replaceGuidList">要替换的guid列表，各个元素索引与duplicateGuidList一致</param>
		private void replaceFileDuplicateGuid(string filePath,string[] duplicateGuidList,string[] replaceGuidList){
			Regex regex=new Regex(@"guid:\s*");
			List<string> fileLines=FileUtil2.getFileLines(filePath,true,-1);
			int len=fileLines.Count;
			for(int i=0;i<len;i++){
				string line=fileLines[i];
				Match match=regex.Match(line);
				if(match.Success){
					int guidStartIndex=match.Index+match.Value.Length;
					string guidString=line.Substring(guidStartIndex,32);
					int atDuplicateListIndex=Array.IndexOf(duplicateGuidList,guidString);
					if(atDuplicateListIndex>-1){
						line=line.Remove(guidStartIndex,32);
						line=line.Insert(guidStartIndex,replaceGuidList[atDuplicateListIndex]);
						fileLines[i]=line;
					}
				}
			}
			FileUtil2.writeFileLines(fileLines.ToArray(),filePath);
		}

		/// <summary>
		/// 返回指定文件夹所有与excludeGuidList中项重复的Guid
		/// </summary>
		/// <param name="folderPath">查找的文件夹路径</param>
		/// <param name="excludeGuidList">用于判断重复的guid列表</param>
		/// <returns></returns>
		private string[] getDuplicateGuidList(string folderPath,string[] excludeGuidList){
			List<string> results=new List<string>();
			string[] folderAllMetaGuids=GuidUtil.getAllMetaFileGuidList(folderPath);
			int i=folderAllMetaGuids.Length;
			while(--i>=0){
				string guidString=folderAllMetaGuids[i];
				bool isDuplicate=Array.IndexOf(excludeGuidList,guidString)>-1;
				if(isDuplicate){
					results.Add(guidString);
				}
			}
			return results.ToArray();
		}
		#endregion
	}

}