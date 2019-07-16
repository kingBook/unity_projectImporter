namespace UnityProjectImporter{ 
	using System;
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	public class ProjectImporterEditor:Editor{
	
		private static readonly string projectPath=Environment.CurrentDirectory;

		[MenuItem("ProjectImporter/import")]
		public static void import(){
			importProject("D:/kingBook/projects/unity_tags");
		}

		private static void importProject(string path){
			string projectName=path.Substring(path.LastIndexOf('/')+1);

			//创建子项目目录
			string childProjectPath=Application.dataPath+"/"+projectName;
			FileUtil2.createDirectory(childProjectPath);

			//创建"ProjectImporter/temp"临时文件夹
			string projectImporterTempPath="Assets/ProjectImporter/temp";
			FileUtil2.createDirectory(projectImporterTempPath);

			//导入tags和Layers
			var tagsAndLayersImporter=new TagsAndLayersImporter();
			tagsAndLayersImporter.import(path,projectImporterTempPath,projectName);

			//导入Assets文件夹
			FileUtil2.copyDirectory(path+"/Assets",childProjectPath+"/Assets",true);

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
		
			//所有事情完成，删除"ProjectImporter/temp"临时文件夹
			AssetDatabase.DeleteAsset(projectImporterTempPath);
		}
	}
}
