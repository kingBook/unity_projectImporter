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
		protected SectionString _name;
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

	}
}