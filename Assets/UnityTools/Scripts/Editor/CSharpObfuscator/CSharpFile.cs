namespace UnityTools {
	using System.IO;
	using UnityEngine;
	using System.Collections.Generic;

	/// <summary>
	/// .cs文件
	/// </summary>
	public struct CSharpFile{

		//private List<SegmentString> _segmentStrings=new List<SegmentString>();
		
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

		/*public void addSegmentString(SegmentString segmentString){
			if(_segmentStrings.IndexOf(segmentString)>-1)return;
			_segmentStrings.Add(segmentString);
		}

		public List<SegmentString> segmentStrings{ get => _segmentStrings; }*/

	}
}