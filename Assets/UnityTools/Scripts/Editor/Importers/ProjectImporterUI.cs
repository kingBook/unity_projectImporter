namespace UnityTools{
    using System.Collections.Generic;
    using System.IO;
	using System.Text.RegularExpressions;
	using System.Threading.Tasks;
	using System.Xml;
	using UnityEditor;
	using UnityEngine;

	/// <inheritdoc />
	/// <summary>项目导入器窗口UI</summary>
	public class ProjectImporterUI:EditorWindow{

		#region static member
		public static readonly string xmlPath=System.Environment.CurrentDirectory+"/ProjectSettings/importProjects.xml";
		
		private static XmlDocument m_xmlDoc;
		private static FileLoader m_fileLoader;
		private static bool m_isLoadXmlComplete;

		[MenuItem("Tools/ProjectImporter")]
		public static void Create(){
			var window=GetWindow(typeof(ProjectImporterUI),false,"ProjectImporter");
			window.minSize=new Vector2(435,120);
			window.Show();
		}

		/// <summary>
		/// 保存xml到本地
		/// </summary>
		public static async void SaveXml(){
			if(m_xmlDoc==null)return;
			await Task.Run(()=>{
				m_xmlDoc.Save(xmlPath);
			});
		}
		
		/// <summary>
		/// 加载xml
		/// </summary>
		public static void LoadXml(){
			if(m_fileLoader==null){
				m_fileLoader=new FileLoader();
			}
			m_fileLoader.LoadAsync(xmlPath);
			m_fileLoader.onCompleteAll+=OnLoadXmlComplete;
		}

		private static void OnLoadXmlComplete(byte[][] bytesList){
			m_fileLoader.onCompleteAll-=OnLoadXmlComplete;
			byte[] bytes=bytesList[0];
			if(bytes!=null){
				string xmlString=System.Text.Encoding.UTF8.GetString(bytes);
				m_xmlDoc=XmlUtil.CreateXmlDocument(xmlString,false);
			}
			m_isLoadXmlComplete=true;
			
		}

		public static XmlDocument xmlDocument{ get =>m_xmlDoc;}
		#endregion
		
		private Vector2 _scrollPosition;
		
		/*private void Awake(){
		}*/
		
		private void OnEnable(){
			LoadXml();
		}
		
		private void OnGUI(){
			if(!m_isLoadXmlComplete)return;
			EditorGUILayout.BeginVertical();
			{
				_scrollPosition=EditorGUILayout.BeginScrollView(_scrollPosition);
				{
					//表头
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.SelectableLabel("Project Name",GUILayout.MinWidth(100),GUILayout.MaxWidth(150));
					EditorGUILayout.LabelField("Version",GUILayout.Width(65));
					EditorGUILayout.LabelField("Path",GUILayout.MinWidth(100));
					GUILayout.Space(140);
					EditorGUILayout.EndHorizontal();
					//
					if(m_xmlDoc!=null){
						var items=m_xmlDoc.FirstChild.ChildNodes;
						for(int i=0;i<items.Count;i++){
							XmlNode item=items[i];
							string projectName=item.Attributes["name"].Value;
							string editorVersion=item.Attributes["editorVersion"].Value;
							string projectFolderPath=item.InnerText;
							EditorGUILayout.BeginHorizontal();
							EditorGUILayout.TextField(projectName,GUILayout.MinWidth(100),GUILayout.MaxWidth(150));
							EditorGUILayout.LabelField(editorVersion,GUILayout.Width(65));
							EditorGUILayout.TextField(projectFolderPath,GUILayout.MinWidth(100));
							if(GUILayout.Button("Explorer",GUILayout.Width(60))){
								ShowInExplorer(item,projectFolderPath);
							}
							if(GUILayout.Button("Reimport",GUILayout.Width(60))){
								OnReimportProject(item,projectFolderPath,projectName);
							}
							if(GUILayout.Button("X",GUILayout.Width(20))){
								OnDeleteProject(item,projectName);
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
						OnAddProject();
					}
				}
				EditorGUILayout.EndHorizontal();
				GUILayout.Space(10);
			}
			EditorGUILayout.EndVertical();

			CheckDragAndDropOnGUI();
		}

		private void CheckDragAndDropOnGUI(){
			if(mouseOverWindow==this){//鼠标位于当前窗口
				if(Event.current.type==EventType.DragUpdated){//拖入窗口未松开鼠标
					DragAndDrop.visualMode=DragAndDropVisualMode.Generic;//改变鼠标外观
				}else if(Event.current.type==EventType.DragExited){//拖入窗口并松开鼠标
					Focus();//获取焦点，使unity置顶(在其他窗口的前面)
					if(DragAndDrop.paths!=null){
						int len=DragAndDrop.paths.Length;
						for(int i=0;i<len;i++){
							string path=DragAndDrop.paths[i];
							if(Directory.Exists(path)&&FileUtil2.IsUnityProjectFolder(path)){
								AddProjectWithUnitProjectPath(path);
							}
						}
					}
				}
			}
		}

		private void ShowInExplorer(XmlNode item,string projectFolderPath){
			if(Directory.Exists(projectFolderPath)){
				FileUtil2.ShowInExplorer(projectFolderPath);
			}else{
				DisplayReassignDialog(item);
				
			}
		}

		/// <summary>
		/// 重新导入项目
		/// </summary>
		/// <param name="item"></param>
		/// <param name="projectFolderPath"></param>
		/// <param name="projectName"></param>
		private void OnReimportProject(XmlNode item,string projectFolderPath,string projectName){
			if(Directory.Exists(projectFolderPath)){
				if(FileUtil2.IsUnityProjectFolder(projectFolderPath)){
					ProjectImporterEditor.DeleteProject(projectName);
					ProjectImporterEditor.ImportCurrentProjectSettings();
					ProjectImporterEditor.ImportProject(projectFolderPath);
				}
			}else{
				DisplayReassignDialog(item);
			}
		}

		/// <summary>
		/// 显示是否重新指定项目路径对话框
		/// </summary>
		/// <param name="item"></param>
		private void DisplayReassignDialog(XmlNode item){
			bool isYes=EditorUtility.DisplayDialog("Invalid project path","Invalid project path.\nWhether to reassign?","Yes","No");
			if(isYes){
				ReassignProjectFolderPath(item);
			}
		}

		/// <summary>
		/// 重新指定项目文件夹
		/// </summary>
		/// <param name="item"></param>
		private void ReassignProjectFolderPath(XmlNode item){
			string folderPath=FileUtil2.OpenSelectUnityProjectFolderPanel();
			if(folderPath!=null){
				if(IsAlreadyExists(folderPath)){
					DisplayAlreadyExistsDialog();
				}else{
					item.InnerText=folderPath;
					SaveXml();
				}
			}
		}

		/// <summary>
		/// 显示项目已经存在对话框
		/// </summary>
		private void DisplayAlreadyExistsDialog(){
			EditorUtility.DisplayDialog("Project already exists","The project already exists","OK");
		}
		
		/// <summary>
		/// 点击项目列表后的delete按钮时
		/// </summary>
		/// <param name="item"></param>
		/// <param name="projectName"></param>
		private void OnDeleteProject(XmlNode item,string projectName){
			m_xmlDoc.FirstChild.RemoveChild(item);
			SaveXml();
			//删除项目
			ProjectImporterEditor.DeleteProject(projectName);
		}
		
		/// <summary>点击导入项目按钮时</summary>
		private void OnAddProject(){
			string folderPath=FileUtil2.OpenSelectUnityProjectFolderPanel();
			AddProjectWithUnitProjectPath(folderPath);
		}

		/// <summary>
		/// 从指定的unity项目路径添加项目
		/// </summary>
		/// <param name="unityProjectPath">文件夹路径</param>
		private void AddProjectWithUnitProjectPath(string unityProjectPath){
			if(unityProjectPath!=null){
				string currentProjectPath=System.Environment.CurrentDirectory.Replace("\\","/");
				bool isImport=true;
				string projectName=unityProjectPath.Substring(unityProjectPath.LastIndexOf('/')+1);
				string editorVersion=GetEditorVersion(unityProjectPath);
				if(IsAlreadyExists(unityProjectPath)){
					//当尝试导入已经存在的项目时，跳过
					DisplayAlreadyExistsDialog();
					isImport=false;
				}else if(currentProjectPath==unityProjectPath){
					//当尝试导入当前项目时，跳过
					EditorUtility.DisplayDialog("Selection error","Cannot import the project itself.","OK");
					isImport=false;
				}else if(IsAlreadyExistsName(projectName)){
					projectName=GetRename(projectName);
				}
				if(isImport){
					//导入当前项目的项目设置
					ProjectImporterEditor.ImportCurrentProjectSettings();
					//导入指定项目
					ProjectImporterEditor.ImportProject(unityProjectPath);
					//记录已添加的项目到xml
					AddItemToXml(unityProjectPath,projectName,editorVersion);
				}
			}
		}

		/// <summary>
		/// 返回项目编辑器版本号
		/// </summary>
		/// <param name="folderPath">unity项目路径</param>
		/// <returns></returns>
		private string GetEditorVersion(string folderPath){
			string projectVersionTxtPath=folderPath+"/ProjectSettings/ProjectVersion.txt";
			List<string> filelines=FileUtil2.GetFileLines(projectVersionTxtPath,false,1);
			string editorVersionLine=filelines[0];//第一行是版本号
			Regex regex=new Regex(@"m_EditorVersion:\s*",RegexOptions.Compiled);
			Match match=regex.Match(editorVersionLine);
			string versionString=editorVersionLine.Substring(match.Value.Length);
			return versionString;
		}

		/// <summary>
		/// 返回重命名字符
		/// </summary>
		/// <param name="projectName"></param>
		/// <returns></returns>
		private string GetRename(string projectName){
			string endNumberString=StringUtil.GetEndNumberString(projectName);
			if(string.IsNullOrEmpty(endNumberString)){
				projectName+="1";
			}else{
				string head=projectName.Substring(projectName.Length-endNumberString.Length);
				projectName=head+(int.Parse(endNumberString)+1);
			}
			return projectName;
		}
		
		/// <summary>
		/// 添加一个项到xml
		/// </summary>
		/// <param name="projectFolderPath">项目路径</param>
		/// <param name="projectName">项目名称</param>
		/// <param name="editorVersion">编辑器版本号</param>
		private void AddItemToXml(string projectFolderPath,string projectName,string editorVersion){
			if(m_xmlDoc==null){
				m_xmlDoc=XmlUtil.CreateXmlDocument(false);
				m_xmlDoc.AppendChild(m_xmlDoc.CreateElement("Root"));
			}
			var itemElement=m_xmlDoc.CreateElement("Item");
			itemElement.SetAttribute("name",projectName);
			itemElement.SetAttribute("editorVersion",editorVersion);
			itemElement.SetAttribute("obfuscated","No");
			itemElement.InnerText=projectFolderPath;
			m_xmlDoc.FirstChild.AppendChild(itemElement);
			SaveXml();
		}

		/// <summary>
		/// 判定一个项目是否已经存在列表中(项目名称和路径都相同)
		/// </summary>
		/// <param name="projectFolderPath"></param>
		/// <returns></returns>
		private bool IsAlreadyExists(string projectFolderPath){
			if(m_xmlDoc!=null){
				XmlNodeList items=m_xmlDoc.FirstChild.ChildNodes;
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
		private bool IsAlreadyExistsName(string projectName){
			if(m_xmlDoc!=null){
				XmlNodeList items=m_xmlDoc.FirstChild.ChildNodes;
				int len=items.Count;
				for(int i=0;i<len;i++){
					if(items[i].Attributes["name"].Value==projectName){
						return true;
					}
				}
			}
			return false;
		}
		
		private void OnDisable(){
			m_isLoadXmlComplete=false;
			SaveXml();
		}
		
		/// <summary>关闭窗口</summary>
		private void OnDestroy(){
			
		}
	}
}