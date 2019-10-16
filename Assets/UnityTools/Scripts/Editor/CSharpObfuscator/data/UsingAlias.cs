﻿namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Using别名，如:"using Project = PC.MyCompany.Project;"
    /// </summary>
    public struct UsingAlias:IUsing{
		/// <summary>
		/// 别名名称(包含空白,但不包含using后的第一个空格)
		/// </summary>
		public SectionString name;
		/// <summary>
		/// "="号后"."分隔的各个单词(包含空白)
		/// </summary>
		public SectionString[] wordStrings;

		public string ToString(string fileString,bool inclusiveWhite){
			string name=this.name.ToString(fileString);
			if(!inclusiveWhite){
				name=Regex.Replace(name,@"\s","");
			}
			
			string text="";
			int len=wordStrings.Length;
			for(int i=0;i<len;i++){
				string str=wordStrings[i].ToString(fileString);
				if(!inclusiveWhite){
					str=Regex.Replace(str,@"\s","");
				}
				text+=str;
				if(i<len-1)text+=",";
			}
			return string.Format("name:{0} wordStrings:{1}",name,text);
		}
	}
}