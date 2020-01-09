using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEditor;

namespace UnityTools {
	/// <summary>
	/// 文件工具类
	/// </summary>
	public class FileUtil2 {
		public static void CopyFile(string source,string dest,bool isRefreshAsset=false,bool isExistReplace=true){
			if(isExistReplace&&File.Exists(dest)){
				FileUtil.ReplaceFile(source,dest);
			}else{
				FileUtil.CopyFileOrDirectory(source,dest);
			}
			if(isRefreshAsset){
				AssetDatabase.Refresh();
			}
		}

		public static void CopyDirectory(string source,string dest,bool isRefreshAsset=false){
			//如果文件夹存在会自动删除
			FileUtil.CopyFileOrDirectory(source,dest);
			if(isRefreshAsset){
				AssetDatabase.Refresh();
			}
		}

		public static void CreateDirectory(string path,bool isExistDelete=true){
			if(isExistDelete&&Directory.Exists(path)){
				FileUtil.DeleteFileOrDirectory(path);
			}
			Directory.CreateDirectory(path);
		}

		/// <summary>
		/// 复制一个目录替换到指定的目录，如果指定目录不存在则新建
		/// </summary>
		/// <param name="source">源目录路径，尾部不包含目录分隔符</param>
		/// <param name="dest">目标目录路径，尾部不包含目录分隔符</param>
		/// <param name="progressVisible">是否显示进度条</param>
		/// <param name="filters">跳过复制操作的子文件或子文件夹，如"/Library","/test.txt"，将从源目录路径的尾部开始匹配，如果匹配成功则跳过复制</param>
		public static void ReplaceDirectory(string source,string dest,bool progressVisible,params string[] filters){
			source=source.Replace("\\","/");
			dest=dest.Replace("\\","/");

			int filtersLen=filters.Length;
			int sourceLen=source.Length;

			if(progressVisible)EditorUtility.DisplayProgressBar("Copying files","Readying...",0.0f);
			string[] files=Directory.GetFiles(source,"*",SearchOption.AllDirectories);
			int len=files.Length;
			for(int i=0;i<len;i++){
				string filePath=files[i];
				filePath=filePath.Replace("\\","/");
				//跳过不复制的文件或文件夹
				bool isContinue=false;
				for(int j=0;j<filtersLen;j++){
					bool isMatch=filePath.IndexOf(filters[j],sourceLen)>-1;
					if(isMatch){
						isContinue=true;
						break;
					}
				}
				if(isContinue)continue;
				//
				FileInfo fileInfo=new FileInfo(filePath);
				//创建放置的文件夹
				string directoryPath=fileInfo.Directory.FullName;
				directoryPath=directoryPath.Replace("\\","/");
				directoryPath=directoryPath.Replace(source,dest);
				Directory.CreateDirectory(directoryPath);
				//复制文件
				string destFilePath=filePath.Replace(source,dest);
				try{
					File.Copy(filePath,destFilePath,true);
				}catch(Exception err){
					//尝试复制文件时发生错误，关闭进度条抛出错误
					if(progressVisible)EditorUtility.ClearProgressBar();
					throw err;
				}
				//显示进度
				if(progressVisible)EditorUtility.DisplayProgressBar("Copying files","Copying "+fileInfo.FullName,(float)(i+1)/len);
			}
			if(progressVisible)EditorUtility.ClearProgressBar();
		}

		/// <summary>
		/// 返回文件的所有行
		/// </summary>
		/// <param name="filePath">文件路径,如果是'\'路径,需要加@转换，如:getFileLines(@"E:\unity_tags\Assets\test.txt")</param>
		/// <param name="isAddLineEndEnter">行尾是否添加回车</param>
		/// <param name="readCount">读取的行数，-1或<0:读取所有行</param>
		/// <returns></returns>
		public static List<string> GetFileLines(string filePath,bool isAddLineEndEnter,int readCount=-1){
			StreamReader streamReader=File.OpenText(filePath);

			List<string> fileLines=new List<string>();
			string line;
			int count=0;
			if(readCount!=0){
				while((line=streamReader.ReadLine())!=null){
					if(isAddLineEndEnter){
						line+='\n';//行尾加回车
					}
					fileLines.Add(line);

					if(readCount>0){
						count++;
						if(count>=readCount)break;
					}
				}
			}
			streamReader.Dispose();
			return fileLines;
		}

