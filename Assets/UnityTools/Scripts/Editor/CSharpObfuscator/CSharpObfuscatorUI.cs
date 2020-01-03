namespace UnityTools {
    using System;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Xml;
	using UnityEditor;
	using UnityEngine;

	/// <summary>CSharp混淆器窗口UI</summary>
	public class CSharpObfuscatorUI:EditorWindow{
		public static readonly string currentProjectPath=Environment.CurrentDirectory.Replace('\\','/');
		
		private bool m_isDuplicate=true;
		private bool m_showInExplorerOnComplete=true;
		private Vector2 m_scrollPosition;

		[MenuItem("Tools/CSharpObfuscator")]
		public static void Create(){
			var window=GetWindow(typeof(CSharpObfuscatorUI),false,"CSharpObfuscator");
			window.minSize=new Vector2(315,120);
			window.Show();
		}

		[MenuItem("Tools/CSharpObfuscatorUITest")]
		public static void CSharpObfuscatorUITest(){
			string text="#if UNITY_2017_2_OR_NEWER\n"+
						"	#if UNITY_2019\n"+
						"		unity2019\n"+
						"	#elif UNITY_2018\n"+
						"		unity2018\n"+
						"	#else\n"+
						"		unity2017.2\n"+
						"	#endif\n"+
						"#elif UNITY_5_6_OR_NEWER\n"+
						"	unity5.6\n"+
						"#elif UNITY_5_5_OR_NEWER\n"+
						"	unity5.5\n"+
						"#endif";
			var regex=new Regex(@"#if[\s\S]*#endif",RegexOptions.Compiled);
			var matches=regex.Matches(text);
			for(int i=0;i<matches.Count;i++){
				Debug.Log("i:"+matches[i].Value);
				
			}

		}


		private void OnEnable(){
			if(ProjectImporterUI.xmlDocument==null){
				ProjectImporterUI.LoadXml();
			}
		}

		private void OnGUI(){
			EditorGUILayout.BeginVertical();
			{
				EditorGUILayout.Space();
				EditorGUILayout.BeginHorizontal();
				{ 
					m_isDuplicate=GUILayout.Toggle(m_isDuplicate,"Duplicate");
					m_showInExplorerOnComplete=GUILayout.Toggle(m_showInExplorerOnComplete,"Show in explorer");
					if(GUILayout.Button("Obfuscate a project")){
						string projectFolderPath=FileUtil2.OpenSelectUnityProjectFolderPanel();
						if(!string.IsNullOrEmpty(projectFolderPath)){
							if(m_isDuplicate){
								CopyAndObfuscateUnityProject(projectFolderPath,true,true);
							}else{
								ObfuscateUnityProject(projectFolderPath,true,true);
							}
						}
					}
				}
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.Space();
				if(ProjectImporterUI.xmlDocument!=null){
					if(GUILayout.Button("Obfuscate all sub project")){
						ObfuscateAllSubProject();
					}
				}
				//表头
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label("Project Name:",GUILayout.MinWidth(100),GUILayout.MaxWidth(150));
				GUILayout.Label("Obfuscated:",GUILayout.Width(90));
				GUILayout.Space(90);
				EditorGUILayout.EndHorizontal();
				//
				m_scrollPosition=EditorGUILayout.BeginScrollView(m_scrollPosition);
				
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
							ObfuscateSubProject(projectName,true,item);
						}
						EditorGUILayout.EndHorizontal();
						GUILayout.Space(5);
					}
				}
				EditorGUILayout.EndScrollView();
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
								if(m_isDuplicate){
									CopyAndObfuscateUnityProject(path,true,true);
								}else{
									ObfuscateUnityProject(path,true,true);
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
		/// <param name="isCheckExistsAssemblyCSharpFile">是否检查存在"Assembly-CSharp.csproj"文件</param>
		/// <param name="isCallComplete">是否执行混淆完成回调</param>
		private void CopyAndObfuscateUnityProject(string unityProjectPath,bool isCheckExistsAssemblyCSharpFile,bool isCallComplete){
			//检查是否有"Assembly-CSharp.csproj"文件
			if(isCheckExistsAssemblyCSharpFile){
				if(!ExistsAssemblyCSharpFile(unityProjectPath)){
					DisplayNotExistsAssemblyCSharpDialog(unityProjectPath);
					return;
				}
			}
			//跳过的文件或文件夹
			string[] ignoreCopys=new string[]{"/.git","/Library"};
			//目标文件夹名称,源文件夹名称加"_confusion"后缀
			string duplicateFolderPath=unityProjectPath+"_confusion";
			FileUtil2.ReplaceDirectory(unityProjectPath,duplicateFolderPath,true,ignoreCopys);
			ObfuscateUnityProject(duplicateFolderPath,false,isCallComplete);
		}

		/// <summary>
		/// <para>混淆一个unity项目</para>
		/// </summary>
		/// <param name="unityProjectPath">unity项目文件夹路径</param>
		/// <param name="isCheckExistsAssemblyCSharpFile">是否检查存在"Assembly-CSharp.csproj"文件</param>
		/// <param name="isCallComplete">是否执行混淆完成回调</param>
		private void ObfuscateUnityProject(string unityProjectPath,bool isCheckExistsAssemblyCSharpFile,bool isCallComplete){
			//检查是否有"Assembly-CSharp.csproj"文件
			if(isCheckExistsAssemblyCSharpFile){
				if(!ExistsAssemblyCSharpFile(unityProjectPath)){
					DisplayNotExistsAssemblyCSharpDialog(unityProjectPath);
					return;
				}
			}
			//
			CSharpObfuscator obfuscator=new CSharpObfuscator();
			string[] defineConstants=GetDefineConstants(unityProjectPath);
			try{
				obfuscator.ObfuscateProject(unityProjectPath,defineConstants,()=>{
					if(isCallComplete){
						if(m_showInExplorerOnComplete){
							FileUtil2.ShowInExplorer(unityProjectPath);
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
		private void ObfuscateAllSubProject(){
			//检查是否有"Assembly-CSharp.csproj"文件
			if(!ExistsAssemblyCSharpFile(currentProjectPath)){
				DisplayNotExistsAssemblyCSharpDialog(currentProjectPath);
				return;
			}
			var items=ProjectImporterUI.xmlDocument.FirstChild.ChildNodes;
			int len=items.Count;
			for(int i=0;i<len;i++){
				XmlNode item=items[i];
				string projectName=item.Attributes["name"].Value;
				//string editorVersion=item.Attributes["editorVersion"].Value;
				//string obfuscated=item.Attributes["obfuscated"].Value;
				//string projectFolderPath=item.InnerText;

				ObfuscateSubProject(projectName,false,item);
			}
		}

		/// <summary>
		/// 混淆一个子项目
		/// </summary>
		/// <param name="projectName"></param>
		/// <param name="isCheckExistsAssemblyCSharpFile"></param>
		/// <param name="item"></param>
		private void ObfuscateSubProject(string projectName,bool isCheckExistsAssemblyCSharpFile,XmlNode item){
			Debug.Log(currentProjectPath);
			//检查是否有"Assembly-CSharp.csproj"文件
			if(isCheckExistsAssemblyCSharpFile){
				if(!ExistsAssemblyCSharpFile(currentProjectPath)){
					DisplayNotExistsAssemblyCSharpDialog(currentProjectPath);
					return;
				}
			}
			//
			string[] defineConstants=GetDefineConstants(currentProjectPath);
			string unityProjectPath=Application.dataPath+"/"+projectName;
			CSharpObfuscator obfuscator=new CSharpObfuscator();
			obfuscator.ObfuscateProject(unityProjectPath,defineConstants,()=>{
				OnObfuscateSubProjectComplete(item);
			});
		}

		/// <summary>
		/// 一个子项目混淆完成
		/// </summary>
		/// <param name="item"></param>
		private void OnObfuscateSubProjectComplete(XmlNode item){
			item.Attributes["obfuscated"].Value="Yes";
			ProjectImporterUI.SaveXml();
		}
		
		/// <summary>
		/// 是否存在Assembly-CSharp.csproj
		/// </summary>
		/// <param name="unityProjectPath"></param>
		/// <returns></returns>
		private bool ExistsAssemblyCSharpFile(string unityProjectPath){
			string csprojPath=unityProjectPath+"/Assembly-CSharp.csproj";
			return File.Exists(csprojPath);
		}
		
		/// <summary>
		/// 显示"未找到Assembly-CSharp.csproj"对话框
		/// </summary>
		/// <param name="unityProjectPath"></param>
		private void DisplayNotExistsAssemblyCSharpDialog(string unityProjectPath){
			string csprojPath=unityProjectPath+"/Assembly-CSharp.csproj";
			EditorUtility.DisplayDialog("Error",csprojPath+" does not exist","Cancel");
		}
		
		/// <summary>
		/// 返回项目的条件编译常量列表
		/// </summary>
		/// <param name="unityProjectPath"></param>
		/// <returns></returns>
		private string[] GetDefineConstants(string unityProjectPath){
			string csprojPath=unityProjectPath+"/Assembly-CSharp.csproj";
			XmlDocument xmlDocument=new XmlDocument();
			xmlDocument.Load(csprojPath);
			string text=xmlDocument["Project"].ChildNodes[2]["DefineConstants"].InnerText;
			string[] defineConstants=text.Split(';');
			return defineConstants;
		}

		private void OnDisable(){
		}
		
		/// <summary>关闭窗口</summary>
		private void OnDestroy(){
		}

	}

}