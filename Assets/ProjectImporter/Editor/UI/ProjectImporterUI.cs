namespace UnityProjectImporter{
	using UnityEngine;
	using UnityEditor;
	using System.Collections.Generic;
	using System.IO;
	using System.Xml;
    using System.Threading.Tasks;

    /// <summary>项目导入器窗口UI</summary>
    public class ProjectImporterUI:EditorWindow{
		
		[MenuItem("Tools/ProjectImporter")]
		public static void create(){
			var window=GetWindow(typeof(ProjectImporterUI),false,"ProjectImporter");
			window.Show();
		}
		
		private static readonly string _xmlPath=System.Environment.CurrentDirectory+"/ProjectSettings/importProjects.xml";
		private Vector2 _scrollPosition;
		private XmlDocument _xmlDocument;
		private FileLoader _fileLoader;
		private bool _isLoadXmlComplete;
		
		private void Awake(){
		}
		
		private void OnEnable(){
			loadXml();
		}
		
		private void OnGUI(){
			if(!_isLoadXmlComplete)return;
			EditorGUILayout.BeginVertical();
			{
				_scrollPosition=EditorGUILayout.BeginScrollView(_scrollPosition);
				{
					//表头
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("ProjectName",GUILayout.MinWidth(100),GUILayout.MaxWidth(150));
					EditorGUILayout.LabelField("Path",GUILayout.MinWidth(100));
					EditorGUILayout.EndHorizontal();
					//
					if(_xmlDocument!=null){
						var items=_xmlDocument.FirstChild.ChildNodes;
						for(int i=0;i<items.Count;i++){
							XmlNode item=items[i];
							string projectName=item.Attributes["name"].Value;
							string projectPath=item.InnerText;
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.TextField(projectName,GUILayout.MinWidth(100),GUILayout.MaxWidth(150));
							EditorGUILayout.TextField(projectPath,GUILayout.MinWidth(100));
							if(GUILayout.Button("Explorer",GUILayout.Width(60))){
								//onDeleteProject(item,projectName);
							}
							if(GUILayout.Button("Reimport",GUILayout.Width(60))){
								//onDeleteProject(item,projectName);
							}
							if(GUILayout.Button("X",GUILayout.Width(20))){
								onDeleteProject(item,projectName);
							}
							EditorGUILayout.EndHorizontal();
							EditorGUILayout.Space();
						}
					}
				}
				EditorGUILayout.EndScrollView();
				EditorGUILayout.Space();
				//add project button
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.Space();
					if(GUILayout.Button("Add Project",GUILayout.Width(80))){
						onAddProject();
					}
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(10);
			}
			EditorGUILayout.EndVertical();
			
		}
		
		/// <summary>点击项目列表后的delete按钮时</summary>
		private void onDeleteProject(XmlNode item,string projectName){
			_xmlDocument.FirstChild.RemoveChild(item);
			saveXml();
			//ProjectImporterEditor.deleteProject(projectName);
		}
		
		/// <summary>点击导入项目按钮时</summary>
		private void onAddProject(){
			//打开选择文件夹对话框
			string folderPath=EditorUtility.OpenFolderPanel("Select a unity project","","");
			if(!string.IsNullOrEmpty(folderPath)){
				//判断是不是unity项目文件夹(是否有"Assets"和"ProjectSettings"文件夹)
				bool hasAssetsFolder=false;
				bool hasProjectSettingsFolder=false;
				string[] subFolders=Directory.GetDirectories(folderPath);
				int len=subFolders.Length;
				for(int i=0;i<len;i++){
					string subFolderPath=subFolders[i];
					int parentFolderIndex=subFolderPath.IndexOf(folderPath);
					subFolderPath=subFolderPath.Substring(parentFolderIndex+1);
					if(subFolderPath.IndexOf("Assets")>-1)hasAssetsFolder=true;
					if(subFolderPath.IndexOf("ProjectSettings")>-1)hasProjectSettingsFolder=true;
					if(hasAssetsFolder&&hasProjectSettingsFolder){
						break;
					}
				}
				bool isUnityProjectFolder=hasAssetsFolder&&hasProjectSettingsFolder;
				//是unity项目文件夹则导入，不是显示选择错误对话框
				if(isUnityProjectFolder){
					//ProjectImporterEditor.importCurrentProjectSettings();
					//ProjectImporterEditor.importProject(folderPath);
					//记录已添加的项目到xml
					addItemToXml(folderPath);
				}else{
					EditorUtility.DisplayDialog("Selection error","Invalid project path:\n"+folderPath,"OK");
				}
			}
		}
		
		/// <summary>添加一个项到xml</summary>
		private void addItemToXml(string projectFolderPath){
			string projectName=projectFolderPath.Substring(projectFolderPath.LastIndexOf('/')+1);
			if(_xmlDocument==null){
				_xmlDocument=XmlUtil.createXmlDocument(false);
				_xmlDocument.AppendChild(_xmlDocument.CreateElement("Root"));
			}
			var itemElement=_xmlDocument.CreateElement("Item");
			itemElement.SetAttribute("name",projectName);
			itemElement.InnerText=projectFolderPath;
			_xmlDocument.FirstChild.AppendChild(itemElement);
			saveXml();
		}
		
		private async void saveXml(){
			if(_xmlDocument==null)return;
			await Task.Run(()=>{
				_xmlDocument.Save(_xmlPath);
			});
		}
		
		private void loadXml(){
			if(_fileLoader==null){
				_fileLoader=new FileLoader();
			}
			_fileLoader.loadAsync(_xmlPath);
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
			saveXml();
		}
		
		/// <summary>失去焦点</summary>
		private void OnLostFocus(){
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