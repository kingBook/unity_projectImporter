namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using UnityTools;
	using UnityEngine.UIElements;

	/// <summary>
	/// Using，如：“using UnityEngine;”或“using System.Collections;”。
	/// </summary>
	public struct UsingString:IUsing{
		
		public bool isStatic;
		/// <summary>
		/// 命名空间用"."分隔的每一个单词(包含多余的空白,但不包含using/static后的第一个空格)
		/// <br>如："using  System.Text.RegularExpressions;"</br>
		/// </summary>
		public SectionString[] strings;
		
		public UsingString(bool isStatic,SectionString[] strings){
			this.isStatic=isStatic;
			this.strings=strings;
		}
		
		public string ToString(string fileString){
			string text="";
			int len=strings.Length;
			for(int i=0;i<len;i++){
				text+=strings[i].ToString(fileString);
				if(i<len-1)text+=" ";
			}
			return string.Format("isStatic:{0} text:{1}",isStatic.ToString(),text);
		}
	}
}
