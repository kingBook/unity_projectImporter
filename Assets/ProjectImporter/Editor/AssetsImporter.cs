namespace UnityProjectImporter{
	using UnityEngine;
	using System.Collections;
	using System.IO;
	using System.Collections.Generic;
	using System.Text;

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
		/// <param name="filePath">.cs文件路径(如果是'\'路径,需要加@转换，只接收转换后的路径)</param>
		/// <param name="projectName">导入的项目名称</param>
		private void editCSharpFile(string filePath,string projectName){
			Debug.Log(filePath);
			//var fs=File.OpenWrite(@filePath);
			var streamReader=File.OpenText(@filePath);

			List<string> lines=new List<string>();
			string line;
			while((line=streamReader.ReadLine())!=null){
				line=line+'\n';
				lines.Add(line);
			}
			streamReader.Dispose();

			//添加以项目命名的namespace到.cs文件
			addNameSpaceToCSharpFile(lines,projectName);
			//重新写入文件
			writeFileLines(lines.ToArray(),filePath);
		}

		/// <summary>
		/// 添加命名空间到.cs文件
		/// </summary>
		/// <param name="fileLines">.cs读取出来的行数组</param>
		/// <param name="namespaceStr">需要添加的命名空间字符串</param>
		private void addNameSpaceToCSharpFile(List<string> fileLines,string namespaceStr){
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
		/// 将行字符数组写入到本地
		/// </summary>
		/// <param name="fileLines">行字符数组</param>
		/// <param name="filePath">写入文件路径</param>
		private void writeFileLines(string[] fileLines,string filePath){
			File.Delete(filePath);
			var fileStream=File.Create(filePath);

			StringBuilder strBuilder=new StringBuilder();
			int len=fileLines.Length;
			for(int i=0;i<len;i++){
				strBuilder.Append(fileLines[i]);
			}
			byte[] bytes=new UTF8Encoding(true).GetBytes(strBuilder.ToString());
			fileStream.Write(bytes,0,bytes.Length);
			fileStream.Dispose();
		}
	}

}