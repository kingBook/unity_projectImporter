namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	using System.IO;

	/// <summary>
	/// CSharp混淆器
	/// <br>一个混淆器只用于一个unity项目</br>
	/// </summary>
	public class CSharpObfuscator{
		
		private System.Action _onObfuscateProjectComplete;
		
		/// <summary>
		/// 混淆一个unity项目
		/// </summary>
		/// <param name="projectAssetsPath">unity项目的Assets文件夹路径</param>
		/// <param name="onComplete">完成时的回调函数</param>
		public void ObfuscateProject(string projectAssetsPath,System.Action onComplete){
			_onObfuscateProjectComplete=onComplete;
			string[] files=Directory.GetFiles(projectAssetsPath,"*.cs",SearchOption.AllDirectories);
			int len=files.Length;
			for(int i=0;i<len;i++){
				string filePath=files[i];
				filePath=filePath.Replace("\\","/");
				
			}
		}
	}
}
