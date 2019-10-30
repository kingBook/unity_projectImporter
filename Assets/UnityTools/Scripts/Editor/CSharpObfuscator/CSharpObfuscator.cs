using UnityEngine;

namespace UnityTools {
	using System.IO;
	using UnityEditor;
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	
	/// <summary>
	/// CSharp混淆器
	/// <br>一个混淆器只用于一个unity项目</br>
	/// </summary>
	public class CSharpObfuscator{
		
		/// <summary>只能在Assets文件夹下的一级子目录的特殊文件夹</summary>
		private readonly string[] _ignoreRootFolderNames=new string[]{"EditorDefaultResources","Gizmos","Plugins","StandardAssets","StreamingAssets"};
		/// <summary>可以在Assets文件夹下的任意子目录的特殊文件夹</summary>
		private readonly string[] _ignoreFolderNames=new string[]{"Editor","DOTween"};

		/// <summary>
		/// 混淆一个unity项目
		/// </summary>
		/// <param name="projectAssetsPath">unity项目的Assets文件夹路径</param>
		/// <param name="onComplete">完成时的回调函数</param>
		public void obfuscateProject(string projectAssetsPath,Action onComplete){
			projectAssetsPath=projectAssetsPath.Replace("\\","/");
			//
			string[] files=Directory.GetFiles(projectAssetsPath,"*.cs",SearchOption.AllDirectories);
			//读取所有.cs文件，生成CSharpFile列表
			CSharpFile[] cSharpFiles=readFiles(projectAssetsPath,files);
			//混淆CSharpFile列表
			obfuscateCSharpFiles(cSharpFiles,onComplete);
		}

		/// <summary>
		/// 读取所有.cs文件,生成CSharpFile列表并返回。
		/// </summary>
		/// <param name="projectAssetsPath"></param>
		/// <param name="files"></param>
		/// <returns></returns>
		private CSharpFile[] readFiles(string projectAssetsPath,string[] files){
			int len=files.Length;
			CSharpFile[] cSharpFiles=new CSharpFile[len];
			EditorUtility.DisplayProgressBar("Read Files","Readying...",0.0f);
			for(int i=0;i<len;i++){
				string filePath=files[i];
				filePath=filePath.Replace("\\","/");
				FileInfo fileInfo=new FileInfo(filePath);
				//
				if(isIgnoreFolderFile(projectAssetsPath,fileInfo.Directory.FullName)){
					continue;
				}
				Debug.Log("read "+fileInfo.Name+"==============================");
				//显示进度
				string shortFilePath=filePath.Replace(projectAssetsPath,"");
				EditorUtility.DisplayProgressBar("Read Files","Reading "+shortFilePath,(float)(i+1)/len);
				//读取文件到字符串
				string fileString=FileUtil2.getFileString(filePath);
				//创建CSharpFile
				CSharpFile cSharpFile=createCSharpFile(fileInfo,fileString);
				//清除CSharpFile里的注释内容
				clearFileStringComments(cSharpFile);
				//读取CSharpFile里的内容
				readCSharpFileContent(cSharpFile);
				cSharpFiles[i]=cSharpFile;
			}
			EditorUtility.ClearProgressBar();
			return cSharpFiles;
		}
		
		#region clearFileStringComments
		/// <summary>
		/// 清除.cs文件字符串中的所有注释内容
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		private void clearFileStringComments(CSharpFile cSharpFile){
			SectionString[] texts=readStringsWithFile(cSharpFile);
			for(int i=0;i<texts.Length;i++){
				Debug.Log(texts[i]);
			}
			SectionString[] lineComments=readFileStringLineComments(cSharpFile);
			SectionString[] blockComments=readFileStringBlockComments(cSharpFile);
			//cSharpFile.fileString=
			//cSharpFile.content=
		}
		
		/// <summary>
		/// 读取.cs文件中的字符串,如："abc"、@"abc"等
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <returns></returns>
		private SectionString[] readStringsWithFile(CSharpFile cSharpFile){
			//匹配@"xxx"、"xxx"、test("xxx",@"xxx")括号里的"xxx",@"xxx"等字符串（在@符号后的双引号转义使用两个")。
			Regex stringRegex=new Regex(@"(@"".*"")|("".*"")",RegexOptions.Compiled);
			MatchCollection matches=stringRegex.Matches(cSharpFile.fileString);
			int count=matches.Count;
			List<SectionString> sectionStrings=new List<SectionString>();
			for(int i=0;i<count;i++){
				Match match=matches[i];
				SectionString[] generalStrings=getStringsWithMatch(cSharpFile,match,true);
				sectionStrings.AddRange(generalStrings);
			}
			return sectionStrings.ToArray();
		}

