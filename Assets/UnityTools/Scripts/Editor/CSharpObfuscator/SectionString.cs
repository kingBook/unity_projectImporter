namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// 一段字符串
	/// </summary>
	public struct SectionString{
		public int startIndex;
		public int length;

		public SectionString(int startIndex,int length){
			this.startIndex=startIndex;
			this.length=length;
		}

		public string ToString(string fileString){
			return fileString.Substring(startIndex,length);
		}
	}
}