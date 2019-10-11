namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	///<summary>大括号块</summary>
	public class BracketBlock{
		public int startIndex;
		public int length;

		public string ToString(string fileString){
			return fileString.Substring(startIndex,length);
		}
	
	}
}