namespace UnityTools{
	using UnityEngine;
	using UnityEditor;
	using System.IO;
    using System.Xml;
    using System.Threading.Tasks;

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
			window.minSize=new Vector2(360,330);
			window.Show();
		}

		private void OnEnable(){
			loadXml();
		}

		private void OnGUI(){
			if(!_isLoadXmlComplete)return;
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
				if(GUILayout.Button("Obfuscate all sub project")){
					obfuscateAllSubProject();
				}
				//表头
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Project Name:",GUILayout.MinWidth(100),GUILayout.MaxWidth(150));
				GUILayout.Label("Obfuscated:",GUILayout.Width(90));
				GUILayout.Space(90);
				EditorGUILayout.EndHorizontal();
				//
				_scrollPosition=EditorGUILayout.BeginScrollView(_scrollPosition);
				if(_xmlDocument!=null){
					var items=_xmlDocument.FirstChild.ChildNodes;
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
			if(_showInExplorerOnComplete){
				FileUtil2.showInExplorer(projectFolderPath);
			}
		}

		/// <summary>
		/// 混淆所有子项目
		/// </summary>
		private void obfuscateAllSubProject(){
			if(_xmlDocument==null)return;
			var items=_xmlDocument.FirstChild.ChildNodes;
			for(int i=0;i<items.Count;i++){
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
			//onObfuscateSubProjectComplete(item);
		}

		/// <summary>
		/// 一个子项目混淆完成
		/// </summary>
		/// <param name="item"></param>
		private void onObfuscateSubProjectComplete(XmlNode item){
			item.Attributes["obfuscated"].Value="Yes";
			saveXml();
		}

		/// <summary>
		/// 保存xml到本地
		/// </summary>
		private async void saveXml(){
			if(_xmlDocument==null)return;
			await Task.Run(()=>{
				_xmlDocument.Save(ProjectImporterUI.xmlPath);
			});
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