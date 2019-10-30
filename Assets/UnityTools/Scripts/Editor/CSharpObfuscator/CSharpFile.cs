namespace UnityTools {
	using System.IO;
	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// .cs文件
	/// </summary>
	public class CSharpFile{

		private List<SectionString> _sectionStrings=new List<SectionString>();
		
		public FileInfo fileInfo;
		public string fileString;
		/// <summary>.cs文件内所有命名空间声明外的using</summary>
		public IUsing[] usings;
		public CSharpNameSpace[] nameSpaces;
		public CSharpClass[] classes;
		public CSharpStruct[] structs;
		public CSharpInterface[] interfaces;
		public CSharpEnum[] enums;
		public CSharpDelegate[] delegates;

		public void addSectionString(SectionString sectionString){
			if(_sectionStrings.IndexOf(sectionString)>-1)return;
			_sectionStrings.Add(sectionString);
		}

		public List<SectionString> sectionStrings{ get => _sectionStrings; }

	}
}