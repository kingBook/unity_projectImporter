namespace UnityProjectImporter{
	using UnityEngine;
	using System.Collections;
    using System.Collections.Generic;
	using System.IO;

	/// <summary>
	/// Guid工具类
	/// </summary>
	public static class GuidUtil{
		
		/// <summary>
		/// 返回指定文件夹下所有资源文件的guid列表
		/// </summary>
		/// <param name="folderPath">文件夹路径,如果是'\'路径,需要加@转换，如:getFolderGuidList(@"E:\unity_tags\Assets")</param>
		/// <returns></returns>
		public static string[] getFolderGuidList(string folderPath){
			
			List<string> list=new List<string>();
			DirectoryInfo directoryInfo=new DirectoryInfo(folderPath);
			FileInfo[] fileInfos=directoryInfo.GetFiles("*.meta",SearchOption.AllDirectories);
			int len=fileInfos.Length;
			for(int i=0;i<len;i++){
				FileInfo fileInfo=fileInfos[i];
				List<string> fileLines=FileUtil2.getFileLines(@fileInfo.FullName);
				//unity的.meta文件都是在第二行表示guid
				string guidLine=fileLines[1];
				Debug.Log(fileInfo.FullName);
				Debug.Log(guidLine);

			}
			return list.ToArray();
		}

		/// <summary>
		/// 返回一个唯一的新的Guid
		/// </summary>
		/// <param name="oldGuidList">返回的Guid将不与该列表中的任意项重复</param>
		/// <returns></returns>
		public static string getUniqueNewGuid(string oldGuidList){
			return null;
		}

		
	}
}
