namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using System.Text.RegularExpressions;

	/// <summary>
	/// 单词+尖括号如："BaseApp&lt;App,Bpp&gt;"
	/// </summary>
	public struct WordAngleBrackets:IString{

		public int startIndex;
		public int length;
		
		public Segment word;

		public AngleBrackets angleBrackets;

		public WordAngleBrackets(int startIndex,int length,Segment word,AngleBrackets angleBrackets){
			this.startIndex=startIndex;
			this.length=length;
			this.word=word;
			this.angleBrackets=angleBrackets;
		}

		public override string ToString(){
			throw new System.Exception("Please call ToString(string fileString)");
		}

		public string ToString(string fileString){
			return $"{word.ToString(fileString)}{angleBrackets.ToString(fileString)}";
		}

	}

}
