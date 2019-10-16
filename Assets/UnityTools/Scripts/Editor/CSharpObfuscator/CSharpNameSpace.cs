namespace UnityTools {
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
    using UnityEngine;

    public class CSharpNameSpace:CSharpRecord{
		protected CSharpNameSpace _parentNameSpace=null;
		/// <summary>命名空间括号内的using</summary>
		protected List<IUsing> _usings;
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

		protected List<IUsing> readUsings(string fileString,SectionString content){
			Debug2.Log("======================",content.startIndex,content.length);
			Debug.Log(content.ToString(fileString));
			//只查找到第一个左括号出现的位置
			int index=fileString.IndexOf('{',content.startIndex);
			int length=index<0?content.length:index-content.startIndex;
			//
			List<IUsing> usings=new List<IUsing>();
			//匹配所有using行
			Regex usingLineRegex=new Regex(@"using[\s\w\.]+;",RegexOptions.Compiled);
			//匹配空白或分号
			Regex whiteSemicolonRegex=new Regex(@"\s|;",RegexOptions.Compiled);
			//匹配如:"using xx;"或"using xxx.xxx.xxx;"
			//Regex usingRegex=new Regex(@"(using\s+\w+\s*;)|(using\s+(\s*\w+\s*\.\s*)+(\w+\s*);)",RegexOptions.Compiled);
			//匹配如:"using static xx;"或"using static xxx.xxx.xxx;"
			//Regex usingStaticRegex=new Regex(@"(using\s+static\s+\w+\s*;)|(using\s+static\s+(\s*\w+\s*\.\s*)+(\w+\s*);)",RegexOptions.Compiled);
			//匹配如:"using xx=xxx;"或"using xx=xxx.xxx.xxx;"
			Regex usingAliasRegex=new Regex(@"(\w+=\w+)|(\w+=(\w+\.)+\w+)",RegexOptions.Compiled);
			

			Match lineMatch=usingLineRegex.Match(fileString,content.startIndex,length);
			while(lineMatch.Success){
				SectionString usingLine=new SectionString(lineMatch.Index,lineMatch.Length);
				string usingLineString=lineMatch.Value;
				string oldUsingLineString=usingLineString;
				//去除"using"、空白、";"
				usingLineString=usingLineString.Substring(5);
				usingLineString=whiteSemicolonRegex.Replace(usingLineString,"");
				//是不是静态using
				bool isStatic=usingLineString.Substring(0,6)=="static";

				if(isStatic){//静态using，如:"using static xx;"或"using static xxx.xxx.xxx;"
					//去除"static"
					usingLineString=usingLineString.Substring(6);

					SectionString text=new SectionString();
					UsingString usingString=new UsingString(true,text);

				}else if(usingAliasRegex.IsMatch(usingLineString)){//using别名，如:"using xx=xxx;"或"using xx=xxx.xxx.xxx;"
					
				}else{//普通using，如:"using xx;"或"using xxx.xxx.xxx;"
					
				}
				lineMatch=lineMatch.NextMatch();
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

			int len=bracketBlocks.Count;
			for(int i=0;i<len;i++){
				SectionString bracketBlock=bracketBlocks[i];

				SectionString leftBracketString;
				if(matchNameSpaceLeftBracketString(bracketBlock,fileString,out leftBracketString)){
					CSharpNameSpace csNameSpace=createCSharpNameSpace(parentNameSpace,leftBracketString,bracketBlock);
					namespaces.Add(csNameSpace);
				}
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

		protected bool matchStructLeftBracketString(SectionString bracketBlock,string fileString,out SectionString result){
			
			result=new SectionString();
			return false;
		}
		
		protected bool matchInterfaceLeftBracketString(SectionString bracketBlock,string fileString,out SectionString result){
			
			result=new SectionString();
			return false;
		}

		protected bool matchEnumLeftBracketString(SectionString bracketBlock,string fileString,out SectionString result){
			
			result=new SectionString();
			return false;
		}
		
		protected bool matchDelegateLeftBracketString(SectionString bracketBlock,string fileString,out SectionString result){
			
			result=new SectionString();
			return false;
		}

	}
}
