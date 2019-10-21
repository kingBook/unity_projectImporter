namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using System.Text.RegularExpressions;
	/// <summary>
	/// 一段字符串
	/// </summary>
	public struct SectionString{
		private CSharpFile _cSharpFile;
		public int startIndex;
		public int length;
		
		/// <summary>
		/// 返回一个SectionString[]数组的字符串
		/// </summary>
		/// <param name="words">SectionString[]数组</param>
		/// <param name="splitChar">分隔符号</param>
		/// <param name="includeWhitespace">是否包含空白</param>
		/// <returns></returns>
		public static string getWordsString(SectionString[] words,string splitChar,bool includeWhitespace){
			string nameString="";
			int len=words.Length;
			for(int i=0;i<len;i++){
				string word=words[i].ToString();
				if(!includeWhitespace)word=Regex.Replace(word,@"\s","",RegexOptions.Compiled);
				nameString+=word;
				if(i<len-1)nameString+=splitChar;
			}
			return nameString;
		}
		
		public SectionString(CSharpFile cSharpFile,int startIndex,int length){
			_cSharpFile=cSharpFile;
			this.startIndex=startIndex;
			this.length=length;

			_cSharpFile.addSectionString(this);
		}

		public override string ToString(){
			return _cSharpFile.fileString.Substring(startIndex,length);
		}
		
	}
}