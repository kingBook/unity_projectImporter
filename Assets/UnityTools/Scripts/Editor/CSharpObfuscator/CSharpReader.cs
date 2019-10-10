namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// 所有读取.cs文件类的基类
	/// </summary>
	public class CSharpReader{
		
		/// <summary>
		/// 从行字符数组读取命名空间
		/// </summary>
		/// <param name="lines">行字符数组</param>
		/// <param name="startLine">读取开始的行号，从0开始</param>
		/// <param name="startColumn">读取开始行的列号，从0开始</param>
		/// <param name="endLine">读取结尾的行号，从0开始</param>
		/// <param name="endColumn">读取结尾行的列号，从0开始。<br>-1：表示读取所有列</br></param>
		/// <returns></returns>
		protected CSharpNameSpace[] readNameSpaces(string[] lines,int startLine,int startColumn,int endLine,int endColumn=-1){
			return null;
		}
	}
}