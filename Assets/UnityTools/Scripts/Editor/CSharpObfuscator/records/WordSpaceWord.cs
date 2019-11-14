namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// 单词+空白+单词
	/// </summary>
	public struct WordSpaceWord:IString{
		
		/// <summary>
		/// 两个单词(长度永远为2)
		/// </summary>
		public Segment[] words;

		public WordSpaceWord(Segment word1,Segment word2){
			words=new Segment[2];
			words[0]=word1;
			words[1]=word2;
		}
		
		public string ToString(string fileString) {
			return $"{words[0]} {words[1]}";
		}
	}
}
