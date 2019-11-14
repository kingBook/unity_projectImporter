namespace UnityTools {
	using UnityEngine;
	using System.IO;
	using UnityEditor;
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using UnityEngine.UIElements;

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
			List<Segment> strings=readStringsWithinFile(cSharpFile);
			List<Segment> lineComments=readLineCommentsWithinFile(cSharpFile);
			//移除字符串内的行注释
			removeLineCommentsWithinStrings(lineComments,strings);
			//移除行注释内的字符串
			removeStringsWithinLineComments(strings,lineComments);
			//**到此处，strings中的所有字符串应该都是正确的
			List<Segment> blockComments=readBlockCommentsWithinFile(cSharpFile,strings);
			//
			string fileString=cSharpFile.fileString;
			//
			List<Segment> comments=new List<Segment>();
			comments.AddRange(lineComments);
			comments.AddRange(blockComments);
			//按startIndex小到大排序
			comments.Sort((Segment a,Segment b)=>{
				return a.startIndex-b.startIndex;
			});

			int i=comments.Count;
			while(--i>=0){
				Segment segmentString=comments[i];
				fileString=fileString.Remove(segmentString.startIndex,segmentString.length);
			}
			cSharpFile.fileString=fileString;
			//保存清除注释后的.cs文件到本地
			/*if(comments.Count>0){ 
				FileUtil2.writeFileString(fileString,cSharpFile.fileInfo.FullName);
			}*/
		}

		private int compareByStartIndex(Segment a,Segment b){
			return a.startIndex-b.startIndex;
		}
		
		/// <summary>
		/// 读取.cs文件中的字符串,如："abc"、@"abc"等
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <returns></returns>
		private List<Segment> readStringsWithinFile(CSharpFile cSharpFile){
			//匹配@"xxx"、"xxx"、test("xxx",@"xxx")括号里的"xxx",@"xxx"等字符串（在@符号后的双引号转义使用两个")。
			MatchCollection matches=Regexes.stringTextRegex.Matches(cSharpFile.fileString);
			int count=matches.Count;
			List<Segment> segmentStrings=new List<Segment>();
			for(int i=0;i<count;i++){
				Match match=matches[i];
				Segment[] generalStrings=getStringsWithMatch(cSharpFile,match,true);
				segmentStrings.AddRange(generalStrings);
			}
			return segmentStrings;
		}

		/// <summary>
		/// 从指定的Match中返回字符串内容
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="match">包含字符串的Match</param>
		/// <param name="isIncludeAtMark">@模式的字符串，返回时是否包含"@"标记</param>
		/// <returns></returns>
		private Segment[] getStringsWithMatch(CSharpFile cSharpFile,Match match,bool isIncludeAtMark){
			List<Segment> segmentStrings=new List<Segment>();
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
						Segment segmentString=new Segment(strStartIndex,strLength);
						segmentStrings.Add(segmentString);
						isAtPattern=false;
						isLastOver=true;
					}
				}
			}
			return segmentStrings.ToArray();
		}
		
		/// <summary>
		/// 读取.cs文件中的行注释
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <returns></returns>
		private List<Segment> readLineCommentsWithinFile(CSharpFile cSharpFile){
			List<Segment> segmentStrings=new List<Segment>();
			//匹配行注释
			MatchCollection matches=Regexes.lineCommentsRegex.Matches(cSharpFile.fileString);
			int count=matches.Count;
			for(int i=0;i<count;i++){
				Match match=matches[i];
				Segment segmentString=new Segment(match.Index,match.Length);
				segmentStrings.Add(segmentString);
			}
			return segmentStrings;

		}

		/// <summary>
		/// 读取.cs文件中的块注释
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="strings">.cs文件中的字符串列表</param>
		/// <returns></returns>
		private List<Segment> readBlockCommentsWithinFile(CSharpFile cSharpFile,List<Segment> strings){
			List<Segment> segmentStrings=new List<Segment>();
			int len=cSharpFile.fileString.Length;
			bool isBlockCommentBegan=false;
			string searchString="/*";
			int blockCommentStartIndex=0;
			int searchStartIndex=0;
			while(true){
				if(searchStartIndex>=len)break;
				int index=cSharpFile.fileString.IndexOf(searchString,searchStartIndex,StringComparison.Ordinal);
				if(index<0)break;
				searchStartIndex=index+2;
				if(isBlockCommentBegan){
					searchString="/*";
					isBlockCommentBegan=false;
					Segment segmentString=new Segment(blockCommentStartIndex,index-blockCommentStartIndex+2);
					segmentStrings.Add(segmentString);
				}else if(!indexWithinSegmentStrings(index,2,strings)){
					blockCommentStartIndex=index;
					searchString="*/";
					isBlockCommentBegan=true;
				}
			}
			return segmentStrings;
		}

		/// <summary>
		/// 移除字符串内的行注释
		/// </summary>
		/// <param name="lineComments">行注释列表</param>
		/// <param name="strings">字符串列表</param>
		private void removeLineCommentsWithinStrings(List<Segment> lineComments,List<Segment> strings){
			int i=lineComments.Count;
			while(--i>=0){
				Segment lineComment=lineComments[i];
				if(indexWithinSegmentStrings(lineComment.startIndex,strings)){
					lineComments.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// 移除行注释内的字符串
		/// </summary>
		/// <param name="strings">字符串列表</param>
		/// <param name="lineComments">行注释列表</param>
		private void removeStringsWithinLineComments(List<Segment> strings,List<Segment> lineComments){
			int i=strings.Count;
			while(--i>=0){
				Segment segmentString=strings[i];
				if(indexWithinSegmentStrings(segmentString.startIndex,lineComments)){
					strings.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// 指定的索引是否在片段字符串列表中的任意一项内
		/// </summary>
		/// <param name="index">查找的索引</param>
		/// <param name="segmentStrings">片段字符串列表</param>
		/// <returns></returns>
		private bool indexWithinSegmentStrings(int index,List<Segment> segmentStrings){
			int count=segmentStrings.Count;
			for(int i=0;i<count;i++){
				Segment segmentString=segmentStrings[i];
				int startIndex=segmentString.startIndex;
				int endIndex=startIndex+segmentString.length-1;
				if(index>=startIndex && index<=endIndex){
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 指定的索引和长度范围是否在片段字符串列表中的任意一项内
		/// </summary>
		/// <param name="index">查找的索引</param>
		/// <param name="length">长度</param>
		/// <param name="segmentStrings">片段字符串列表</param>
		/// <returns></returns>
		private bool indexWithinSegmentStrings(int index,int length,List<Segment> segmentStrings){
			int checkEndIndex=index+length-1;

			int count=segmentStrings.Count;
			for(int i=0;i<count;i++){
				Segment segmentString=segmentStrings[i];
				int startIndex=segmentString.startIndex;
				int endIndex=startIndex+segmentString.length-1;
				if(index>=startIndex && checkEndIndex<=endIndex){
					return true;
				}
			}
			return false;
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
			Segment content=new Segment(0,cSharpFile.fileString.Length);
			cSharpFile.usings=readUsings(cSharpFile,content);
			
			Segment[] bracesBlocks=readBracesBlocks(cSharpFile,content);
			List<CSharpNameSpace> namespaces;
			List<CSharpClass> classes;
			List<CSharpStruct> structs;
			List<CSharpInterface> interfaces;
			List<CSharpEnum> enums;
			List<CSharpDelegate> delegates;
			readNameSpaceSubObjects(cSharpFile,CSharpNameSpace.None,bracesBlocks,out namespaces,out classes,out structs,out interfaces,out enums,out delegates);
			
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
		/// <param name="leftBraceMatch">匹配左括号命名空间声明的Match，Match的值如："namespace unity_tags{"。</param>
		/// <param name="bracketBlock">命名空间包含的括号块，包含大括号</param>
		/// <returns></returns>
		private CSharpNameSpace createNameSpace(CSharpFile cSharpFile,CSharpNameSpace parentNameSpace,Match leftBraceMatch,Segment bracketBlock){
			CSharpNameSpace csNameSpace=new CSharpNameSpace();
			csNameSpace.parent=parentNameSpace;
			
			//命名空间名称
			Group nameWordsGroup=leftBraceMatch.Groups["nameWords"];
			csNameSpace.name=readDotPath(cSharpFile,new Segment(nameWordsGroup.Index,nameWordsGroup.Length));
			//Debug.Log(csNameSpace.getNameWordsString(cSharpFile.fileString));
			
			//命名空间内容，从命名空间声明"{"的右边开始,"}"的左边结束(就是减去两个大括号的长度)
			Segment content=new Segment(bracketBlock.startIndex+1,bracketBlock.length-2);
			csNameSpace.usings=readUsings(cSharpFile,content);
			csNameSpace.content=content;
			
			Segment[] bracesBlocks=readBracesBlocks(cSharpFile,content);
			List<CSharpNameSpace> namespaces;
			List<CSharpClass> classes;
			List<CSharpStruct> structs;
			List<CSharpInterface> interfaces;
			List<CSharpEnum> enums;
			List<CSharpDelegate> delegates;
			readNameSpaceSubObjects(cSharpFile,csNameSpace,bracesBlocks,out namespaces,out classes,out structs,out interfaces,out enums,out delegates);
			
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
		/// <param name="nameSpace">类所在的命名空间</param>
		/// <param name="leftBraceMatch">匹配左括号类声明的Match，Match的值如："public class Main{"等。</param>
		/// <param name="bracketBlock">类包含的括号块，包含大括号</param>
		/// <returns></returns>
		private CSharpClass createClass(CSharpFile cSharpFile,CSharpNameSpace nameSpace,Match leftBraceMatch,Segment bracketBlock){
			CSharpClass csClass=new CSharpClass();
			csClass.nameSpace=nameSpace;
			int index;
			csClass.name=readTypeName(cSharpFile,"class",leftBraceMatch,out index);
			csClass.extends=readTypeExtends(cSharpFile,leftBraceMatch,index,out index);
			csClass.implements=readImplements(cSharpFile,leftBraceMatch,index,out index);
			csClass.genericConstraints=readTypeGenericConstraints(cSharpFile,leftBraceMatch,index);
			return csClass;
		}

		private CSharpStruct createStruct(CSharpFile cSharpFile,CSharpNameSpace nameSpace,Match leftBraceMatch,Segment bracketBlock){
			CSharpStruct cSharpStruct=new CSharpStruct();
			cSharpStruct.nameSpace=nameSpace;
			int index;
			cSharpStruct.name=readTypeName(cSharpFile,"struct",leftBraceMatch,out index);
			//接口,合并extends和implements
			IString extends=readTypeExtends(cSharpFile,leftBraceMatch,index,out index);
			IString[] implements=readImplements(cSharpFile,leftBraceMatch,index,out index);
			if(extends!=null){
				IString[] list=new IString[implements.Length+1];
				list[0]=extends;
				if(implements.Length>0)implements.CopyTo(list,1);
				cSharpStruct.implements=list;
			}else{
				cSharpStruct.implements=implements;
			}
			cSharpStruct.genericConstraints=readTypeGenericConstraints(cSharpFile,leftBraceMatch,index);
			return cSharpStruct;
		}

		private CSharpInterface createInterface(CSharpFile cSharpFile,CSharpNameSpace nameSpace,Match leftBraceMatch,Segment bracketBlock){
			CSharpInterface cSharpInterface=new CSharpInterface();
			cSharpInterface.nameSpace=nameSpace;
			int index;
			cSharpInterface.name=readTypeName(cSharpFile,"interface",leftBraceMatch,out index);
			//接口,合并extends和implements
			IString extends=readTypeExtends(cSharpFile,leftBraceMatch,index,out index);
			IString[] implements=readImplements(cSharpFile,leftBraceMatch,index,out index);
			if(extends!=null){
				IString[] list=new IString[implements.Length+1];
				list[0]=extends;
				if(implements.Length>0)implements.CopyTo(list,1);
				cSharpInterface.implements=list;
			}else{
				cSharpInterface.implements=implements;
			}
			cSharpInterface.genericConstraints=readTypeGenericConstraints(cSharpFile,leftBraceMatch,index);
			return cSharpInterface;
		}

		private CSharpEnum createEnum(CSharpFile cSharpFile,CSharpNameSpace nameSpace,Match leftBraceMatch,Segment bracketBlock){
			CSharpEnum cSharpEnum=new CSharpEnum();
			cSharpEnum.nameSpace=nameSpace;
			int index;
			cSharpEnum.name=(Segment)readTypeName(cSharpFile,"enum",leftBraceMatch,out index);
			cSharpEnum.baseType=(Segment)readTypeExtends(cSharpFile,leftBraceMatch,index,out index);
			return cSharpEnum;
		}

		private CSharpDelegate createDelegate(CSharpFile cSharpFile,CSharpNameSpace nameSpace,Match leftBraceMatch,Segment bracketBlock){
			CSharpDelegate cSharpDelegate=new CSharpDelegate();
			//cSharpDelegate.name
			return cSharpDelegate;
		}

		private IString readTypeName(CSharpFile cSharpFile,string type,Match leftBraceMatch,out int index){
			IString result=null;
			index=leftBraceMatch.Index;
			//匹配"type xxx<...>"
			Regex typeWordAngleBracketsRegex=new Regex(type+@"\s+"+Regexes.wordAngleBracketsRegex,RegexOptions.Compiled);
			Match typeWordAngleBracketsMatch=typeWordAngleBracketsRegex.Match(cSharpFile.fileString,leftBraceMatch.Index,leftBraceMatch.Length);
			if(typeWordAngleBracketsMatch.Success){
				Group wordAngleBracketsGroup=typeWordAngleBracketsMatch.Groups["wordAngleBrackets"];
				index=wordAngleBracketsGroup.Index+wordAngleBracketsGroup.Length;
				result=readWordAngleBrackets(cSharpFile,new Segment(wordAngleBracketsGroup.Index,wordAngleBracketsGroup.Length));
			}else{
				//匹配"type xxx"
				Regex typeWordRegex=new Regex(type+@"\s+"+Regexes.wordRegex,RegexOptions.Compiled);
				Match typeWordMatch=typeWordRegex.Match(cSharpFile.fileString,leftBraceMatch.Index,leftBraceMatch.Length);
				if(typeWordMatch.Success){
					Group wordGroup=typeWordMatch.Groups["word"];
					index=wordGroup.Index+wordGroup.Length;
					result=new Segment(wordGroup.Index,wordGroup.Length);
				}else{
					Debug.LogError("错误：没找到匹配项");
				}
			}
			return result;
		}

		private IString readTypeExtends(CSharpFile cSharpFile,Match leftBraceMatch,int startIndex,out int index){
			IString result=null;
			index=startIndex;
			int searchLength=leftBraceMatch.Length-(startIndex-leftBraceMatch.Index);
			//如果有"where"关键，只搜索到"where"之前
			int whereIndex=cSharpFile.fileString.IndexOf("where",startIndex,searchLength,StringComparison.Ordinal);
			if(whereIndex>-1){
				searchLength=whereIndex-startIndex;
			}
			//匹配":xxx.xxx.xxx<...>"
			Regex colonDotPathAngleBracketsRegex=new Regex(@":\s*"+Regexes.dotPathAngleBracketsRegex,RegexOptions.Compiled);
			Match colonDotPathAngleBracketsMatch=colonDotPathAngleBracketsRegex.Match(cSharpFile.fileString,startIndex,searchLength);
			if(colonDotPathAngleBracketsMatch.Success){
				Group dotPathAngleBracketsGroup=colonDotPathAngleBracketsMatch.Groups["dotPathAngleBrackets"];
				index=dotPathAngleBracketsGroup.Index+dotPathAngleBracketsGroup.Length;
				result=readDotPathAngleBrackets(cSharpFile,new Segment(dotPathAngleBracketsGroup.Index,dotPathAngleBracketsGroup.Length));
			}else{
				//匹配":xxx<...>"
				Regex colonWordAngleBracketsRegex=new Regex(@":\s*"+Regexes.wordAngleBracketsRegex,RegexOptions.Compiled);
				Match colonWordAngleBracketsMatch=colonWordAngleBracketsRegex.Match(cSharpFile.fileString,startIndex,searchLength);
				if(colonWordAngleBracketsMatch.Success){
					Group wordAngleBracketsGroup=colonWordAngleBracketsMatch.Groups["wordAngleBrackets"];
					index=wordAngleBracketsGroup.Index+wordAngleBracketsGroup.Length;
					result=readWordAngleBrackets(cSharpFile,new Segment(wordAngleBracketsGroup.Index,wordAngleBracketsGroup.Length));
				}else{
					//匹配":xxx.xxx.xxx..."
					Regex colonDotPathRegex=new Regex(@":\s*"+Regexes.dotPathRegex,RegexOptions.Compiled);
					Match colonDoPathMatch=colonDotPathRegex.Match(cSharpFile.fileString,startIndex,searchLength);
					if(colonDoPathMatch.Success){
						Group dotPathGroup=colonDoPathMatch.Groups["dotPath"];
						index=dotPathGroup.Index+dotPathGroup.Length;
						result=readDotPath(cSharpFile,new Segment(dotPathGroup.Index,dotPathGroup.Length));
					}else{
						//匹配":xxx"
						Regex colonWordRegex=new Regex(@":\s*"+Regexes.wordRegex,RegexOptions.Compiled);
						Match colonWordMatch=colonWordRegex.Match(cSharpFile.fileString,startIndex,searchLength);
						if(colonWordMatch.Success){
							Group wordGroup=colonWordMatch.Groups["word"];
							index=wordGroup.Index+wordGroup.Length;
							result=new Segment(wordGroup.Index,wordGroup.Length);
						}
					}
				}
			}
			return result;
		}

		private IString[] readImplements(CSharpFile cSharpFile,Match leftBraceMatch,int startIndex,out int index){
			List<IString> strings=new List<IString>();
			index=startIndex;
			int searchLength=leftBraceMatch.Length-(startIndex-leftBraceMatch.Index);
			//如果有"where"关键，只搜索到"where"之前
			int whereIndex=cSharpFile.fileString.IndexOf("where",startIndex,searchLength,StringComparison.Ordinal);
			if(whereIndex>-1){
				searchLength=whereIndex-startIndex;
			}
			//匹配",xxx.xxx.xxx<...>"|",xxx<...>"|",xxx.xxx.xxx"|",xxx"
			Regex regex=new Regex(@"\s*,\s*"+Regexes.dotPathAngleBrackets_wordAngleBrackets_dotPath_wordRegex,RegexOptions.Compiled);
			Match match=regex.Match(cSharpFile.fileString,startIndex,searchLength);
			while(match.Success){
				//"xxx.xxx.xxx<...>"
				Group group=match.Groups["dotPathAngleBrackets"];
				if(group.Success){
					DotPathAngleBrackets dotPathAngleBrackets=readDotPathAngleBrackets(cSharpFile,new Segment(group.Index,group.Length));
					strings.Add(dotPathAngleBrackets);
				}else{
					//"xxx<...>"
					group=match.Groups["wordAngleBrackets"];
					if(group.Success){
						WordAngleBrackets wordAngleBrackets=readWordAngleBrackets(cSharpFile,new Segment(group.Index,group.Length));
						strings.Add(wordAngleBrackets);
					}else{
						//"xxx.xxx.xxx"
						group=match.Groups["dotPath"];
						if(group.Success){
							DotPath dotPath=readDotPath(cSharpFile,new Segment(group.Index,group.Length));
							strings.Add(dotPath);
						}else{
							//"xxx"
							group=match.Groups["word"];
							if(group.Success){
								Segment word=new Segment(group.Index,group.Length);
								strings.Add(word);
							}
						}
					}
				}
				index=group.Index+group.Length;
				match=match.NextMatch();
			}
			return strings.ToArray();
		}

		private CSharpGenericConstraint[] readTypeGenericConstraints(CSharpFile cSharpFile,Match leftBraceMatch,int startIndex){
			List<CSharpGenericConstraint> genericConstraints=new List<CSharpGenericConstraint>();
			int searchLength=leftBraceMatch.Length-(startIndex-leftBraceMatch.Index);
			Match match=Regexes.genericConstraintRegex.Match(cSharpFile.fileString,startIndex,searchLength);
			while(match.Success){
				Group tNameGroup=match.Groups["genericConstraintName"];
				Segment tName=new Segment(tNameGroup.Index,tNameGroup.Length);
				CaptureCollection wordCpatures=match.Groups["genericConstraintSplitContent"].Captures;
				int len=wordCpatures.Count;
				List<IString> words=new List<IString>();
				for(int i=0;i<len;i++){
					Capture wordCapture=wordCpatures[i];
					//匹配":xxx.xxx.xxx<...>"
					Match dotPathAngleBracketsMatch=Regexes.dotPathAngleBracketsRegex.Match(cSharpFile.fileString,wordCapture.Index,wordCapture.Length);
					if(dotPathAngleBracketsMatch.Success){
						Group dotPathAngleBracketsGroup=dotPathAngleBracketsMatch.Groups["dotPathAngleBrackets"];
						DotPathAngleBrackets dotPathAngleBrackets=readDotPathAngleBrackets(cSharpFile,new Segment(dotPathAngleBracketsGroup.Index,dotPathAngleBracketsGroup.Length));
						words.Add(dotPathAngleBrackets);
					}else{
						//匹配":xxx<...>"
						Match wordAngleBracketsMatch=Regexes.wordAngleBracketsRegex.Match(cSharpFile.fileString,wordCapture.Index,wordCapture.Length);
						if(wordAngleBracketsMatch.Success){
							Group wordAngleBracketsGroup=wordAngleBracketsMatch.Groups["wordAngleBrackets"];
							WordAngleBrackets wordAngleBrackets=readWordAngleBrackets(cSharpFile,new Segment(wordAngleBracketsGroup.Index,wordAngleBracketsGroup.Length));
							words.Add(wordAngleBrackets);
						}else{
							//匹配":xxx.xxx.xxx..."
							Match doPathMatch=Regexes.dotPathRegex.Match(cSharpFile.fileString,wordCapture.Index,wordCapture.Length);
							if(doPathMatch.Success){
								Group dotPathGroup=doPathMatch.Groups["dotPath"];
								DotPath dotPath=readDotPath(cSharpFile,new Segment(dotPathGroup.Index,dotPathGroup.Length));
								words.Add(dotPath);
							}else{
								Match newParenthesesMatch=Regexes.newParenthesesRegex.Match(cSharpFile.fileString,wordCapture.Index,wordCapture.Length);
								if(newParenthesesMatch.Success){ 
									Group newParenthesesGroup=newParenthesesMatch.Groups["newParentheses"];
									Segment newParentheses=new Segment(newParenthesesGroup.Index,newParenthesesGroup.Length);
									words.Add(newParentheses);
								}else{
									//匹配":xxx"
									Match wordMatch=Regexes.wordRegex.Match(cSharpFile.fileString,wordCapture.Index,wordCapture.Length);
									if(wordMatch.Success){
										Group wordGroup=wordMatch.Groups["word"];
										Segment word=new Segment(wordGroup.Index,wordGroup.Length);
										words.Add(word);
									}
								}
							}
						}
					}
				}
				CSharpGenericConstraint genericConstraint=new CSharpGenericConstraint(tName,words.ToArray());
				genericConstraints.Add(genericConstraint);
				match=match.NextMatch();
			}
			return genericConstraints.ToArray();
		}
		
		#region ReadUsings
		/// <summary>
		/// 读取指定内容块的所有Using(不读取内嵌套大括号的using)
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="content">内容块</param>
		/// <returns></returns>
		private IUsing[] readUsings(CSharpFile cSharpFile,Segment content){
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
				Segment usingLine=new Segment(lineMatch.Index,lineMatch.Length);
				//去除"using"、空白、";"
				string usingLineString=lineMatch.Value;
				usingLineString=usingLineString.Substring(5);
				usingLineString=whiteSemicolonRegex.Replace(usingLineString,"");
				//
				if(usingAliasRegex.IsMatch(usingLineString)){//using别名，如:"using xx=xxx;"或"using xx=xxx.xxx.xxx;"
					CSharpUsingAlias usingAlias=readUsingAlias(cSharpFile,usingLine);
					usings.Add(usingAlias);
				}else{
					//是不是静态using
					bool isStatic=usingLineString.Substring(0,6)=="static";
					if(isStatic){//静态using，如:"using static xx;"或"using static xxx.xxx.xxx;"
						CSharpUsing usingString=readStaticUsing(cSharpFile,usingLine);
						usings.Add(usingString);
					}else{//普通using，如:"using xx;"或"using xxx.xxx.xxx;"
						CSharpUsing usingString=readUsing(cSharpFile,usingLine);
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
		private CSharpUsing readUsing(CSharpFile cSharpFile,Segment usingLine){
			string usingLineString=usingLine.ToString();
			//匹配"using "。
			Match headMatch=Regex.Match(usingLineString,@"using\s",RegexOptions.Compiled);
			
			int startIndex=usingLine.startIndex+headMatch.Length;
			//长度为:减去"using "的长度，再减去";"的长度
			int length=usingLine.length-headMatch.Length-1;
			DotPath words=readDotPath(cSharpFile,new Segment(startIndex,length));
			CSharpUsing usingString=new CSharpUsing(false,words);
			
			return usingString;
		}
		
		/// <summary>
		/// 读取静态Using
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="usingLine">using行内容块，如："using static UnityEngine.Mathf;"</param>
		/// <returns></returns>
		private CSharpUsing readStaticUsing(CSharpFile cSharpFile,Segment usingLine){
			string usingLineString=usingLine.ToString();
			//匹配"using static "。
			Match headMatch=Regex.Match(usingLineString,@"using\s+static\s",RegexOptions.Compiled);
			
			int startIndex=usingLine.startIndex+headMatch.Length;
			//长度为:减去"using static "的长度，再减去";"的长度
			int length=usingLine.length-headMatch.Length-1;
			DotPath words=readDotPath(cSharpFile,new Segment(startIndex,length));
			CSharpUsing usingString=new CSharpUsing(true,words);
			
			return usingString;
		}
		
		/// <summary>
		/// 读取Using别名
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="usingLine">using行内容块，如："using static UnityEngine.Mathf;"</param>
		/// <returns></returns>
		private CSharpUsingAlias readUsingAlias(CSharpFile cSharpFile,Segment usingLine){
			string usingLineString=usingLine.ToString();
			//匹配"using xxx="
			Match headMatch=Regex.Match(usingLineString,@"using\s+\w+=",RegexOptions.Compiled);
			
			//开始索引:加上"using "的长度
			int startIndex=usingLine.startIndex+6;
			//name长度为:"using xxx="的长度-"using "长度-"="长度
			int length=headMatch.Length-6-1;
			Segment name=new Segment(startIndex,length);
			
			startIndex=usingLine.startIndex+headMatch.Length;
			//长度为:减去"using xxx= "的长度，再减去";"的长度
			length=usingLine.length-headMatch.Length-1;
			DotPath words=readDotPath(cSharpFile,new Segment(startIndex,length));
			CSharpUsingAlias usingAlias=new CSharpUsingAlias(name,words);
			return usingAlias;
		}
		#endregion

		/// <summary>
		/// 读取命名空间的子对象
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="nameSpace">父级命名空间</param>
		/// <param name="bracesBlocks">命名空间内的大括号内容块列表（包含"{}"）</param>
		/// <param name="namespaces">输出的命名空间列表</param>
		/// <param name="classes">输出的类列表</param>
		/// <param name="structs">输出的结构体列表</param>
		/// <param name="interfaces">输出的接口列表</param>
		/// <param name="enums">输出的枚举列表</param>
		/// <param name="delegates">输出的委托列表</param>
		private void readNameSpaceSubObjects(CSharpFile cSharpFile,CSharpNameSpace nameSpace,Segment[] bracesBlocks,
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
			
			int len=bracesBlocks.Length;
			for(int i=0;i<len;i++){
				Segment bracesBlock=bracesBlocks[i];
				
				Match leftBraceMatch;
				if(matchNameSpaceDeclare(cSharpFile,bracesBlock,out leftBraceMatch)){
					CSharpNameSpace cSharpNameSpace=createNameSpace(cSharpFile,nameSpace,leftBraceMatch,bracesBlock);
					namespaces.Add(cSharpNameSpace);
				}else if(matchClassDeclare(cSharpFile,bracesBlock,out leftBraceMatch)){
					CSharpClass cSharpClass=createClass(cSharpFile,nameSpace,leftBraceMatch,bracesBlock);
					classes.Add(cSharpClass);
				}else if(matchStructDeclare(cSharpFile,bracesBlock,out leftBraceMatch)){
					CSharpStruct cSharpStruct=createStruct(cSharpFile,nameSpace,leftBraceMatch,bracesBlock);
					structs.Add(cSharpStruct);
				}else if(matchInterfaceDeclare(cSharpFile,bracesBlock,out leftBraceMatch)){
					CSharpInterface cSharpInterface=createInterface(cSharpFile,nameSpace,leftBraceMatch,bracesBlock);
					interfaces.Add(cSharpInterface);
				}else if(matchEnumDeclare(cSharpFile,bracesBlock,out leftBraceMatch)){
					CSharpEnum cSharpEnum=createEnum(cSharpFile,nameSpace,leftBraceMatch,bracesBlock);
					enums.Add(cSharpEnum);
				}else if(matchDelegateDeclare(cSharpFile,bracesBlock,out leftBraceMatch)){
					CSharpDelegate cSharpDelegate=createDelegate(cSharpFile,nameSpace,leftBraceMatch,bracesBlock);
					delegates.Add(cSharpDelegate);
				}
			}
		}

		/// <summary>
		/// 根据指定的大括号块匹配命名空间声明，并通过out参数匹配的Match。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">大括号块，包含大括号</param>
		/// <param name="result">输出匹配的Match，Match的值如:"namespace unity_tags{"。</param>
		/// <returns></returns>
		private bool matchNameSpaceDeclare(CSharpFile cSharpFile,Segment bracketBlock,out Match result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//匹配命名空间的正则表达式，从"{"的右侧向左查找
			Regex regex=new Regex(@"namespace\s+(?<nameWords>(\b\w+\b)|((\b\w+\b\s*\.\s*)+\b\w+\b))\s*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=match;
				return true;
			}
			result=null;
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配类声明，并通过out参数输出匹配的Match。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">类包含的括号块，包含大括号</param>
		/// <param name="result">输出匹配的Match，Match的值如:"class xxx{"、"public class xxx{"、"public static class xxx{"、"class xxx<T>:Basexxx where T:class,new(){"、"class xxx:Basexxx <xxx>{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchClassDeclare(CSharpFile cSharpFile,Segment bracketBlock,out Match result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//命名空间正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"((public|internal|protected|private|static|abstract|sealed)\s+)*class\s+\b\w+\b(.|\n)*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=match;
				return true;
			}
			result=null;
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配结构体声明，并通过out参数输出匹配的Match。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">结构体包含的括号块，包含大括号</param>
		/// <param name="result">输出匹配的Match，Match的值:"public struct xxx{"、"internal struct xxx{"、"struct xxx{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchStructDeclare(CSharpFile cSharpFile,Segment bracketBlock,out Match result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//结构体正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"((public|internal|protected|private|readonly)\s+)*struct\s+\b\w+\b(.|\n)*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=match;
				return true;
			}
			result=null;
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配接口声明，并通过out参数输出匹配的Match。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">接口包含的括号块，包含大括号</param>
		/// <param name="result">输出匹配的Match，Match的值如:"public interface xxx{"、"internal interface xxx{"、"interface xxx{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchInterfaceDeclare(CSharpFile cSharpFile,Segment bracketBlock,out Match result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//接口正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"((public|internal|protected|private)\s+)*interface\s+\b\w+\b(.|\n)*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=match;
				return true;
			}
			result=null;
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配枚举声明，并通过out参数输出匹配的Match。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">枚举包含的括号块，包含大括号</param>
		/// <param name="result">输出匹配的Match，Match的值如:"public enum xxx{"、"internal enum xxx{"、"enum xxx{","private enum xxx{","protected enum xxx{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchEnumDeclare(CSharpFile cSharpFile,Segment bracketBlock,out Match result){
			//计算"{"向左查找的最远位置(就是上一个"{|}"出现的位置)
			Regex regexBracket=new Regex("{|}",RegexOptions.Compiled|RegexOptions.RightToLeft);
			Match bracketMatch=regexBracket.Match(cSharpFile.fileString,bracketBlock.startIndex);
			//接口正则表达式，从"{"的右侧开始查找
			Regex regex=new Regex(@"((public|internal|protected|private)\s+)*enum\s+\b\w+\b(.|\n)*{",RegexOptions.Compiled|RegexOptions.RightToLeft);
			int startIndex=bracketBlock.startIndex+1;//"{"的右边
			Match match;
			if(bracketMatch.Success){
				match=regex.Match(cSharpFile.fileString,bracketMatch.Index+1,startIndex-bracketMatch.Index);
			}else{
				match=regex.Match(cSharpFile.fileString,startIndex);
			}
			//必须(match.Index+match.Value.Length==startIndex)才是当前"{"的匹配项
			if(match.Success){
				result=match;
				return true;
			}
			result=null;
			return false;
		}
		
		/// <summary>
		/// 根据指定的大括号块匹配委托声明，并通过out参数输出匹配的Match。
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="bracketBlock">委托包含的括号块，包含大括号</param>
		/// <param name="result">输出匹配的Match，Match的值如:"public delegate xxx{"、"internal delegate xxx{"、"delegate xxx{"等。</param>
		/// <returns>是否匹配成功</returns>
		private bool matchDelegateDeclare(CSharpFile cSharpFile,Segment bracketBlock,out Match result){
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
				result=match;
				return true;
			}
			result=null;
			return false;
		}



		#region Functions
		/// <summary>
		/// 读取尖括号里的内容(不包含尖括号&lt;&gt;),如:"T,U,K"、"in T,in U"、"IName&lt;int&gt;,IGood,IFoo&lt;int,string&gt;"
		/// </summary>
		/// <param name="cSharpFile"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		private AngleBrackets readAngleBrackets(CSharpFile cSharpFile,Segment content){
			//一个尖括号里可能出现<点路径尖括号，名称尖括号，点路径，单词+空格+单词，单词>
			//匹配一个尖括号里","分隔的各个项
			Match match=Regexes.splitAngleBracketsRegex.Match(cSharpFile.fileString,content.startIndex,content.length);
			return readAngleBrackets(cSharpFile,match);
		}
		private AngleBrackets readAngleBrackets(CSharpFile cSharpFile,Match match){
			Group splitAngleBracketsContentGroup=match.Groups["splitAngleBracketsContent"];
			CaptureCollection captures=splitAngleBracketsContentGroup.Captures;
			int len=captures.Count;
			IString[] tNames=new IString[len];
			for(int i=0;i<len;i++){
				Capture capture=captures[i];
				//"xxx.xxx.xxx<...>"
				Match dotPathAngleBracketsMatch=Regexes.dotPathAngleBracketsRegex.Match(cSharpFile.fileString,capture.Index,capture.Length);
				if(dotPathAngleBracketsMatch.Success){
					tNames[i]=readDotPathAngleBrackets(cSharpFile,dotPathAngleBracketsMatch);
				}else{
					//"xxx<...>"
					Match wordAngleBracketsMatch=Regexes.wordAngleBracketsRegex.Match(cSharpFile.fileString,capture.Index,capture.Length);
					if(wordAngleBracketsMatch.Success){
						tNames[i]=readWordAngleBrackets(cSharpFile,wordAngleBracketsMatch);
					}else{
						//"xxx.xxx.xxx"
						Match dotPathMatch=Regexes.dotPathRegex.Match(cSharpFile.fileString,capture.Index,capture.Length);
						if(dotPathMatch.Success){
							tNames[i]=readDotPath(cSharpFile,dotPathMatch);
						}else{
							//"xxx xxx"
							Match word_wordMatch=Regexes.wordSpaceword.Match(cSharpFile.fileString,capture.Index,capture.Length);
							if(word_wordMatch.Success){
								CaptureCollection wordCaptures=word_wordMatch.Groups["word"].Captures;
								Segment word1=new Segment(wordCaptures[0].Index,wordCaptures[0].Length);
								Segment word2=new Segment(wordCaptures[1].Index,wordCaptures[1].Length);
								tNames[i]=new WordSpaceWord(word1,word2);
							}else{
								//"xxx"
								Match wordMatch=Regexes.wordRegex.Match(cSharpFile.fileString,capture.Index,capture.Length);
								if(wordMatch.Success){
									tNames[i]=new Segment(wordMatch.Index,wordMatch.Length);
								}else{
									Debug.LogError("错误：没有找到可匹配的项");
								}
							}
						}
					}
				}
			}
			return new AngleBrackets(match.Index,match.Length,tNames);
		}

		/// <summary>
		/// 读取以"."分隔的各个单词(不包含空白),如:"System.Text.RegularExpressions"，将得到"System","Text","RegularExpressions"
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="content">如："System.Text.RegularExpressions"</param>
		/// <returns></returns>
		private DotPath readDotPath(CSharpFile cSharpFile,Segment content){
			Match match=Regexes.dotPathRegex.Match(cSharpFile.fileString,content.startIndex,content.length);
			return readDotPath(cSharpFile,match);
		}
		private DotPath readDotPath(CSharpFile cSharpFile,Match match){
			CaptureCollection captures=match.Groups["word"].Captures;
			int len=captures.Count;
			Segment[] words=new Segment[len];
			for(int i=0;i<len;i++){
				Capture capture=captures[i];
				words[i]=new Segment(capture.Index,capture.Length);
			}
			return new DotPath(words);
		}

		/// <summary>
		/// 读取点路径+尖括号，如:"game.core.BaseApp&lt;...&gt;"
		/// </summary>
		/// <param name="cSharpFile"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		private DotPathAngleBrackets readDotPathAngleBrackets(CSharpFile cSharpFile,Segment content){
			Match match=Regexes.dotPathAngleBracketsRegex.Match(cSharpFile.fileString,content.startIndex,content.length);
			return readDotPathAngleBrackets(cSharpFile,content);
		}
		private DotPathAngleBrackets readDotPathAngleBrackets(CSharpFile cSharpFile,Match match){
			Group dotPathGroup=match.Groups["dotPath"];
			DotPath dotPath=readDotPath(cSharpFile,new Segment(dotPathGroup.Index,dotPathGroup.Length));
			Group angleBracketsGroup=match.Groups["angleBrackets"];
			AngleBrackets angleBrackets=readAngleBrackets(cSharpFile,new Segment(angleBracketsGroup.Index,angleBracketsGroup.Length));
			return new DotPathAngleBrackets(match.Index,match.Length,dotPath,angleBrackets);
		}

		/// <summary>
		/// 读取单词+尖括号，如:"BaseApp&lt;...&gt;"
		/// </summary>
		/// <param name="cSharpFile"></param>
		/// <param name="content"></param>
		/// <returns></returns>
		private WordAngleBrackets readWordAngleBrackets(CSharpFile cSharpFile,Segment content){
			Match match=Regexes.wordAngleBracketsRegex.Match(cSharpFile.fileString,content.startIndex,content.length);
			return readWordAngleBrackets(cSharpFile,match);
		}
		private WordAngleBrackets readWordAngleBrackets(CSharpFile cSharpFile,Match match){
			Group wordGroup=match.Groups["word"];
			Segment word=new Segment(wordGroup.Index,wordGroup.Length);
			
			Group angleBracketsGroup=match.Groups["angleBrackets"];
			AngleBrackets angleBrackets=readAngleBrackets(cSharpFile,new Segment(angleBracketsGroup.Index,angleBracketsGroup.Length));
			return new WordAngleBrackets(match.Index,match.Length,word,angleBrackets);
		}

		/// <summary>
		/// 读取大括号块(包含大括号,不读取子级大括号块)，"{...}"
		/// </summary>
		/// <param name="cSharpFile">CSharpFile</param>
		/// <param name="content">读取内容的位置和长度</param>
		/// <returns></returns>
		private Segment[] readBracesBlocks(CSharpFile cSharpFile,Segment content){
			List<Segment> bracketBlocks=new List<Segment>();
			Match match=Regexes.bracesRegex.Match(cSharpFile.fileString,content.startIndex,content.length);
			while(match.Success){
				Segment bracesBlock=new Segment(match.Index,match.Length);
				bracketBlocks.Add(bracesBlock);
				match=match.NextMatch();
			}
			return bracketBlocks.ToArray();
		}
		#endregion


	}
}
