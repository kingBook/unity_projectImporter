namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Using别名，如:"using Project = PC.MyCompany.Project;"
    /// </summary>
    public class CSharpUsingAlias:IAlignObject,IUsing{
		/// <summary>
		/// 别名名称(不包含空白)
		/// </summary>
		public Segment name;
		/// <summary>
		/// "="号后"."分隔的一个单词<see cref="Segment"/>或<see cref="DotPath"/>
		/// </summary>
		public IString words;
		
		public CSharpUsingAlias(Segment name,IString words){
			this.name=name;
			this.words=words;
		}

		public override string ToString(){
			throw new System.Exception("Please call ToString(string fileString)");
		}

		public string ToString(string fileString){
			string name=this.name.ToString(fileString);
			/*if(!includeWhitespace){
				name=Regex.Replace(name,@"\s","");
			}*/
			return $"name:{name} words:{words.ToString(fileString)}";
		}
	}
}
