namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using UnityTools;
	using UnityEngine.UIElements;
	using System.Text.RegularExpressions;
	/// <summary>
	/// Using，如：“using UnityEngine;”或“using System.Collections;”。
	/// </summary>
	public struct CSharpUsing:IUsing{
		
		public bool isStatic;
		/// <summary>
		/// <para>"using"/"static"后"."分隔的各个单词(包含空白,但不包含using/static后的第一个空格)</para>
		/// <para>如："System.Text.RegularExpressions"</para>
		/// </summary>
		public NamePathString words;
		
		public CSharpUsing(bool isStatic,NamePathString words){
			this.isStatic=isStatic;
			this.words=words;
		}

		public string ToString(string fileString){
			return ToString(fileString,true);
		}
		
		/// <summary>
		/// 转换为字符串
		/// </summary>
		/// <param name="fileString">.cs文件字符串</param>
		/// <param name="includeWhitespace">是否包含空白</param>
		/// <returns></returns>
		public string ToString(string fileString,bool includeWhitespace){
			return $"isStatic:{isStatic.ToString()} nameWord:{words.ToString(fileString,includeWhitespace)}";
		}
	}
}