		/// <summary>
		/// 从指定的Match中返回字符串内容
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="match">包含字符串的Match</param>
		/// <param name="isIncludeAtMark">@模式的字符串，返回时是否包含"@"标记</param>
		/// <returns></returns>
		private SectionString[] getStringsWithMatch(CSharpFile cSharpFile,Match match,bool isIncludeAtMark){
			List<SectionString> sectionStrings=new List<SectionString>();
			string matchValue=match.Value;
			int len=matchValue.Length;
			//引号(")计数
			int doubleQuotationMarkCount=0;
			//一段字符串的开始索引,从引号(")开始
			int startIndex=0;
			//是不是@模式字符串，每次字符串开头引号(")都重新赋值
			bool isAtPattern=false;
			//表示上一段字符串查找是否已结束
			bool isLastOver=true;
			for(int i=0;i<len;i++){
				//当前char是不是字符串引号(")，@字符串时排除("")，常规字符串时排除(\")
				bool isQuotationMark=false;
				if(matchValue[i]=='"'){
					if(isLastOver){
						//上一段字符串查找已结束，设置是不是@模式字符串
						isAtPattern=i>0 && matchValue[i-1]=='@';
					}
					if(isAtPattern){
						if(isLastOver && matchValue[i-1]=='@'){
							//如果上一段字符患上查找结束，上一个字符是"@"的一定是字符串引号(")
							isQuotationMark=true;
						}else if(i+1<len-1){//非最后一个字符
							if(matchValue[i+1]=='"'){
								//在@符号后的双引号转义使用两个"，所以当前和下一个都是引号(")则不是字符串引号(")，
								//i+=1，跳过检查下一个索引
								i+=1;
							}else{
								//下一个不是引号(")时，当前则是字符串引号(")
								isQuotationMark=true;
							}
						}else{
							//最后一个字符，一定是字符串引号(")
							isQuotationMark=true;
						}
					}else{
						if(i>0){
							//常规字符串(非@模式字符串)，上一个不是转义符(\)则是字符串引号(")
							if(matchValue[i-1]!='\\')isQuotationMark=true;
						}else{
							isQuotationMark=true;
						}
					}
				}
				//如果是字符串引号(")，则计算并截切
				if(isQuotationMark){
					doubleQuotationMarkCount++;
					if((doubleQuotationMarkCount&1)==1){
						isLastOver=false;
						//第一个字符串引号(")，设置开始索引
						startIndex=i;
					}else{
						//第二个字符串引号(")，截取字符串
						int strStartIndex=match.Index+startIndex;
						int strLength=i-startIndex+1;
						if(isAtPattern&&isIncludeAtMark){
							strStartIndex-=1;
							strLength+=1;
						}
						SectionString sectionString=new SectionString(cSharpFile,strStartIndex,strLength);
						sectionStrings.Add(sectionString);
						isAtPattern=false;
						isLastOver=true;
					}
				}
			}
			return sectionStrings.ToArray();
		}
		
		/// <summary>
		/// 读取.cs文件中的块注释
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <returns></returns>
		private SectionString[] readFileStringBlockComments(CSharpFile cSharpFile){
			Regex regex=new Regex(@"",RegexOptions.Compiled);
			return null;
		}
		
		/// <summary>
		/// 读取.cs文件中的行注释
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <returns></returns>
		private  SectionString[] readFileStringLineComments(CSharpFile cSharpFile){
			return null;

		}
		#endregion

