namespace UnityTools {
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
    using UnityEngine;

    public class CSharpNameSpace{
	    public static readonly CSharpNameSpace None=new CSharpNameSpace();
	    
	    public CSharpNameSpace parent;
	    public SegmentString[] nameWords;
		/// <summary>命名空间括号内的using</summary>
		public IUsing[] usings;
		public SegmentString content;
		public CSharpNameSpace[] nameSpaces;
		public CSharpClass[] classes;
		public CSharpStruct[] structs;
		public CSharpInterface[] interfaces;
		public CSharpEnum[] enums;
		public CSharpDelegate[] delegates;
		
		public string getNameWordsString(string fileString,bool includeWhitespace=false){
			return this==None?"None":SegmentString.getWordsString(nameWords,".",includeWhitespace);
		}
		
	}
}
