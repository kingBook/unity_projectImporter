namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using UnityTools;
	using UnityEngine.UIElements;
	using System.Text.RegularExpressions;
	/// <summary>
	/// Using，如：“using UnityEngine;”或“using System.Collections;”。
	/// </summary>
	public struct UsingString:IUsing{
		
		public bool isStatic;
		/// <summary>
		/// "using"/"static"后"."分隔的各个单词(包含空白,但不包含using/static后的第一个空格)
		/// <br>如："using  System.Text.RegularExpressions;"</br>
		/// </summary>
		public SectionString[] strings;
		
		public UsingString(bool isStatic,SectionString[] strings){
			this.isStatic=isStatic;
			this.strings=strings;
		}
		
		/// <summary>
		/// 转换为字符串
		/// </summary>
		/// <param name="fileString">.cs文件字符串</param>
		/// <param name="inclusiveWhite">是否包含空白</param>
		/// <returns></returns>
		public string ToString(string fileString,bool inclusiveWhite){
			string text="";
			int len=strings.Length;
			for(int i=0;i<len;i++){
				string str=strings[i].ToString(fileString);
				if(!inclusiveWhite){
					str=Regex.Replace(str,@"\s","");
				}
				text+=str;
				if(i<len-1)text+=" ";
			}
			return string.Format("isStatic:{0} strings:{1}",isStatic.ToString(),text);
		}
	}
}
