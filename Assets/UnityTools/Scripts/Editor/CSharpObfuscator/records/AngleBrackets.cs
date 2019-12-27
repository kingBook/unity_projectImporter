namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using UnityEngine.UIElements;

	/// <summary>
	/// 尖括号，如："&lt;...&gt;"
	/// </summary>
	public struct AngleBrackets:IString{
		
		public int startIndex;
		public int length;
		/// <summary>
		/// <para>尖括号里的一个或多个名称(","号分隔的各个名称)</para>
		/// <para>元素的类型可以为：<see cref="DotPathAngleBrackets"/>|<see cref="WordAngleBrackets"/>|<see cref="DotPath"/>|<see cref="Segment"/></para>
		/// </summary>
		public IString[] tNames;

		public AngleBrackets(int startIndex,int length,IString[] tNames){
			this.startIndex=startIndex;
			this.length=length;
			this.tNames=tNames;
		}

		public override string ToString(){
			throw new System.Exception("Please call ToString(string fileString)");
		}

		public string ToString(string fileString){
			const char splitChar=',';
			string strTNames="";
			int len=tNames.Length;
			for(int i=0;i<len;i++){
				strTNames+=tNames[i].ToString(fileString);
				if(i<len-1)strTNames+=splitChar;
			}
			return $"<{strTNames}>";
		}
	}
}
