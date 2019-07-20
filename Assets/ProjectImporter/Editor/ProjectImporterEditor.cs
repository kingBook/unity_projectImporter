namespace UnityProjectImporter {
	using System;
    using System.Collections.Generic;
    using UnityEditor;
	using UnityEngine;

	public class ProjectImporterEditor:Editor{
	
		private static readonly string projectPath=Environment.CurrentDirectory;

		[MenuItem("ProjectImporter/import")]
		public static void import(){
			importProject("E:/kingBook/projects/unity_tags");
			//deleteProject("unity_parkinggame");
		}

		public static void importProject(string path){
			string projectName=path.Substring(path.LastIndexOf('/')+1);
			//删除指定项目的所有资源和设置,用于重复导入时清空上一次导入的资源和设置
			deleteProject(projectName);

			//创建"ProjectImporter/temp"临时文件夹,如果文件夹存在则先删除
			string projectImporterTempPath="Assets/ProjectImporter/temp";
			FileUtil2.createDirectory(projectImporterTempPath,true);

			//导入tags和Layers
			var tagsAndLayersImporter=new TagsAndLayersImporter();
			tagsAndLayersImporter.import(path,projectImporterTempPath,projectName);

			//导入Assets文件夹,并修改.cs文件解决冲突
			var assetsImporter=new AssetsImporter();
			assetsImporter.import(path,projectImporterTempPath,projectName);

			//导入Time
			var timeImporter=new TimeImporter();
			timeImporter.import(path,projectImporterTempPath,projectName);

			//导入Physics
			var physicsImporter=new PhysicsImporter();
			physicsImporter.import(path,projectImporterTempPath,projectName);

			//导入Physics2D
			var physics2DImporter=new Physics2DImporter();
			physics2DImporter.import(path,projectImporterTempPath,projectName);

			//导入Quality
			var qualityImporter=new QualityImporter();
			qualityImporter.import(path,projectImporterTempPath,projectName);
			
			//导入BuildSettings
			var buildSettingsImporter=new BuildSettingsImporter();
			buildSettingsImporter.import(path,projectImporterTempPath,projectName);

			//所有事情完成，删除"ProjectImporter/temp"临时文件夹
			AssetDatabase.DeleteAsset(projectImporterTempPath);
		}

		/// <summary>
		/// 用于重复导入指定的项目时，删除指定项目的所有资源和设置
		/// </summary>
		/// <param name="projectName">项目名称</param>
		public static void deleteProject(string projectName){
			//删除项目在BuildSettings窗口中的场景
			deleteBuildSettingsScenes(projectName);
			//删除项目设置
			deleteProjectSettings(projectName);
			//删除项目文件夹
			AssetDatabase.DeleteAsset("Assets/"+projectName);
			//刷新AssetDataBase
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// 删除指定项目在BuildSettings窗口中的的场景
		/// </summary>
		/// <param name="projectName">项目名称</param>
		private static void deleteBuildSettingsScenes(string projectName){
			var scenes=new List<EditorBuildSettingsScene>();
			scenes.AddRange(EditorBuildSettings.scenes);
			int i=scenes.Count;
			while(--i>=0){
				bool isProjectScene=scenes[i].path.IndexOf(projectName+'/')>-1;
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
		private static void deleteProjectSettings(string projectName){
			AssetDatabase.DeleteAsset("Assets/ProjectImporter/Resources/"+projectName+"_buildSettingsData.asset");
			AssetDatabase.DeleteAsset("Assets/ProjectImporter/Resources/"+projectName+"_layersData.asset");
			AssetDatabase.DeleteAsset("Assets/ProjectImporter/Resources/"+projectName+"_physics2dData.asset");
			AssetDatabase.DeleteAsset("Assets/ProjectImporter/Resources/"+projectName+"_physicsData.asset");
			AssetDatabase.DeleteAsset("Assets/ProjectImporter/Resources/"+projectName+"_qualityData.asset");
			AssetDatabase.DeleteAsset("Assets/ProjectImporter/Resources/"+projectName+"_sortingLayersData.asset");
			AssetDatabase.DeleteAsset("Assets/ProjectImporter/Resources/"+projectName+"_timeData.asset");
		}
	}
}
