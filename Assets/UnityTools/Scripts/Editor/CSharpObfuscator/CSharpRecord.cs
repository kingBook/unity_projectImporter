namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using System.Text.RegularExpressions;
    using System.Collections.Generic;

    /// <summary>
    /// 所有读取.cs文件类的基类
    /// </summary>
    public abstract class CSharpRecord{
		protected string _name;
		protected int _startIndex;
		protected int _length;

		/// <summary>
		/// 读取括号块不包含子级括号
		/// </summary>
		/// <param name="fileString">.cs文件字符串</param>
		/// <param name="startIndex">读取开始索引号</param>
		/// <param name="length">读取的长度</param>
		/// <returns></returns>
		protected List<BracketBlock> readBracketBlocks(string fileString,int startIndex,int length){
			List<BracketBlock> bracketBlocks=new List<BracketBlock>();
			int bracketCount=0;
			for(int i=startIndex;i<length;i++){
				char charString=fileString[i];
				
				if(charString=='{'){
					if(bracketCount>0){
						bracketCount++;
					}else{
						BracketBlock bracketBlock=new BracketBlock();
						bracketBlock.startIndex=i;
						bracketBlocks.Add(bracketBlock);
						bracketCount=1;
					}
				}else if(charString=='}'){
					bracketCount--;
					if(bracketCount==0){
						BracketBlock bracketBlock=bracketBlocks[bracketBlocks.Count-1];
						bracketBlock.length=i-bracketBlock.startIndex+1;
					}
				}
			}
			return bracketBlocks;
		}

		public string ToString(string fileString){
			return fileString.Substring(startIndex,length);
		}

		public string name { get => _name; }
		public int startIndex { get => _startIndex; }
		public int length{ get => _length; }

	}
}