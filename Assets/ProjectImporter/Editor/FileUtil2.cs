namespace UnityProjectImporter{ 
	using System;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

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

		public static void createDirectory(string path,bool isExistReplace=true){
			if(isExistReplace&&Directory.Exists(path)){
				FileUtil.DeleteFileOrDirectory(path);
			}
			Directory.CreateDirectory(path);
		}
	
	}
}
