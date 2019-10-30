namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using System.Text.RegularExpressions;
	/// <summary>
	/// 记录一段字符串在.cs文件字符中的起始索引和长度的结构体
	/// </summary>
	public struct SectionString{
		private readonly string _fileString;
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
		
		/// <summary>
		/// 创建一个SectionString结构体，并添加到cSharpFile.sectionStrings列表中。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="startIndex">起始索引</param>
		/// <param name="length">长度</param>
		public SectionString(CSharpFile cSharpFile,int startIndex,int length){
			_fileString=cSharpFile.fileString;
			this.startIndex=startIndex;
			this.length=length;
			
			cSharpFile.addSectionString(this);
		}
		
		/// <summary>
		/// 创建一个SectionString结构体。
		/// </summary>
		/// <param name="fileString">.cs文件字符串</param>
		/// <param name="startIndex">起始索引</param>
		/// <param name="length">长度</param>
		/*public SectionString(string fileString,int startIndex,int length){
			_fileString=fileString;
			this.startIndex=startIndex;
			this.length=length;
		}*/

		public override string ToString(){
			return _fileString.Substring(startIndex,length);
		}
		
	}
}