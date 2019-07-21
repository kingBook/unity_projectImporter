namespace UnityProjectImporter{
	using UnityEngine;
	using System.Collections;
	using System.IO;
	using System.Collections.Generic;
	using System.Text;
	using System.Text.RegularExpressions;
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
			//导入项目的Assets文件夹到子项目路径
			FileUtil2.copyDirectory(path+"/Assets",childProjectPath+"/Assets");
			//修改文件夹下的.cs解决冲突
			foreachAndEditFolderCSharpFiles(childProjectPath+"/Assets",projectName);
		}

		/// <summary>
		/// 遍历和修改文件夹下的所有.cs文件
		/// </summary>
		/// <param name="folderPath">包含.cs文件的文件夹目录</param>
		/// <param name="projectName">导入的项目名称</param>
		private void foreachAndEditFolderCSharpFiles(string folderPath,string projectName){
			//Debug.Log(Directory.Exists(folderPath));
			var directoryInfo=new DirectoryInfo(folderPath);
			var files=directoryInfo.GetFiles("*.cs",SearchOption.AllDirectories);
			for(int i=0;i<files.Length;i++){
				//Debug.Log( "FullName:" + files[i].FullName );  
				//Debug.Log( "DirectoryName:" + files[i].DirectoryName ); 
				editCSharpFile(@files[i].FullName,projectName);
			}
		}

		/// <summary>
		/// 修改.cs文件
		/// </summary>
		/// <param name="filePath">.cs文件路径，如果是'\'路径,需要加@转换，如:editCSharpFile(@"E:\unity_tags\Assets\Main.cs")。</param>
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

			//修正不兼容的"SceneManager"代码,使用"SceneManager2"类替换
			fixSceneManagerCode(fileLines);
			//检测并添加以项目命名的namespace到.cs文件
			checkAndAddNameSpaceToCSharpFile(fileLines,projectName,filePath);
			//重新写入文件
			writeFileLines(fileLines.ToArray(),filePath);
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
			const string sceneManagerStr="SceneManager";
			int matchRegexsLen=matchRegexs.Length;
			int len=fileLines.Count;
			for(int i=0;i<len;i++){
				string line=fileLines[i];
				//行是否匹配想要替换的字符
				bool isMatch=false;
				for(int j=0;j<matchRegexsLen;j++){
					isMatch=matchRegexs[j].IsMatch(line);
					if(isMatch)break;
				}
				//匹配则将"SceneManager"替换为"SceneManager2"
				if(isMatch){
					int id=line.IndexOf(sceneManagerStr)+sceneManagerStr.Length;
					fileLines[i]=line.Insert(id,"2");
				}
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
				if(classRegex.IsMatch(line)){
					//从开头找到class声明的位置则停止
					break;
				}
			}
			return isNameSpaceNull;
		}

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