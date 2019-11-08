namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    /// 表示带"."的全路径名称,如:"System.Collections.Generic.List"
    /// </summary>
    public struct DotPath:IString{

		/// <summary>
		/// 带"."的全路径名称长度>1
		/// </summary>
		public Segment[] words;

		public DotPath(Segment[] words){
			this.words=words;
		}

		public string ToString(string fileString){
			return ToString(fileString,true);
		}

		public string ToString(string fileString,bool includeWhitespace){
			const char splitChar='.';
			string nameString="";
			int len=words.Length;
			for(int i=0;i<len;i++){
				string word=words[i].ToString(fileString);
				if(!includeWhitespace)word=Regex.Replace(word,@"\s","",RegexOptions.Compiled);
				nameString+=word;
				if(i<len-1)nameString+=splitChar;
			}
			return nameString;
		}

	}
}
