namespace UnityTools{
	using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
	
	public class ProjectImporterEditor:Editor{
		public static readonly string currentProjectPath=Environment.CurrentDirectory.Replace('\\','/');
		public static readonly string currentProjectTempPath=currentProjectPath+"/Temp";
		public static readonly string projectImporterTempPath="Assets/UnityTools/temp";
		public static readonly string resourcePath="Assets/UnityTools/Resources";

		[MenuItem("ProjectImporter/import")]
		public static void Import(){
			ImportCurrentProjectSettings();
			//importProject("D:/kingBook/projects/unity_parkinggame");
			//deleteProject("unity_parkinggame");

			
		}

		/// <summary>
		/// 将当前的项目设置导入到"ProjectImporter/Resources"保存
		/// </summary>
		public static void ImportCurrentProjectSettings(){
			ImportProject(currentProjectPath,false,false,"default",false,false);
		}

		/// <summary>
		/// 导入一个项目的Assets文件夹和ProjectSettings
		/// </summary>
		/// <param name="path">项目的路径位置</param>
		/// <param name="isImportAssets">是否导入Assets文件夹</param>
		/// <param name="isImportBuildSettings">是否导入BuildSettings</param>
		/// <param name="projectName">导入进来的文件夹名；项目中的所有设置文件的名称前缀,null时将从path的最后截取</param>
		/// <param name="isDeleteBuildSettingsScenes">导入前是否清除由projectName指定的项目在上一次导入时在BuildSettings窗口中的场景</param>
		/// <param name="isDeleteAssets">导入前是否清除由projectName指定的项目在上一次导入时的资源文件夹</param>
		public static void ImportProject(string path,bool isImportAssets=true,bool isImportBuildSettings=true,
		string projectName=null,bool isDeleteBuildSettingsScenes=true,bool isDeleteAssets=true){
			if(projectName==null){ 
				projectName=path.Substring(path.LastIndexOf('/')+1);
			}

			//删除指定项目的所有资源和设置,用于重复导入时清空上一次导入的资源和设置
			DeleteProject(projectName,isDeleteBuildSettingsScenes,isDeleteAssets);

			//创建临时文件夹,如果文件夹存在则先删除
			FileUtil2.createDirectory(projectImporterTempPath,true);

			//导入tags和Layers
			var tagsAndLayersImporter=new TagsAndLayersImporter();
			tagsAndLayersImporter.Import(path,currentProjectTempPath,projectName);

			if(isImportAssets){
				//导入Assets文件夹,并修改.cs文件解决冲突,必须在导入tags和Layers之后执行
				var assetsImporter=new AssetsImporter();
				assetsImporter.Import(path,currentProjectTempPath,projectName);
			}

			//导入Time
			var timeImporter=new TimeImporter();
			timeImporter.Import(path,currentProjectTempPath,projectName);

			//导入Physics
			var physicsImporter=new PhysicsImporter();
			physicsImporter.Import(path,currentProjectTempPath,projectName);

			//导入Physics2D
			var physics2DImporter=new Physics2DImporter();
			physics2DImporter.Import(path,currentProjectTempPath,projectName);

			//导入Quality
			var qualityImporter=new QualityImporter();
			qualityImporter.Import(path,currentProjectTempPath,projectName);
			
			if(isImportBuildSettings){
				//导入BuildSettings
				var buildSettingsImporter=new BuildSettingsImporter();
				buildSettingsImporter.Import(path,currentProjectTempPath,projectName);
			}

			//所有事情完成，删除"ProjectImporter/temp"临时文件夹
			AssetDatabase.DeleteAsset(projectImporterTempPath);
		}

		/// <summary>
		/// 用于重复导入指定的项目时，删除指定项目的所有资源和设置。
		/// <br>项目的设置文件一定会被清除，BuildSettings窗口中的场景和导入进来的项目资源文件夹，</br>
		/// <br>则由参数决定是否删除</br>
		/// </summary>
		/// <param name="projectName">项目名称</param>
		/// <param name="isDeleteBuildSettingsScenes">是否删除项目在BuildSettings窗口中的场景列表</param>
		/// <param name="isDeleteAssets">删除导入进来的项目资源文件夹</param>
		public static void DeleteProject(string projectName,bool isDeleteBuildSettingsScenes=true,bool isDeleteAssets=true){
			if(isDeleteBuildSettingsScenes){
				//删除项目在BuildSettings窗口中的场景
				DeleteBuildSettingsScenes(projectName);
			}
			//删除项目设置
			DeleteProjectSettings(projectName);
			if(isDeleteAssets){
				//删除项目文件夹
				AssetDatabase.DeleteAsset("Assets/"+projectName);
			}
			//刷新AssetDataBase
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// 删除指定项目在BuildSettings窗口中的的场景
		/// </summary>
		/// <param name="projectName">项目名称</param>
		private static void DeleteBuildSettingsScenes(string projectName){
			var scenes=new List<EditorBuildSettingsScene>();
			scenes.AddRange(EditorBuildSettings.scenes);
			int i=scenes.Count;
			while(--i>=0){
				bool isProjectScene=scenes[i].path.IndexOf(projectName+'/', StringComparison.Ordinal)>-1;
				if(isProjectScene){
					scenes.RemoveAt(i);
				}
			}
			EditorBuildSettings.scenes=scenes.ToArray();
		}

		/// <summary>
		/// 删除指定项目在"Assets/ProjectImporter/Resources"中的设置
		/// </summary>
		/// <param name="projectName"></param>
		private static void DeleteProjectSettings(string projectName){
			AssetDatabase.DeleteAsset(resourcePath+"/"+projectName+"_buildSettingsData.asset");
			AssetDatabase.DeleteAsset(resourcePath+"/"+projectName+"_layersData.asset");
			AssetDatabase.DeleteAsset(resourcePath+"/"+projectName+"_physics2dData.asset");
			AssetDatabase.DeleteAsset(resourcePath+"/"+projectName+"_physicsData.asset");
			AssetDatabase.DeleteAsset(resourcePath+"/"+projectName+"_qualityData.asset");
			AssetDatabase.DeleteAsset(resourcePath+"/"+projectName+"_sortingLayersData.asset");
			AssetDatabase.DeleteAsset(resourcePath+"/"+projectName+"_timeData.asset");
		}


	}
}