		/// <summary>
		/// 读取并返回文件的所有字符
		/// </summary>
		/// <param name="filePath">读取的文件路径</param>
		/// <returns></returns>
		public static string GetFileString(string filePath){
			StreamReader streamReader=File.OpenText(filePath);
			string fileString=streamReader.ReadToEnd();
			streamReader.Dispose();
			return fileString;
		}

		/// <summary>
		/// 将行字符串数组写入到本地(UTF-8格式)
		/// </summary>
		/// <param name="fileLines">行字符数组</param>
		/// <param name="filePath">写入文件的路径,如果是'\'路径,需要加@转换，如:getFileLines(@"E:\unity_tags\Assets\test.txt")</param>
		public static void WriteFileLines(string[] fileLines,string filePath){
			StringBuilder strBuilder=new StringBuilder();
			int len=fileLines.Length;
			for(int i=0;i<len;i++){
				strBuilder.Append(fileLines[i]);
			}
			WriteFileString(strBuilder.ToString(),filePath);
		}

		/// <summary>
		/// 将行字符串写入到本地(UTF-8格式)
		/// </summary>
		/// <param name="text">需要写入本地的字符串</param>
		/// <param name="filePath">写入文件的路径,如果是'\'路径,需要加@转换，如:getFileLines(@"E:\unity_tags\Assets\test.txt")</param>
		public static void WriteFileString(string text, string filePath){
			File.Delete(filePath);
			FileStream fileStream=File.Create(filePath);

			UTF8Encoding utf8Bom=new UTF8Encoding(true);
			byte[] bytes=utf8Bom.GetBytes(text);
			fileStream.Write(bytes,0,bytes.Length);
			fileStream.Dispose();

		}

		/// <summary>
		/// 打开选择文件夹对话框选择一个unity项目文件夹。
		/// <br>取消或选择非unity项目文件夹时都返回null</br>
		/// <br>选择非unity项目文件夹时，将弹出选择错误对话框</br>
		/// </summary>
		/// <returns>返回选择的unity项目文件夹路径</returns>
		public static string OpenSelectUnityProjectFolderPanel(){
			string folderPath=EditorUtility.OpenFolderPanel("Select a unity project","","");
			if(!string.IsNullOrEmpty(folderPath)){
				if(IsUnityProjectFolder(folderPath)){
					return folderPath;
				}else{
					EditorUtility.DisplayDialog("Selection error","Invalid project path:\n"+folderPath,"OK");
				}
			}
			return null;
		}

		/// <summary>
		/// 判断是不是unity项目文件夹(是否有"Assets"和"ProjectSettings"文件夹)
		/// </summary>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		public static bool IsUnityProjectFolder(string folderPath){
			bool hasAssetsFolder=false;
			bool hasProjectSettingsFolder=false;
			string[] subFolders=Directory.GetDirectories(folderPath);
			int len=subFolders.Length;
			for(int i=0;i<len;i++){
				string subFolderPath=subFolders[i];
				int parentFolderIndex=subFolderPath.IndexOf(folderPath);
				subFolderPath=subFolderPath.Substring(parentFolderIndex+1);
				if(subFolderPath.IndexOf("Assets")>-1)hasAssetsFolder=true;
				if(subFolderPath.IndexOf("ProjectSettings")>-1)hasProjectSettingsFolder=true;
				if(hasAssetsFolder&&hasProjectSettingsFolder){
					break;
				}
			}
			return hasAssetsFolder&&hasProjectSettingsFolder;
		}

		/// <summary>
		/// 使用Windows的Explorer打开一个文件夹目录
		/// </summary>
		public static void ShowInExplorer(string folderPath){
			folderPath=folderPath.Replace("/","\\");
			Process.Start("explorer.exe",folderPath);
		}

	}
}
