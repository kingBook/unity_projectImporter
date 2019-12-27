namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using UnityTools;

	/// <summary>
	/// 泛型约束，如：“where T : class”、“where T : new()”、“where T : Base, new()”、“where T : <基类名>”
	/// </summary>
	public struct CSharpGenericConstraint:IString{
	
		/// <summary>
		/// 如：“where T : class”中的“T”。
		/// </summary>
		public Segment tName;

		/// <summary>
		/// "where T :"之后以“,”分隔的各个字符串
		/// </summary>
		public IString[] words;

		public CSharpGenericConstraint(Segment tName,IString[] words){
			this.tName=tName;
			this.words=words;
		}

		public string ToString(string fileString){
			string tNameText=tName.ToString(fileString);
		
			const char splitChar=',';
			string wordsText="";
			int len=words.Length;
			for(int i=0;i<len;i++){
				string word=words[i].ToString(fileString);
				wordsText+=word;
				if(i<len-1)wordsText+=splitChar;
			}
			return $"where {tNameText}:{wordsText}";
		}

	}
}