namespace UnityTools{
	using UnityEngine;
	using UnityEditor;

	/// <summary>CSharp解析器窗口UI</summary>
	public class CSharpParserUI:EditorWindow{
		
		[MenuItem("Tools/CSharpParser")]
		public static void create(){
			var window=GetWindow(typeof(CSharpParserUI),false,"CSharpParser");
			window.Show();
		}

		private void OnGUI(){
			
		}



	}

}