namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using System.Text.RegularExpressions;
	/// <summary>
	/// 记录一段字符串在.cs文件字符中的起始索引和长度的结构体
	/// </summary>
	public struct Segment:IString{
		
		public int startIndex;
		public int length;
		
		/// <summary>
		/// 创建一个SegmentString结构体。
		/// </summary>
		/// <param name="startIndex">起始索引</param>
		/// <param name="length">长度</param>
		public Segment(int startIndex,int length){
			this.startIndex=startIndex;
			this.length=length;
		}

		public string ToString(string fileString){
			return fileString.Substring(startIndex,length);
		}
		
	}
}