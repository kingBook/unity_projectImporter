using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ProjectImporterEditor:Editor{
	
	private static readonly string projectPath=Environment.CurrentDirectory;

	[MenuItem("ProjectImporter/import")]
    public static void import(){
		importProject("D:/kingBook/projects/unity_a");
	}

	private static void importProject(string path){
		string projectName=path.Substring(path.LastIndexOf('/')+1);

		//创建子项目目录
		string childProjectPath=Application.dataPath+"/"+projectName;
		FileUtil2.createDirectory(childProjectPath);

		//在子项目目录创建projectImportSetting
		string projectImportSettingPath=childProjectPath+"/projectImportSetting";
		FileUtil2.createDirectory(projectImportSettingPath);

		//导入tags和Layers
		var tagsAndLayersImporter=new TagsAndLayersImporter();
		tagsAndLayersImporter.import(path,projectImportSettingPath,projectName);

		//导入Assets文件夹
		FileUtil2.copyDirectory(path+"/Assets",childProjectPath+"/Assets",true);
		

	}

	


	

}
