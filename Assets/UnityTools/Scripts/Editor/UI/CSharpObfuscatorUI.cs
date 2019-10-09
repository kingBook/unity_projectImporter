namespace UnityTools {
	using System.Xml;
	using UnityEditor;
	using UnityEngine;

	/// <summary>CSharp混淆器窗口UI</summary>
	public class CSharpObfuscatorUI:EditorWindow{

		private bool _isCopy=true;
		private bool _showInExplorerOnComplete=true;
		private Vector2 _scrollPosition;

		[MenuItem("Tools/CSharpObfuscator")]
		public static void create(){
			var window=GetWindow(typeof(CSharpObfuscatorUI),false,"CSharpObfuscator");
			window.minSize=new Vector2(315,120);
			window.Show();
		}

		private void OnEnable(){
			if(ProjectImporterUI.xmlDocument==null){
				ProjectImporterUI.loadXml();
			}
		}

		private void OnGUI(){
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				{ 
					_isCopy=GUILayout.Toggle(_isCopy,"Is copy");
					_showInExplorerOnComplete=GUILayout.Toggle(_showInExplorerOnComplete,"Show in explorer");
					if(GUILayout.Button("Obfuscate a project")){
						string projectFolderPath=FileUtil2.openSelectUnityProjectFolderPanel();
						if(!string.IsNullOrEmpty(projectFolderPath)){
							if(_isCopy){
								string duplicateFolderPath=projectFolderPath+"_confusion";
								FileUtil2.replaceDirectory(projectFolderPath,duplicateFolderPath);
								obfuscateUnityProject(duplicateFolderPath);
							}else{
								obfuscateUnityProject(projectFolderPath);
							}
						}
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				if(ProjectImporterUI.xmlDocument!=null){
					if(GUILayout.Button("Obfuscate all sub project")){
						obfuscateAllSubProject();
					}
				}
				//表头
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Project Name:",GUILayout.MinWidth(100),GUILayout.MaxWidth(150));
				GUILayout.Label("Obfuscated:",GUILayout.Width(90));
				GUILayout.Space(90);
				EditorGUILayout.EndHorizontal();
				//
				_scrollPosition=EditorGUILayout.BeginScrollView(_scrollPosition);
				
				if(ProjectImporterUI.xmlDocument!=null){
					var items=ProjectImporterUI.xmlDocument.FirstChild.ChildNodes;
					for(int i=0;i<items.Count;i++){
						XmlNode item=items[i];
						string projectName=item.Attributes["name"].Value;
						//string editorVersion=item.Attributes["editorVersion"].Value;
						string obfuscated=item.Attributes["obfuscated"].Value;
						//string projectFolderPath=item.InnerText;

						EditorGUILayout.BeginHorizontal();
						GUILayout.Label(projectName,GUILayout.MinWidth(100),GUILayout.MaxWidth(150));
						GUILayout.Label(obfuscated,GUILayout.Width(90));
						if(GUILayout.Button("Obfuscate",GUILayout.Width(90))){
							obfuscateSubProject(projectName,item);
						}
						EditorGUILayout.EndHorizontal();
						GUILayout.Space(5);
					}
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.EndVertical();
		}

		/// <summary>
		/// 混淆一个unity项目
		/// </summary>
		/// <param name="projectFolderPath"></param>
		private void obfuscateUnityProject(string projectFolderPath){
			string assetsPath=projectFolderPath+"/Assets";
			CSharpObfuscator obfuscator=new CSharpObfuscator();
			obfuscator.ObfuscateProject(assetsPath,()=>{
				if(_showInExplorerOnComplete){
					FileUtil2.showInExplorer(projectFolderPath);
				}
			});
		}

		/// <summary>
		/// 混淆所有子项目
		/// </summary>
		private void obfuscateAllSubProject(){
			var items=ProjectImporterUI.xmlDocument.FirstChild.ChildNodes;
			int len=items.Count;
			for(int i=0;i<len;i++){
				XmlNode item=items[i];
				string projectName=item.Attributes["name"].Value;
				//string editorVersion=item.Attributes["editorVersion"].Value;
				//string obfuscated=item.Attributes["obfuscated"].Value;
				//string projectFolderPath=item.InnerText;

				obfuscateSubProject(projectName,item);
			}
		}

		/// <summary>
		/// 混淆一个子项目
		/// </summary>
		/// <param name="projectName"></param>
		/// <param name="item"></param>
		private void obfuscateSubProject(string projectName,XmlNode item){
			string assetsPath=Application.dataPath+"/"+projectName+"/Assets";
			Debug.Log(assetsPath);
			CSharpObfuscator obfuscator=new CSharpObfuscator();
			obfuscator.ObfuscateProject(assetsPath,()=>{
				onObfuscateSubProjectComplete(item);
			});
		}

		/// <summary>
		/// 一个子项目混淆完成
		/// </summary>
		/// <param name="item"></param>
		private void onObfuscateSubProjectComplete(XmlNode item){
			item.Attributes["obfuscated"].Value="Yes";
			ProjectImporterUI.saveXml();
		}

		private void OnDisable(){
		}
		
		/// <summary>关闭窗口</summary>
		private void OnDestroy(){
		}

	}

}