namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// 泛型名称如："BaseApp<App,Bpp>"
	/// </summary>
	public class NameGenericString:IString{
		
		/// <summary>
		/// <see cref="SegmentString"/>/<see cref="NamePathString"/>/<see cref="NameGenericString"/>
		/// </summary>
		public IString name;
		/// <summary>
		/// <para>尖括号里的一个或多个名称(","号分隔的各个名称)</para>
		/// <para>元素的类型可以为：<see cref="SegmentString"/>/<see cref="NamePathString"/>/<see cref="NameGenericString"/></para>
		/// </summary>
		public IString[] tNames;

		public NameGenericString(IString name,IString[] tNames){
			this.name=name;
			this.tNames=tNames;
		}

		public string ToString(string fileString){
			const char splitChar=',';
			string strTNames="";
			int len=tNames.Length;
			for(int i=0;i<len;i++){
				strTNames+=tNames[i].ToString(fileString);
				if(i<len-1)strTNames+=splitChar;
			}
			return $"{name.ToString(fileString)}<{strTNames}>";
		}

	}

}
