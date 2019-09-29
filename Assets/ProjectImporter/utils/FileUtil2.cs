﻿namespace UnityProjectImporter {
	using System.Collections.Generic;
	using System.IO;
	using System.Text;
	using UnityEditor;
	/// <summary>
	/// 文件工具类
	/// </summary>
	public class FileUtil2 {
		public static void copyFile(string source,string dest,bool isRefreshAsset=false,bool isExistReplace=true){
			if(isExistReplace&&File.Exists(dest)){
				FileUtil.ReplaceFile(source,dest);
			}else{
				FileUtil.CopyFileOrDirectory(source,dest);
			}
			if(isRefreshAsset)AssetDatabase.Refresh();
		}

		public static void copyDirectory(string source,string dest,bool isRefreshAsset=false){
			//如果文件夹存在会自动删除
			FileUtil.CopyFileOrDirectory(source,dest);
			if(isRefreshAsset)AssetDatabase.Refresh();
		}

		public static void createDirectory(string path,bool isExistDelete=true){
			if(isExistDelete&&Directory.Exists(path)){
				FileUtil.DeleteFileOrDirectory(path);
			}
			Directory.CreateDirectory(path);
		}

		/// <summary>
		/// 返回文件的所有行
		/// </summary>
		/// <param name="filePath">文件路径,如果是'\'路径,需要加@转换，如:getFileLines(@"E:\unity_tags\Assets\test.txt")</param>
		/// <returns></returns>
		public static List<string> getFileLines(string filePath){
			StreamReader streamReader=File.OpenText(filePath);

			List<string> fileLines=new List<string>();
			string line;
			while((line=streamReader.ReadLine())!=null){
				line+='\n';//行尾加回车
				fileLines.Add(line);
			}
			streamReader.Dispose();
			return fileLines;
		}

		/// <summary>
		/// 将行字符串数组写入到本地(UTF-8格式)
		/// </summary>
		/// <param name="fileLines">行字符数组</param>
		/// <param name="filePath">写入文件的路径,如果是'\'路径,需要加@转换，如:getFileLines(@"E:\unity_tags\Assets\test.txt")</param>
		public static void writeFileLines(string[] fileLines,string filePath){
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
