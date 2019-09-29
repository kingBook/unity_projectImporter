﻿namespace UnityProjectImporter {
	using System.IO;
	using System.Threading.Tasks;
	using System.Xml;
	using UnityEditor;
	using UnityEngine;

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
							string projectFolderPath=item.InnerText;
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.TextField(projectName,GUILayout.MinWidth(100),GUILayout.MaxWidth(150));
							EditorGUILayout.TextField(projectFolderPath,GUILayout.MinWidth(100));
							if(GUILayout.Button("Explorer",GUILayout.Width(60))){
								showInExplorer(item,projectFolderPath);
							}
							if(GUILayout.Button("Reimport",GUILayout.Width(60))){
								onReimportProject(item,projectFolderPath,projectName);
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

		private void showInExplorer(XmlNode item,string projectFolderPath){
			if(Directory.Exists(projectFolderPath)){
				projectFolderPath=projectFolderPath.Replace("/","\\");
				System.Diagnostics.Process.Start("explorer.exe",projectFolderPath);
			}else{
				displayReassignDialog(item);
				
			}
		}

		/// <summary>
		/// 重新导入项目
		/// </summary>
		/// <param name="item"></param>
		/// <param name="projectFolderPath"></param>
		/// <param name="projectName"></param>
		private void onReimportProject(XmlNode item,string projectFolderPath,string projectName){
			if(Directory.Exists(projectFolderPath)){
				if(isUnityProjectFolder(projectFolderPath)){
					ProjectImporterEditor.deleteProject(projectName);
					ProjectImporterEditor.importCurrentProjectSettings();
					ProjectImporterEditor.importProject(projectFolderPath);
				}
			}else{
				displayReassignDialog(item);
			}
		}

		/// <summary>
		/// 显示是否重新指定项目路径对话框
		/// </summary>
		/// <param name="item"></param>
		private void displayReassignDialog(XmlNode item){
			bool isYes=EditorUtility.DisplayDialog("Invalid project path","Invalid project path.\nWhether to reassign?","Yes","No");
			if(isYes){
				reassignProjectFolderPath(item);
			}
		}

		/// <summary>
		/// 重新指定项目文件夹
		/// </summary>
		/// <param name="item"></param>
		private void reassignProjectFolderPath(XmlNode item){
			string folderPath=openSelectUnityProjectFolderPanel();
			if(folderPath!=null){
				if(isAlreadyExists(folderPath)){
					displayAlreadyExistsDialog();
				}else{
					item.InnerText=folderPath;
					saveXml();
				}
			}
		}

		/// <summary>
		/// 显示项目已经存在对话框
		/// </summary>
		private void displayAlreadyExistsDialog(){
			EditorUtility.DisplayDialog("Project already exists","The project already exists","OK");
		}
		
		/// <summary>
		/// 点击项目列表后的delete按钮时
		/// </summary>
		/// <param name="item"></param>
		/// <param name="projectName"></param>
		private void onDeleteProject(XmlNode item,string projectName){
			_xmlDocument.FirstChild.RemoveChild(item);
			saveXml();
			//删除项目
			ProjectImporterEditor.deleteProject(projectName);
		}

		/// <summary>
		/// 打开选择文件夹对话框选择一个unity项目文件夹。
		/// <br>取消或选择非unity项目文件时都返回null</br>
		/// </summary>
		/// <returns></returns>
		private string openSelectUnityProjectFolderPanel(){
			string folderPath=EditorUtility.OpenFolderPanel("Select a unity project","","");
			if(!string.IsNullOrEmpty(folderPath)){
				if(isUnityProjectFolder(folderPath)){
					return folderPath;
				}else{
					EditorUtility.DisplayDialog("Selection error","Invalid project path:\n"+folderPath,"OK");
				}
			}
			return null;
		}
		
		/// <summary>点击导入项目按钮时</summary>
		private void onAddProject(){
			string folderPath=openSelectUnityProjectFolderPanel();
			if(folderPath!=null){
				bool isImport=true;
				string projectName=folderPath.Substring(folderPath.LastIndexOf('/')+1);
				if(isAlreadyExists(folderPath)){
					displayAlreadyExistsDialog();
					isImport=false;
				}else if(isAlreadyExistsName(projectName)){
					projectName=getRename(projectName);
				}
				if(isImport){
					//导入当前项目的项目设置
					ProjectImporterEditor.importCurrentProjectSettings();
					//导入指定项目
					ProjectImporterEditor.importProject(folderPath);
					//记录已添加的项目到xml
					addItemToXml(folderPath,projectName);
				}
			}
		}

		/// <summary>
		/// 返回重命名字符
		/// </summary>
		/// <param name="projectName"></param>
		/// <returns></returns>
		private string getRename(string projectName){
			string endNumberString=StringUtil.getEndNumberString(projectName);
			if(string.IsNullOrEmpty(endNumberString)){
				projectName+="1";
			}else{
				string head=projectName.Substring(projectName.Length-endNumberString.Length);
				projectName=head+(int.Parse(endNumberString)+1);
			}
			return projectName;
		}

		/// <summary>
		/// 判断是不是unity项目文件夹(是否有"Assets"和"ProjectSettings"文件夹)
		/// </summary>
		/// <param name="folderPath"></param>
		/// <returns></returns>
		private bool isUnityProjectFolder(string folderPath){
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
			return hasAssetsFolder&&hasProjectSettingsFolder;
		}
		
		/// <summary>
		/// 添加一个项到xml
		/// </summary>
		/// <param name="projectFolderPath"></param>
		/// <param name="projectName"></param>
		private void addItemToXml(string projectFolderPath,string projectName){
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

		/// <summary>
		/// 判定一个项目是否已经存在列表中(项目名称和路径都相同)
		/// </summary>
		/// <param name="projectFolderPath"></param>
		/// <returns></returns>
		private bool isAlreadyExists(string projectFolderPath){
			if(_xmlDocument!=null){
				XmlNodeList items=_xmlDocument.FirstChild.ChildNodes;
				int len=items.Count;
				for(int i=0;i<len;i++){
					if(items[i].InnerText==projectFolderPath){
						return true;
					}
				}
			}
			return false;
		}

		/// <summary>
		/// 判定项目名称是否已经存在列表中(只判定名称相同)
		/// </summary>
		/// <param name="projectName"></param>
		/// <returns></returns>
		private bool isAlreadyExistsName(string projectName){
			if(_xmlDocument!=null){
				XmlNodeList items=_xmlDocument.FirstChild.ChildNodes;
				int len=items.Count;
				for(int i=0;i<len;i++){
					if(items[i].Attributes["name"].Value==projectName){
						return true;
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// 保存xml到本地
		/// </summary>
		private async void saveXml(){
			if(_xmlDocument==null)return;
			await Task.Run(()=>{
				_xmlDocument.Save(_xmlPath);
			});
		}
		
		/// <summary>
		/// 加载xml
		/// </summary>
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
			_isLoadXmlComplete=false;
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