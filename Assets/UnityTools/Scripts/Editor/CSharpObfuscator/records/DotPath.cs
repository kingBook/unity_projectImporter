using System;

namespace UnityTools{
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

		public override string ToString(){
			throw new Exception("Please call ToString(string fileString)");
		}

		public string ToString(string fileString){
			const char splitChar='.';
			string text="";
			int len=words.Length;
			for(int i=0;i<len;i++){
				string word=words[i].ToString(fileString);
				//if(!includeWhitespace)word=Regex.Replace(word,@"\s","",RegexOptions.Compiled);
				text+=word;
				if(i<len-1)text+=splitChar;
			}
			return text;
		}

	}
}
