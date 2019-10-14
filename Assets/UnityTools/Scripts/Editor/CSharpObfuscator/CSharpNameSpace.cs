namespace UnityTools {
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
    using UnityEngine;

    public class CSharpNameSpace:CSharpRecord{
		protected CSharpNameSpace _parentNameSpace=null;
		protected List<SectionString> _usings;
		protected List<CSharpNameSpace> _nameSpaces;
		protected List<CSharpClass> _classes;
		protected List<CSharpStruct> _structs;
		protected List<CSharpInterface> _interfaces;
		protected List<CSharpEnum> _enums;
		protected List<CSharpDelegate> _delegates;

		public void init(CSharpFile cSharpFile,CSharpNameSpace parentNameSpace,SectionString name,SectionString content){
			_cSharpFile=cSharpFile;
			_parentNameSpace=parentNameSpace;
			_name=name;
			_content=content;
			
			_usings=readUsings(_cSharpFile.fileString,content);
			List<SectionString> bracketBlocks=readBracketBlocks(_cSharpFile.fileString,content);
			readObjectsWithBracketBlocks(this,bracketBlocks,_cSharpFile.fileString,out _nameSpaces,out _classes,out _structs,out _interfaces,out _enums,out _delegates);
		}

		protected List<SectionString> readUsings(string fileString,SectionString content){
			Debug.Log("======================");
			//Debug.Log(content.ToString(fileString));
			//只查找到第一个左括号出现的位置
			int index=fileString.IndexOf('{',content.startIndex);
			int length=index<0?content.length:index-content.startIndex;
			//
			List<SectionString> usings=new List<SectionString>();
			//匹配如:"using xx;"或"using xxx.xxx.xxx;"
			Regex regex=new Regex(@"(using\s+\w+\s*;)|(using\s(\s*\w+\s*\.\s*)+(\w+\s*);)",RegexOptions.Compiled);
			//Regex regex=new Regex(@"using\s+\w+\s*;",RegexOptions.Compiled);
			/*Match match=regex.Match(fileString);
			if(match.Success){
				Debug.Log(match.Value);
			}*/
			MatchCollection matchCollection=regex.Matches(fileString,content.startIndex);
			int count=matchCollection.Count;
			//Debug.Log(count);
			for(int i=0;i<count;i++){
				Match match=matchCollection[i];
				if(match.Success){
					string matchValue=Regex.Replace(match.Value,@"\s","",RegexOptions.Compiled);
					Debug.Log(matchValue);
				}
			}
			return usings;
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
		protected void readObjectsWithBracketBlocks(CSharpNameSpace parentNameSpace,List<SectionString> bracketBlocks,string fileString,
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

			//匹配如："namespace xxx{","class xxx{"，"class xxx:xxx{"等
			//Regex leftBracketRegex=new Regex(@"\w+\s+\S+\s*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			//匹配"class"，"namespace"等
			//Regex typeRegex=new Regex(@"\w+",RegexOptions.Compiled);
			//匹配类名称，命名空间名称等
			//Regex nameRegex=new Regex(@"\S+",RegexOptions.Compiled|RegexOptions.RightToLeft);

			int len=bracketBlocks.Count;
			for(int i=0;i<len;i++){
				SectionString bracketBlock=bracketBlocks[i];

				SectionString leftBracketString;
				if(matchNameSpaceLeftBracketString(bracketBlock,fileString,out leftBracketString)){
					CSharpNameSpace csNameSpace=createCSharpNameSpace(parentNameSpace,leftBracketString,bracketBlock);
					namespaces.Add(csNameSpace);
				}

				/*
				Match leftBracketMatch=leftBracketRegex.Match(fileString,bracketBlock.startIndex+1);
				if(leftBracketMatch.Success){
					//匹配成功，并去掉"{"
					//string leftBracketMatchValue=leftBracketMatch.Value.Replace("{","");
					//string type=typeRegex.Match(leftBracketMatchValue).Value;
					//string name=nameRegex.Match(leftBracketMatchValue,leftBracketMatchValue.Length).Value;

					if(type=="namespace"){
						CSharpNameSpace csNameSpace=new CSharpNameSpace();
						//命名空间内容从命名空间声明"{"的右边开始,"}"的左边结束(就是减去两个大括号的长度)
						SectionString section=new SectionString(bracketBlock.startIndex+1,bracketBlock.length-2);
						csNameSpace.init(_cSharpFile,parentNameSpace,section);
						namespaces.Add(csNameSpace);
					}else if(type=="class"){
						//CSharpClass csClass=new CSharpClass();
						//csClass.init()
					}else if(type=="struct"){
						
					}else if(type=="interface"){
						
					}else if(type=="enum"){
						
					}else if(type=="delegate"){
						
					}
				}*/
			}
		}

		/// <summary>
		/// 根据指定的大括号块匹配命名空间声明，并通过out参数输出。如:"namespace unity_tags{"
		/// </summary>
		/// <param name="bracketBlock">大括号块，包含大括号</param>
		/// <param name="fileString">.cs文件字符串</param>
		/// <param name="result">输出结果，如:"namespace unity_tags{"。</param>
		/// <returns></returns>
		protected bool matchNameSpaceLeftBracketString(SectionString bracketBlock,string fileString,out SectionString result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(fileString,bracketBlock.startIndex);
			//命名空间正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"namespace\s+\w+\s*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=new SectionString(match.Index,match.Value.Length);
				return true;
			}
			result=new SectionString();
			return false;
		}

		/// <summary>
		/// 创建CSharpNameSpace
		/// </summary>
		/// <param name="parentNameSpace">父级命名空间</param>
		/// <param name="leftBracketString">左括号命名空间声明，如："namespace unity_tags{"。</param>
		/// <param name="bracketBlock">命名空间包含的括号块，包含大括号</param>
		/// <returns></returns>
		protected CSharpNameSpace createCSharpNameSpace(CSharpNameSpace parentNameSpace,SectionString leftBracketString,SectionString bracketBlock){
			CSharpNameSpace csNameSpace=new CSharpNameSpace();
			//命名空间名称
			Regex regex=new Regex(@"\w+",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int len=leftBracketString.length-1;//去掉"{"
			Match match=regex.Match(_cSharpFile.fileString,leftBracketString.startIndex,len);
			SectionString name=new SectionString(match.Index,match.Length);
			//命名空间内容，从命名空间声明"{"的右边开始,"}"的左边结束(就是减去两个大括号的长度)
			SectionString content=new SectionString(bracketBlock.startIndex+1,bracketBlock.length-2);
			csNameSpace.init(_cSharpFile,parentNameSpace,name,content);
			return csNameSpace;
		}

		protected bool matchClassLeftBracketString(SectionString bracketBlock,string fileString,out SectionString result){
			
			result=new SectionString();
			return false;
		}
	}
}