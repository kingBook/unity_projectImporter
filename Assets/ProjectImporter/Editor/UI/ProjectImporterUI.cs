namespace UnityProjectImporter{
	using UnityEngine;
	using UnityEditor;
	using System.Collections.Generic;
	using System.IO;
	using System.Xml;
	
	/// <summary>项目导入器窗口UI</summary>
	public class ProjectImporterUI:EditorWindow{
		
		[MenuItem("Tools/ProjectImporter")]
		public static void create(){
			var window=GetWindow(typeof(ProjectImporterUI),false,"ProjectImporter");
			window.Show();
		}
		
		private Vector2 _scrollPosition;
		private List<string> _projects=new List<string>{"projectA","projectB","projectC"};
		private XmlDocument _xmlDocument=null;
		
		private void Awake(){
			
		}
		
		private void OnGUI(){
			//if(_xmlDocument==null)return;
			EditorGUILayout.BeginVertical();
			{
				_scrollPosition=EditorGUILayout.BeginScrollView(_scrollPosition);
				{
					for(int i=0;i<_projects.Count;i++){
						EditorGUILayout.BeginHorizontal();
						{
							EditorGUILayout.LabelField(_projects[i],GUILayout.MinWidth(50),GUILayout.MaxWidth(200));
							EditorGUILayout.Space();
							if(GUILayout.Button("delete",GUILayout.MinWidth(50),GUILayout.MaxWidth(100))){
								onDeleteProject(i);
							}
						}
						EditorGUILayout.EndHorizontal();
						EditorGUILayout.Space();
					}
				}
				EditorGUILayout.EndScrollView();
				EditorGUILayout.Space();
				
				//add project button
				EditorGUILayout.BeginHorizontal();
				{
					EditorGUILayout.Space();
					if(GUILayout.Button("Add Project",GUILayout.MaxWidth(100))){
						onAddProject();
					}
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(10);
			}
			EditorGUILayout.EndVertical();
			
		}
		
		/// <summary>点击项目列表后的delete按钮时</summary>
		private void onDeleteProject(int index){
			_projects.RemoveAt(index);
			//ProjectImporterEditor.deleteProject("unity_framework");
		}
		
		/// <summary>点击导入项目按钮时</summary>
		private void onAddProject(){
			//打开选择文件夹对话框
			string folderPath=EditorUtility.OpenFolderPanel("Select a unity project","","");
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
			Debug.Log(XmlUtil.formatXml(_xmlDocument));
		}
		
		private void saveXml(){
			//_xmlDocument.Save();
		}
		
		private void OnDestroy(){
			saveXml();
		}
	}
}