using System;
using Random=UnityEngine.Random;

namespace UnityTools{
	/// <summary>
	/// 名称工具类
	/// </summary>
	public static class NameUtil{
		private static readonly string m_upperCases="ABCDEFGHIJKLMNOPQRSTUVWXYZ";
		private static readonly string m_lowerCases="abcdefghijklmnopqrstuvwxyz";
		
		/// <summary>
		/// 返回一个随机的大写字母
		/// </summary>
		/// <returns></returns>
		public static char GetRandomUpperCase(){
			int index=Random.Range(0,26);
			return m_upperCases[index];
		}
		
		/// <summary>
		/// 返回一个随机的小写字母
		/// </summary>
		/// <returns></returns>
		public static char GetRandomLowerCase(){
			int index=Random.Range(0,26);
			return m_lowerCases[index];
		}
		
		/// <summary>
		/// 返回一个随机大写字母开头的名称，格式：大写字母+32位GUID
		/// </summary>
		/// <returns></returns>
		public static string GetStartUpperName(){
			char letter=GetRandomUpperCase();
			string guid=GuidUtil.GetNewGuid("N");
			return letter+guid;
		}
		
		/// <summary>
		/// 返回一个随机小写字母开头的名称，格式：小写字母+32位GUID
		/// </summary>
		/// <returns></returns>
		public static string GetStartLowerName(){
			char letter=GetRandomLowerCase();
			string guid=GuidUtil.GetNewGuid("N");
			return letter+guid;
		}
		
		/// <summary>
		/// 返回一个唯一的小写字母开头的名称
		/// </summary>
		/// <param name="excludeList">返回的名称不能与此列表中的任意项重复</param>
		/// <returns></returns>
		public static string GetUniqueStartLowerName(string[] excludeList){
			string name=GetStartLowerName();
			while(true){
				bool isHas=Array.IndexOf(excludeList,name)>-1;
				if(isHas){
					name=GetStartLowerName();
				}else{
					break;
				}
			}
			return name;
		}
		
		/// <summary>
		/// 返回一个唯一的大写字母开头的名称
		/// </summary>
		/// <param name="excludeList">返回的名称不能与此列表中的任意项重复</param>
		/// <returns></returns>
		public static string GetUniqueStartUpperName(string[] excludeList){
			string name=GetStartUpperName();
			while(true){
				bool isHas=Array.IndexOf(excludeList,name)>-1;
				if(isHas){
					name=GetStartUpperName();
				}else{
					break;
				}
			}
			return name;
		}
	}
}