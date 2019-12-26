namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Using别名，如:"using Project = PC.MyCompany.Project;"
    /// </summary>
    public class CSharpUsingAlias:IUsing{
		/// <summary>
		/// 别名名称(包含空白,但不包含using后的第一个空格)
		/// </summary>
		public Segment name;
		/// <summary>
		/// "="号后"."分隔的各个单词(包含空白)
		/// </summary>
		public DotPath words;
		
		public CSharpUsingAlias(Segment name,DotPath words){
			this.name=name;
			this.words=words;
		}

		public string ToString(string fileString){
			return ToString(fileString,true);
		}

		public string ToString(string fileString,bool includeWhitespace){
			string name=this.name.ToString();
			if(!includeWhitespace){
				name=Regex.Replace(name,@"\s","");
			}
			return $"name:{name} wordStrings:{words.ToString(fileString)}";
		}
	}
}
