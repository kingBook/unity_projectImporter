namespace UnityTools {
	using System.IO;
    using System.Text.RegularExpressions;
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

		[MenuItem("Tools/test")]
		public static void test(){

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
								copyAndObfuscateUnityProject(projectFolderPath,true);
							}else{
								obfuscateUnityProject(projectFolderPath,true);
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

			checkDragAndDropOnGUI();
		}

		private void checkDragAndDropOnGUI(){
			if(mouseOverWindow==this){//鼠标位于当前窗口
				if(Event.current.type==EventType.DragUpdated){//拖入窗口未松开鼠标
					DragAndDrop.visualMode=DragAndDropVisualMode.Generic;//改变鼠标外观
				}else if(Event.current.type==EventType.DragExited){//拖入窗口并松开鼠标
					Focus();//获取焦点，使unity置顶(在其他窗口的前面)
					if(DragAndDrop.paths!=null){
						int len=DragAndDrop.paths.Length;
						for(int i=0;i<len;i++){
							string path=DragAndDrop.paths[i];
							if(Directory.Exists(path)&&FileUtil2.isUnityProjectFolder(path)){
								if(_isCopy){
									copyAndObfuscateUnityProject(path,true);
								}else{
									obfuscateUnityProject(path,true);
								}
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// 复制并混淆一个unity项目
		/// </summary>
		/// <param name="unityProjectPath">unit项目文件夹路径</param>
		/// <param name="isCallComplete">是否执行混淆完成回调</param>
		private void copyAndObfuscateUnityProject(string unityProjectPath, bool isCallComplete){
			//跳过的文件或文件夹
			string[] ignoreCopys=new string[]{"/.git","/Library"};
			//目标文件夹名称,源文件夹名称加"_confusion"后缀
			string duplicateFolderPath=unityProjectPath+"_confusion";
			FileUtil2.replaceDirectory(unityProjectPath,duplicateFolderPath,true,ignoreCopys);
			obfuscateUnityProject(duplicateFolderPath,isCallComplete);
		}

		/// <summary>
		/// <para>混淆一个unity项目</para>
		/// </summary>
		/// <param name="unityProjectPath">unity项目文件夹路径</param>
		/// <param name="isCallComplete">是否执行混淆完成回调</param>
		private void obfuscateUnityProject(string unityProjectPath,bool isCallComplete){
			string assetsPath=unityProjectPath+"/Assets";
			CSharpObfuscator obfuscator=new CSharpObfuscator();
			try{
				obfuscator.obfuscateProject(assetsPath,()=>{
					if(isCallComplete){
						if(_showInExplorerOnComplete){
							FileUtil2.showInExplorer(unityProjectPath);
						}
					}
				});
			}catch(System.Exception err){
				//出错时隐藏进度条
				EditorUtility.ClearProgressBar();
				throw err;
			}
			
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
			CSharpObfuscator obfuscator=new CSharpObfuscator();
			obfuscator.obfuscateProject(assetsPath,()=>{
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