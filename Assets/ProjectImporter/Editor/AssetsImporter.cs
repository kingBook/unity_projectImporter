namespace UnityProjectImporter {
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
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
			//创建子项目目录,如果目录存在则先删除
			string childProjectPath=Application.dataPath+"/"+projectName;
			FileUtil2.createDirectory(childProjectPath,true);
			//子项目Assets目录
			string childProjectAssetsPath=childProjectPath+"/Assets";
			//导入项目的Assets文件夹到子项目路径
			FileUtil2.copyDirectory(path+"/Assets",childProjectAssetsPath);
			//修改文件夹下的.cs文件解决冲突
			foreachAndEditCSharpFiles(childProjectAssetsPath,projectName);
			//修改文件夹下的.unity文件,修正SortingLayer等
			foreachAndEditUnityFiles(childProjectAssetsPath,projectName);
		}

		#region foreachAndEditCSharpFiles
		/// <summary>
		/// 遍历和修改文件夹下的.cs文件
		/// </summary>
		/// <param name="folderPath">文件夹目录</param>
		/// <param name="projectName">导入的项目名称</param>
		private void foreachAndEditCSharpFiles(string folderPath,string projectName){
			//Debug.Log(Directory.Exists(folderPath));
			var directoryInfo=new DirectoryInfo(folderPath);
			var files=directoryInfo.GetFiles("*.cs",SearchOption.AllDirectories);
			int len=files.Length;
			for(int i=0;i<len;i++){
				//Debug.Log( "FullName:" + files[i].FullName );  
				//Debug.Log( "DirectoryName:" + files[i].DirectoryName ); 
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
			//Debug.Log(filePath);
			var streamReader=File.OpenText(@filePath);

			List<string> fileLines=new List<string>();
			string line;
			while((line=streamReader.ReadLine())!=null){
				line+='\n';//行尾加回车
				fileLines.Add(line);
			}
			streamReader.Dispose();
			//修正不兼容的"SortingLayer"代码,使用"SortingLayer2"替换
			fixSortingLayerCode(fileLines);
			//修正不兼容的"SceneManager"代码,使用"SceneManager2"类替换
			fixSceneManagerCode(fileLines);
			//检测并添加以项目命名的namespace到.cs文件
			checkAndAddNameSpaceToCSharpFile(fileLines,projectName,filePath);
			//重新写入文件
			writeFileLines(fileLines.ToArray(),filePath);
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
				new Regex(@"SortingLayer\s*.\s*GetLayerValueFromID",RegexOptions.Compiled),
				new Regex(@"SortingLayer\s*.\s*GetLayerValueFromName",RegexOptions.Compiled),
				new Regex(@"SortingLayer\s*.\s*IDToName",RegexOptions.Compiled),
				new Regex(@"SortingLayer\s*.\s*IsValid",RegexOptions.Compiled),
				new Regex(@"SortingLayer\s*.\s*NameToID",RegexOptions.Compiled),
				//匹配"SortingLayer xxx=xxx"
				new Regex(@"SortingLayer\s+\S+\s*=\s*\S+",RegexOptions.Compiled)
			};
			replaceWithMatchRegexs(fileLines,matchRegexs,"SortingLayer","SortingLayer2");
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
			var streamReader=File.OpenText(@filePath);
			List<string> fileLines=new List<string>();
			string line;
			while((line=streamReader.ReadLine())!=null){
				line+='\n';//行尾加回车
				fileLines.Add(line);
			}
			streamReader.Dispose();
			//修正引用的SortingLayer.id
			fixSortingLayerID(fileLines);
			//重新写入文件
			writeFileLines(fileLines.ToArray(),filePath);
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

		/// <summary>
		/// 将行字符数组写入到本地
		/// </summary>
		/// <param name="fileLines">行字符数组</param>
		/// <param name="filePath">写入文件的路径</param>
		private void writeFileLines(string[] fileLines,string filePath){
			File.Delete(filePath);
			var fileStream=File.Create(filePath);

			StringBuilder strBuilder=new StringBuilder();
			int len=fileLines.Length;
			for(int i=0;i<len;i++){
				strBuilder.Append(fileLines[i]);
			}
			UTF8Encoding utf8Bom=new UTF8Encoding(true);
			byte[] bytes=utf8Bom.GetBytes(strBuilder.ToString());
			fileStream.Write(bytes,0,bytes.Length);
			fileStream.Dispose();
		}
	}

}