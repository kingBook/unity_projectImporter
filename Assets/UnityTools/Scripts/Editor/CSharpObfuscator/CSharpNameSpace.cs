namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    public class CSharpNameSpace:CSharpRecord{
		protected CSharpNameSpace _parentNameSpace=null;

		protected List<string> _usings;
		protected List<BracketBlock> _bracketBlocks;
		protected List<CSharpNameSpace> _nameSpaces;
		protected List<CSharpClass> _classes;
		protected List<CSharpStruct> _structs;
		protected List<CSharpInterface> _interfaces;
		protected List<CSharpEnum> _enums;
		protected List<CSharpDelegate> _delegates;

		public void init(CSharpNameSpace parentNameSpace,string fileString,string name,int startIndex,int length){
			_parentNameSpace=parentNameSpace;
			_name=name;
			_startIndex=startIndex;
			_length=length;
			
			_usings=readUsings(fileString,startIndex,length);
			_bracketBlocks=readBracketBlocks(fileString,startIndex,length);
			for(int i=0;i<_bracketBlocks.Count;i++){
				Debug.Log(_bracketBlocks[i].ToString(fileString));
			}
			readObjectsWithBracketBlocks(this,_bracketBlocks,fileString,out _nameSpaces,out _classes,out _structs,out _interfaces,out _enums,out _delegates);
			
		}

		protected List<string> readUsings(string fileString,int startIndex,int length){
			//Debug.LogFormat("{0},{1},{2}",name,startIndex,length);
			//Debug.Log(this.ToString(fileString));
			return null;
		}

		/// <summary>
		/// 从指定的同级括号块列表读取对象
		/// </summary>
		/// <param name="parentNameSpace"></param>
		/// <param name="bracketBlocks"></param>
		/// <param name="fileString"></param>
		/// <param name="namespaces"></param>
		/// <param name="classes"></param>
		/// <param name="structs"></param>
		/// <param name="interfaces"></param>
		/// <param name="enums"></param>
		/// <param name="delegates"></param>
		protected void readObjectsWithBracketBlocks(CSharpNameSpace parentNameSpace,List<BracketBlock> bracketBlocks,string fileString,
		out List<CSharpNameSpace> namespaces,
		out List<CSharpClass> classes,
		out List<CSharpStruct> structs,
		out List<CSharpInterface> interfaces,
		out List<CSharpEnum> enums,
		out List<CSharpDelegate> delegates){
			
			namespaces=new List<CSharpNameSpace>();
			classes=new List<CSharpClass>();
			structs=new List<CSharpStruct>();
			interfaces=new List<CSharpInterface>();
			enums=new List<CSharpEnum>();
			delegates=new List<CSharpDelegate>();
			//匹配如："namespace xxx{","class xxx{"等
			Regex leftBracketRegex=new Regex(@"\w+\s+\S+\s*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			//匹配"class"，"namespace"等
			Regex typeRegex=new Regex(@"\w+",RegexOptions.Compiled);
			//匹配类名称，命名空间名称等
			Regex nameRegex=new Regex(@"\S+",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int len=bracketBlocks.Count;
			for(int i=0;i<len;i++){
				BracketBlock bracketBlock=bracketBlocks[i];
				Match leftBracketMatch=leftBracketRegex.Match(fileString,bracketBlock.startIndex+1);
				if(leftBracketMatch.Success){
					Debug.Log(leftBracketMatch.Value);
					//匹配成功，并去掉"{"
					string leftBracketMatchValue=leftBracketMatch.Value.Replace("{","");
					string type=typeRegex.Match(leftBracketMatchValue).Value;
					string name=nameRegex.Match(leftBracketMatchValue,leftBracketMatchValue.Length).Value;
					if(type=="namespace"){
						CSharpNameSpace csNameSpace=new CSharpNameSpace();
						//命名空间内容从命名空间声明"{"的右边开始
						int startIndex=bracketBlock.startIndex+1;
						//"}"的左边结束(就是减去两个大括号的长度)
						int length=bracketBlock.length-2;
						Debug.Log(bracketBlock.ToString(fileString));
						//Debug.LogFormat("name:{0},startIndex:{1},length:{2}",name,startIndex,length);
						csNameSpace.init(parentNameSpace,fileString,name,startIndex,length);
						namespaces.Add(csNameSpace);
					}else if(type=="class"){
						
					}else if(type=="struct"){
						
					}else if(type=="interface"){
						
					}else if(type=="enum"){
						
					}else if(type=="delegate"){
						
					}
				}
			}
		}

	}
}