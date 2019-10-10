namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// cs文件
	/// </summary>
	public class CSharpFile:CSharpReader{
		
		public string name;
		public string[] fileLines;

		public CSharpNameSpace[] nameSpaces;
		public CSharpClass[] classes;
		public CSharpStruct[] structs;
		public CSharpInterface[] interfaces;
		public CSharpEnum[] enums;
		public CSharpDelegate[] delegates;

		public static CSharpFile create(string name,string[] fileLines){
			CSharpFile csFile=new CSharpFile();
			csFile.init(name,fileLines);
			return csFile;
		}

		private void init(string name,string[] fileLines){
			this.name=name;
			this.fileLines=fileLines;

			nameSpaces=readNameSpaces(fileLines,0,0,fileLines.Length-1,-1);
		}

		
	
	}
}