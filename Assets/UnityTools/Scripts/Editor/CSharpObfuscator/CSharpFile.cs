namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using System.IO;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    /// <summary>
    /// cs文件
    /// </summary>
    public class CSharpFile:CSharpNameSpace{
		
		private FileInfo _fileInfo;
		private string _fileString;

		public void init(FileInfo fileInfo,string fileString){
			_fileInfo=fileInfo;
			_fileString=fileString;

			Debug.Log(_fileInfo.Name+"===================");
			//init(null,_fileString,_fileInfo.Name,0,_fileString.Length);




			_parentNameSpace=null;
			_name=_fileInfo.Name;
			_startIndex=0;
			_length=_fileString.Length;
			
			_usings=readUsings(fileString,startIndex,length);
			_bracketBlocks=readBracketBlocks(fileString,startIndex,length);

			
			readObjectsWithBracketBlocks(this,_bracketBlocks,fileString,out _nameSpaces,out _classes,out _structs,out _interfaces,out _enums,out _delegates);
		}
	
	}
}