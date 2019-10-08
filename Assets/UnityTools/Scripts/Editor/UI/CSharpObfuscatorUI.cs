namespace UnityTools{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
    using System.Xml;

    /// <summary>CSharp混淆器窗口UI</summary>
    public class CSharpObfuscatorUI:EditorWindow{
		private bool _isCopy=true;
		private bool _showInExplorerOnComplete=true;
		private Vector2 _scrollPosition;
		private XmlDocument _xmlDocument;
		private FileLoader _fileLoader;
		private bool _isLoadXmlComplete;

		[MenuItem("Tools/CSharpObfuscator")]
		public static void create(){
			var window=GetWindow(typeof(CSharpObfuscatorUI),false,"CSharpObfuscator");
			window.Show();
		}

		private void OnEnable(){
			loadXml();
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
				GUILayout.Button("Obfuscate all sub project");
				EditorGUILayout.Space();
				_scrollPosition=EditorGUILayout.BeginScrollView(_scrollPosition);
				{
					for(int i=0;i<50;i++)
					GUILayout.Button("Obfuscate a sub project");
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
			
			if(_showInExplorerOnComplete){
				FileUtil2.showInExplorer(projectFolderPath);
			}
			
		}

		/// <summary>
		/// 加载xml
		/// </summary>
		private void loadXml(){
			if(_fileLoader==null){
				_fileLoader=new FileLoader();
			}
			_fileLoader.loadAsync(ProjectImporterUI.xmlPath);
			_fileLoader.onComplete+=onloadXmlComplete;
		}
		private void onloadXmlComplete(byte[][] bytesList){
			_fileLoader.onComplete-=onloadXmlComplete;
			byte[] bytes=bytesList[0];
			if(bytes!=null){
				string xmlString=System.Text.Encoding.UTF8.GetString(bytes);
				_xmlDocument=XmlUtil.createXmlDocument(xmlString,false);
			}
			_isLoadXmlComplete=true;
			
		}

		private void OnDisable(){
			_isLoadXmlComplete=false;
		}
		
		/// <summary>关闭窗口</summary>
		private void OnDestroy(){
			if(_fileLoader!=null){
				_fileLoader.destroy();
				_fileLoader=null;
			}
			
		}

	}

}