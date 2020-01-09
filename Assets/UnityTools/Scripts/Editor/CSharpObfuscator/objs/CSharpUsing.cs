using System;

namespace UnityTools{
	/// <summary>
	/// Using，如：“using UnityEngine;”或“using System.Collections;”。
	/// </summary>
	public class CSharpUsing:IAlignObject,IUsing{
		
		public bool isStatic;
		/// <summary>
		/// <para>可以是一个单词<see cref="Segment"/>或<see cref="DotPath"/></para>
		/// <para>"using"/"static"后"."分隔的各个单词(包含空白,但不包含using/static后的第一个空格)</para>
		/// <para>如："System.Text.RegularExpressions"</para>
		/// </summary>
		public IString words;
		
		public CSharpUsing(bool isStatic,IString words){
			this.isStatic=isStatic;
			this.words=words;
		}

		public override string ToString(){
			throw new Exception("Please call ToString(string fileString)");
		}
		
		public string ToString(string fileString){
			return $"isStatic:{isStatic.ToString()} words:{words.ToString(fileString)}";
		}
	}
}
