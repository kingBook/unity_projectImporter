using UnityEngine;
using System.Collections;
using UnityTools;
/// <summary>
/// 点路径+尖括号，如："game.foo.BaseApp&lt;...&gt;"
/// </summary>
public struct DotPathAngleBrackets:IString{
	
	public int startIndex;
	public int length;
	
	public DotPath dotPath;
	
	public AngleBrackets angleBrackets;

	public DotPathAngleBrackets(int startIndex,int length,DotPath dotPath,AngleBrackets angleBrackets){
		this.startIndex=startIndex;
		this.length=length;
		this.dotPath=dotPath;
		this.angleBrackets=angleBrackets;
	}

	public string ToString(string fileString){
		return $"{dotPath.ToString(fileString)}{angleBrackets.ToString(fileString)}";
	}
}
