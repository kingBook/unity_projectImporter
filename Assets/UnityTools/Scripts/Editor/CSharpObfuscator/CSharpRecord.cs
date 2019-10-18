namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using System.Text.RegularExpressions;
    using System.Collections.Generic;

    /// <summary>
    /// 所有读取.cs文件类的基类
    /// </summary>
    public abstract class CSharpRecord{
		protected CSharpFile _cSharpFile=null;
		protected SectionString _content;

		/// <summary>
		/// 读取括号块，"{...}"包含括号,不读取子级括号块
		/// </summary>
		/// <param name="fileString">.cs文件字符串</param>
		/// <param name="content">读取内容的位置和长度</param>
		/// <returns></returns>
		protected List<SectionString> readBracketBlocks(string fileString,SectionString content){
			List<SectionString> bracketBlocks=new List<SectionString>();
			
			int startIndex=content.startIndex;
			int end=startIndex+content.length;

			int bracketCount=0;
			int bracketBlockStartIndex=0;

			for(int i=startIndex;i<end;i++){
				char charString=fileString[i];
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
						SectionString bracketBlock=new SectionString(bracketBlockStartIndex,bracketBlockLength);
						bracketBlocks.Add(bracketBlock);
					}
				}
			}
			return bracketBlocks;
		}
		
		/// <summary>
		/// 读取以"."分隔的各个单词(包含空白),如:"System.Text.RegularExpressions"，将得到"System","Text","RegularExpressions"
		/// </summary>
		/// <param name="fileString">.cs文件字符串</param>
		/// <param name="sectionString">如："System.Text.RegularExpressions"</param>
		/// <returns></returns>
		protected SectionString[] readWords(string fileString,SectionString sectionString){
			string usingContent=sectionString.ToString(fileString);
			string[] splitStrings=usingContent.Split('.');
			int len=splitStrings.Length;
			int startIndex=sectionString.startIndex;
			SectionString[] words=new SectionString[len];
			for(int i=0;i<len;i++){
				int tempLength=splitStrings[i].Length;
				words[i]=new SectionString(startIndex,tempLength);
				//加一个单词和"."的长度
				startIndex+=tempLength+1;
			}
			return words;
		}
		
		

	}
}