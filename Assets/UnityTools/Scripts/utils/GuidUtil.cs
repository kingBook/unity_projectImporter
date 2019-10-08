﻿namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using System.Collections.Generic;
	using System.IO;
	using System.Text.RegularExpressions;
	using System;

	/// <summary>
	/// Guid工具类
	/// </summary>
	public static class GuidUtil{
		
		/// <summary>
		/// 返回指定文件夹下所有.meta文件的guid列表
		/// </summary>
		/// <param name="folderPath">文件夹路径,如果是'\'路径,需要加@转换，如:getFolderGuidList(@"E:\unity_tags\Assets")</param>
		/// <returns></returns>
		public static string[] getAllMetaFileGuidList(string folderPath){
			DirectoryInfo directoryInfo=new DirectoryInfo(folderPath);
			FileInfo[] fileInfos=directoryInfo.GetFiles("*.meta",SearchOption.AllDirectories);
			Regex regex=new Regex(@"guid:\s*",RegexOptions.Compiled);
			int len=fileInfos.Length;
			string[] list=new string[len];
			for(int i=0;i<len;i++){
				FileInfo fileInfo=fileInfos[i];
				List<string> fileLines=FileUtil2.getFileLines(@fileInfo.FullName,false,2);
				//meta文件都是在第二行表示guid
				string guidLine=fileLines[1];
				
				Match match=regex.Match(guidLine);
				list[i]=guidLine.Substring(match.Value.Length,32);
			}
			return list;
		}

		/// <summary>
		/// 返回唯一的新的Guid列表，长度与excludeGuidList一致
		/// </summary>
		/// <param name="excludeGuidList">返回的Guid将不与该列表中的任意项重复</param>
		/// <returns></returns>
		public static string[] getUniqueNewGuids(string[] excludeGuidList){
			int len=excludeGuidList.Length;
			string[] list=new string[len];
			for(int i=0;i<len;i++){
				list[i]=getUniqueNewGuid(excludeGuidList);
			}
			return list;
		}

		/// <summary>
		/// 返回一个唯一的新的Guid
		/// </summary>
		/// <param name="excludeGuidList">返回的Guid将不与该列表中的任意项重复</param>
		/// <returns></returns>
		public static string getUniqueNewGuid(string[] excludeGuidList){
			string guid=getNewGuid();
			while(true){
				bool isHas=Array.IndexOf(excludeGuidList,guid)>-1;
				if(isHas){
					guid=getNewGuid();
				}else{
					break;
				}
			}
			return guid;
		}

		/// <summary>
		/// 返回一个新的Guid
		/// </summary>
		/// <param name="format">格式详情:https://www.cnblogs.com/kingBook/p/11608443.html</param>
		/// <returns></returns>
		public static string getNewGuid(string format="N"){
			return Guid.NewGuid().ToString(format);
		}

		
	}
}