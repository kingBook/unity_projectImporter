namespace UnityTools {
	using System.IO;
	using UnityEngine;

	/// <summary>
	/// .cs文件
	/// </summary>
	public class CSharpFile:CSharpNameSpace{
		
		private FileInfo _fileInfo;
		public string fileString;

		public void init(FileInfo fileInfo,string fileString){
			_fileInfo=fileInfo;
			this.fileString=fileString;
			//
			Debug.Log(_fileInfo.Name+"===================");
			//.cs文件在文件内没有名称字段，使用new SectionString()占位
			SectionString name=new SectionString();
			//所有.cs文件内容长度
			SectionString content=new SectionString(0,this.fileString.Length);
			//
			init(this,null,name,content);
		}

	}
}