		/// <summary>
		/// 文件夹路径是不是忽略的文件夹
		/// </summary>
		/// <param name="projectAssetsPath">项目的Assets路径</param>
		/// <param name="folderPath">.cs文件所在的文件夹路径</param>
		/// <returns></returns>
		private bool isIgnoreFolderFile(string projectAssetsPath,string folderPath){
			folderPath=folderPath.Replace("\\","/");
			//检测一级忽略的目录
			int i=_ignoreRootFolderNames.Length;
			while(--i>=0){
				string ignoreFolderPath=projectAssetsPath+"/"+_ignoreRootFolderNames[i];
				if(folderPath.IndexOf(ignoreFolderPath,StringComparison.Ordinal)>-1){
					return true;
				}
			}
			//检测所有子级忽略的目录
			//取去掉"Assets"之前的部分（包含"Assets"）
			folderPath=folderPath.Replace(projectAssetsPath,"");
			i=_ignoreFolderNames.Length;
			while(--i>=0){
				if(folderPath.IndexOf(_ignoreFolderNames[i],StringComparison.Ordinal)>-1){
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 混淆CSharpFile列表
		/// </summary>
		/// <param name="cSharpFiles">CSharpFile数组列表</param>
		/// <param name="onComplete">混淆完成时的回调函数</param>
		private void obfuscateCSharpFiles(CSharpFile[] cSharpFiles,Action onComplete){
			
		}
		
		/// <summary>
		/// 创建cCSharpFile
		/// </summary>
		/// <param name="fileInfo">.cs文件信息</param>
		/// <param name="fileString">.cs文件字符串</param>
		/// <returns></returns>
		private CSharpFile createCSharpFile(FileInfo fileInfo,string fileString){
			CSharpFile file=new CSharpFile();
			file.fileInfo=fileInfo;
			file.fileString=fileString;
			return file;
		}

		/// <summary>
		/// 读取指定CSharpFile里的内容
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		private void readCSharpFileContent(CSharpFile cSharpFile){
			SectionString content=new SectionString(cSharpFile,0,cSharpFile.fileString.Length);
			cSharpFile.usings=readUsings(cSharpFile,content);
			
			List<SectionString> bracketBlocks=readBracketBlocks(cSharpFile,content);
			List<CSharpNameSpace> namespaces;
			List<CSharpClass> classes;
			List<CSharpStruct> structs;
			List<CSharpInterface> interfaces;
			List<CSharpEnum> enums;
			List<CSharpDelegate> delegates;
			readNameSpaceSubObjects(cSharpFile,CSharpNameSpace.None,bracketBlocks,out namespaces,out classes,out structs,out interfaces,out enums,out delegates);
			
			cSharpFile.nameSpaces=namespaces.ToArray();
			cSharpFile.classes=classes.ToArray();
			cSharpFile.structs=structs.ToArray();
			cSharpFile.interfaces=interfaces.ToArray();
			cSharpFile.enums=enums.ToArray();
			cSharpFile.delegates=delegates.ToArray();
		}
		
		/// <summary>
		/// 创建CSharpNameSpace
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>create
		/// <param name="parentNameSpace">父级命名空间</param>
		/// <param name="leftBracketString">左括号命名空间声明，如："namespace unity_tags{"。</param>
		/// <param name="bracketBlock">命名空间包含的括号块，包含大括号</param>
		/// <returns></returns>
		private CSharpNameSpace createNameSpace(CSharpFile cSharpFile,CSharpNameSpace parentNameSpace,SectionString leftBracketString,SectionString bracketBlock){
			CSharpNameSpace csNameSpace=new CSharpNameSpace();
			csNameSpace.parent=parentNameSpace;
			
			//命名空间名称
			int startIndex=leftBracketString.startIndex+10;//从"namespace "右侧开始
			int length=leftBracketString.length-10-1;//减去"namespace "和"{"的长度
			SectionString[] nameWords=readWords(cSharpFile,new SectionString(cSharpFile,startIndex,length));
			csNameSpace.nameWords=nameWords;
			//Debug.Log(csNameSpace.getNameWordsString(cSharpFile.fileString));
			
			//命名空间内容，从命名空间声明"{"的右边开始,"}"的左边结束(就是减去两个大括号的长度)
			SectionString content=new SectionString(cSharpFile,bracketBlock.startIndex+1,bracketBlock.length-2);
			csNameSpace.usings=readUsings(cSharpFile,content);
			csNameSpace.content=content;
			
			List<SectionString> bracketBlocks=readBracketBlocks(cSharpFile,content);
			List<CSharpNameSpace> namespaces;
			List<CSharpClass> classes;
			List<CSharpStruct> structs;
			List<CSharpInterface> interfaces;
			List<CSharpEnum> enums;
			List<CSharpDelegate> delegates;
			readNameSpaceSubObjects(cSharpFile,csNameSpace,bracketBlocks,out namespaces,out classes,out structs,out interfaces,out enums,out delegates);
			
			csNameSpace.nameSpaces=namespaces.ToArray();
			csNameSpace.classes=classes.ToArray();
			csNameSpace.structs=structs.ToArray();
			csNameSpace.interfaces=interfaces.ToArray();
			csNameSpace.enums=enums.ToArray();
			csNameSpace.delegates=delegates.ToArray();
			return csNameSpace;
		}
		
		/// <summary>
		/// 创建CSharpClass
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="parentNameSpace">类所在的命名空间</param>
		/// <param name="leftBracketString">左括号类声明，如："public class Main{"。</param>
		/// <param name="bracketBlock">类包含的括号块，包含大括号</param>
		/// <returns></returns>
		private CSharpClass createClass(CSharpFile cSharpFile,CSharpNameSpace parentNameSpace,SectionString leftBracketString,SectionString bracketBlock){
			CSharpClass csClass=new CSharpClass();
			
			return csClass;
		}
		
		/// <summary>
		/// 读取括号块，"{...}"包含括号,不读取子级括号块
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="content">读取内容的位置和长度</param>
		/// <returns></returns>
		private List<SectionString> readBracketBlocks(CSharpFile cSharpFile,SectionString content){
			List<SectionString> bracketBlocks=new List<SectionString>();
			
			int startIndex=content.startIndex;
			int end=startIndex+content.length;

			int bracketCount=0;
			int bracketBlockStartIndex=0;

			for(int i=startIndex;i<end;i++){
				char charString=cSharpFile.fileString[i];
				if(charString=='{'){
					if(bracketCount>0){
						bracketCount++;
					}else{
						bracketBlockStartIndex=i;
						bracketCount=1;
					}
				}else if(charString=='}'){
					bracketCount--;
					if(bracketCount==0){
						int bracketBlockLength=i-bracketBlockStartIndex+1;
						SectionString bracketBlock=new SectionString(cSharpFile,bracketBlockStartIndex,bracketBlockLength);
						bracketBlocks.Add(bracketBlock);
					}
				}
			}
			return bracketBlocks;
		}
		
		/// <summary>
		/// 读取以"."分隔的各个单词(包含空白),如:"System.Text.RegularExpressions"，将得到"System","Text","RegularExpressions"
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="sectionString">如："System.Text.RegularExpressions"</param>
		/// <returns></returns>
		private SectionString[] readWords(CSharpFile cSharpFile,SectionString sectionString){
			string usingContent=sectionString.ToString();
			string[] splitStrings=usingContent.Split('.');
			int len=splitStrings.Length;
			int startIndex=sectionString.startIndex;
			SectionString[] words=new SectionString[len];
			for(int i=0;i<len;i++){
				int tempLength=splitStrings[i].Length;
				words[i]=new SectionString(cSharpFile,startIndex,tempLength);
				//加一个单词和"."的长度
				startIndex+=tempLength+1;
			}
			return words;
		}
		
		#region ReadUsings
		/// <summary>
		/// 读取指定内容块的所有Using(不读取内嵌套大括号的using)
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="content">内容块</param>
		/// <returns></returns>
		private IUsing[] readUsings(CSharpFile cSharpFile,SectionString content){
			//只查找到第一个左括号出现的位置
			int index=cSharpFile.fileString.IndexOf('{',content.startIndex);
			int length=index<0?content.length:index-content.startIndex;
			//
			List<IUsing> usings=new List<IUsing>();
			//匹配所有using行
			Regex usingLineRegex=new Regex(@"using[\s\w\.=]+;",RegexOptions.Compiled);
			//匹配空白或分号
			Regex whiteSemicolonRegex=new Regex(@"\s|;",RegexOptions.Compiled);
			//匹配如:"using xx=xxx;"或"using xx=xxx.xxx.xxx;"
			Regex usingAliasRegex=new Regex(@"(\w+=\w+)|(\w+=(\w+\.)+\w+)",RegexOptions.Compiled);

			Match lineMatch=usingLineRegex.Match(cSharpFile.fileString,content.startIndex,length);
			while(lineMatch.Success){
				SectionString usingLine=new SectionString(cSharpFile,lineMatch.Index,lineMatch.Length);
				//去除"using"、空白、";"
				string usingLineString=lineMatch.Value;
				usingLineString=usingLineString.Substring(5);
				usingLineString=whiteSemicolonRegex.Replace(usingLineString,"");
				//
				if(usingAliasRegex.IsMatch(usingLineString)){//using别名，如:"using xx=xxx;"或"using xx=xxx.xxx.xxx;"
					UsingAlias usingAlias=readUsingAlias(cSharpFile,usingLine);
					usings.Add(usingAlias);
				}else{
					//是不是静态using
					bool isStatic=usingLineString.Substring(0,6)=="static";
					if(isStatic){//静态using，如:"using static xx;"或"using static xxx.xxx.xxx;"
						UsingString usingString=readStaticUsing(cSharpFile,usingLine);
						usings.Add(usingString);
					}else{//普通using，如:"using xx;"或"using xxx.xxx.xxx;"
						UsingString usingString=readUsing(cSharpFile,usingLine);
						usings.Add(usingString);
					}
				}
				lineMatch=lineMatch.NextMatch();
			}
			return usings.ToArray();
		}
		
		/// <summary>
		/// 读取普通的Using
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="usingLine">using行内容块，如："using System.IO;"</param>
		/// <returns></returns>
		private UsingString readUsing(CSharpFile cSharpFile,SectionString usingLine){
			string usingLineString=usingLine.ToString();
			//匹配"using "。
			Match headMatch=Regex.Match(usingLineString,@"using\s",RegexOptions.Compiled);
			
			int startIndex=usingLine.startIndex+headMatch.Length;
			//长度为:减去"using "的长度，再减去";"的长度
			int length=usingLine.length-headMatch.Length-1;
			SectionString[] wordStrings=readWords(cSharpFile,new SectionString(cSharpFile,startIndex,length));
			UsingString usingString=new UsingString(false,wordStrings);
			
			return usingString;
		}
		
		/// <summary>
		/// 读取静态Using
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="usingLine">using行内容块，如："using static UnityEngine.Mathf;"</param>
		/// <returns></returns>
		private UsingString readStaticUsing(CSharpFile cSharpFile,SectionString usingLine){
			string usingLineString=usingLine.ToString();
			//匹配"using static "。
			Match headMatch=Regex.Match(usingLineString,@"using\s+static\s",RegexOptions.Compiled);
			
			int startIndex=usingLine.startIndex+headMatch.Length;
			//长度为:减去"using static "的长度，再减去";"的长度
			int length=usingLine.length-headMatch.Length-1;
			SectionString[] wordStrings=readWords(cSharpFile,new SectionString(cSharpFile,startIndex,length));
			UsingString usingString=new UsingString(true,wordStrings);
			
			return usingString;
		}
		
		/// <summary>
		/// 读取Using别名
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="usingLine">using行内容块，如："using static UnityEngine.Mathf;"</param>
		/// <returns></returns>
		private UsingAlias readUsingAlias(CSharpFile cSharpFile,SectionString usingLine){
			string usingLineString=usingLine.ToString();
			//匹配"using xxx="
			Match headMatch=Regex.Match(usingLineString,@"using\s+\w+=",RegexOptions.Compiled);
			
			//开始索引:加上"using "的长度
			int startIndex=usingLine.startIndex+6;
			//name长度为:"using xxx="的长度-"using "长度-"="长度
			int length=headMatch.Length-6-1;
			SectionString name=new SectionString(cSharpFile,startIndex,length);
			
			startIndex=usingLine.startIndex+headMatch.Length;
			//长度为:减去"using xxx= "的长度，再减去";"的长度
			length=usingLine.length-headMatch.Length-1;
			SectionString[] words=readWords(cSharpFile,new SectionString(cSharpFile,startIndex,length));
			
			UsingAlias usingAlias=new UsingAlias(name,words);
			return usingAlias;
		}
		#endregion

		/// <summary>
		/// 读取命名空间的子对象
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="parentNameSpace">父级命名空间</param>
		/// <param name="bracketBlocks">命名空间内的大括号内容块列表（包含"{}"）</param>
		/// <param name="namespaces">输出的命名空间列表</param>
		/// <param name="classes">输出的类列表</param>
		/// <param name="structs">输出的结构体列表</param>
		/// <param name="interfaces">输出的接口列表</param>
		/// <param name="enums">输出的枚举列表</param>
		/// <param name="delegates">输出的委托列表</param>
		private void readNameSpaceSubObjects(CSharpFile cSharpFile,CSharpNameSpace parentNameSpace,List<SectionString> bracketBlocks,
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
				if(matchNameSpaceLeftBracketString(cSharpFile,bracketBlock,out leftBracketString)){
					CSharpNameSpace csNameSpace=createNameSpace(cSharpFile,parentNameSpace,leftBracketString,bracketBlock);
					namespaces.Add(csNameSpace);
				}else if(matchClassLeftBracketString(cSharpFile,bracketBlock,out leftBracketString)){
					//Debug2.Log(parentNameSpace.getNameWordsString(cSharpFile.fileString),leftBracketString.ToString(cSharpFile.fileString));
					CSharpClass csClass=createClass(cSharpFile,parentNameSpace,leftBracketString,bracketBlock);
					classes.Add(csClass);
				}else if(matchStructLeftBracketString(cSharpFile,bracketBlock,out leftBracketString)){
					
				}else if(matchEnumLeftBracketString(cSharpFile,bracketBlock,out leftBracketString)){
					
				}else if(matchDelegateLeftBracketString(cSharpFile,bracketBlock,out leftBracketString)){
					
				}
			}
		}

		/// <summary>
		/// 根据指定的大括号块匹配命名空间声明，并通过out参数输出。如:"namespace unity_tags{"
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">大括号块，包含大括号</param>
		/// <param name="result">输出结果，如:"namespace unity_tags{"。</param>
		/// <returns></returns>
		private bool matchNameSpaceLeftBracketString(CSharpFile cSharpFile,SectionString bracketBlock,out SectionString result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//匹配命名空间的正则表达式，从"{"的右侧向左查找
			Regex regex=new Regex(@"namespace\s+((\w+)|((\w+\s*\.\s*)+\w+))\s*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=new SectionString(cSharpFile,match.Index,match.Value.Length);
				return true;
			}
			result=new SectionString();
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配类声明，并通过out参数输出结果。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">类包含的括号块，包含大括号</param>
		/// <param name="result">输出结果，如:"class xxx{"、"public class xxx{"、"public static class xxx{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchClassLeftBracketString(CSharpFile cSharpFile,SectionString bracketBlock,out SectionString result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//命名空间正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"((public|internal|protected|private|static|abstract|sealed)\s+)*class\s+\w+.*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=new SectionString(cSharpFile,match.Index,match.Value.Length);
				return true;
			}
			result=new SectionString();
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配结构体声明，并通过out参数输出结果。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">结构体包含的括号块，包含大括号</param>
		/// <param name="result">输出结果，如:"public struct xxx{"、"internal struct xxx{"、"struct xxx{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchStructLeftBracketString(CSharpFile cSharpFile,SectionString bracketBlock,out SectionString result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//结构体正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"(public|internal|protected|private)?\s+struct\s+\w+.*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=new SectionString(cSharpFile,match.Index,match.Value.Length);
				return true;
			}
			result=new SectionString();
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配接口声明，并通过out参数输出结果。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">接口包含的括号块，包含大括号</param>
		/// <param name="result">输出结果，如:"public interface xxx{"、"internal interface xxx{"、"interface xxx{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchInterfaceLeftBracketString(CSharpFile cSharpFile,SectionString bracketBlock,out SectionString result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//接口正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"(public|internal|protected|private)?\s+interface\s+\w+.*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=new SectionString(cSharpFile,match.Index,match.Value.Length);
				return true;
			}
			result=new SectionString();
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配枚举声明，并通过out参数输出结果。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">枚举包含的括号块，包含大括号</param>
		/// <param name="result">输出结果，如:"public enum xxx{"、"internal enum xxx{"、"enum xxx{","private enum xxx{","protected enum xxx{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchEnumLeftBracketString(CSharpFile cSharpFile,SectionString bracketBlock,out SectionString result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//接口正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"(public|internal|protected|private)?\s+enum\s+\w+.*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=new SectionString(cSharpFile,match.Index,match.Value.Length);
				return true;
			}
			result=new SectionString();
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配委托声明，并通过out参数输出结果。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">委托包含的括号块，包含大括号</param>
		/// <param name="result">输出结果，如:"public delegate xxx{"、"internal delegate xxx{"、"delegate xxx{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchDelegateLeftBracketString(CSharpFile cSharpFile,SectionString bracketBlock,out SectionString result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//接口正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"(public|internal|protected|private)?\s+enum\s+\w+.*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=new SectionString(cSharpFile,match.Index,match.Value.Length);
				return true;
			}
			result=new SectionString();
			return false;
		}
		

	}
}
