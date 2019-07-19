namespace UnityProjectImporter{ 
	using UnityEngine;
	using System.Collections;
	using UnityEditor;
	using System.IO;

	public class CSharpPostprocessor:AssetPostprocessor {

		/*private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths){
			foreach (string str in importedAssets){
				int dotIndex=str.LastIndexOf('.');
				if(dotIndex>-1){
					string extensionName=str.Substring(dotIndex);
					if(extensionName==".cs"){
						OnCSharpPostprocess(str);
					}
				}
			}
		}
		*/
		private static void OnCSharpPostprocess(string path){
			int id=path.IndexOf('/');
			path=path.Substring(id);
			path=Application.dataPath+path;
			
			//Debug.Log(path);
			/*var fs=File.OpenText(path);
			string line;
			while((line=fs.ReadLine())!=null){ 
				Debug.Log(line);
			}*/
		
		}
	}
}
