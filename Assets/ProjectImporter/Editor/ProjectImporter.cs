using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ProjectImporter:Editor{
	
	private static readonly string projectPath=Environment.CurrentDirectory;

	[MenuItem("ProImporter/import")]
    public static void import(){
		importProject("D:/kingBook/projects/unity_a");
	}

	private static void importProject(string path){
		string projectName=path.Substring(path.LastIndexOf('/')+1);

		//创建子项目目录
		string childProjectPath=Application.dataPath+"/"+projectName;
		//-如果子项目目录存在则先删除
		if(Directory.Exists(childProjectPath))FileUtil.DeleteFileOrDirectory(childProjectPath);
		Directory.CreateDirectory(childProjectPath);

		//在子项目目录创建projectImportSetting
		string projectImportSettingPath=childProjectPath+"/projectImportSetting";
		Directory.CreateDirectory(projectImportSettingPath);

		//导入tags
		importTagManager(path,projectImportSettingPath,projectName);
	}

	private static void importTagManager(string path,string projectImportSettingPath,string projectName){
		string sourceTagFilePath=path+"/ProjectSettings/TagManager.asset";
		string destTagFilePath=projectImportSettingPath+"/TagManager.asset";
		copyFile(sourceTagFilePath,destTagFilePath,true);

		string destTagAssetPath="Assets/"+projectName+"/projectImportSetting/TagManager.asset";
		SerializedObject tagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagAssetPath));

		var it=tagManager.GetIterator();
		while (it.NextVisible(true)){
			if (it.name=="tags"){
				int len=it.arraySize;
				for(int i=0;i<len;i++){
					SerializedProperty tagElement=it.GetArrayElementAtIndex(i);
					addTag(tagElement.stringValue);
				}
			}
		}

		FileUtil.DeleteFileOrDirectory(destTagFilePath);
		AssetDatabase.Refresh();

	}

	private static void addTag(string tag){
		if(isHasTag(tag))return;
		SerializedObject tagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty it=tagManager.GetIterator();
		while (it.NextVisible(true)){
			if(it.name=="tags"){
				int count=it.arraySize;
				it.InsertArrayElementAtIndex(count);
				SerializedProperty newElement = it.GetArrayElementAtIndex(count);
				newElement.stringValue=tag;
				tagManager.ApplyModifiedProperties();
			}
		}
	}

	private static bool isHasTag(string tag){
		for (int i=0;i<UnityEditorInternal.InternalEditorUtility.tags.Length;i++){
			if(UnityEditorInternal.InternalEditorUtility.tags[i].Equals(tag)){
				return true;
			}
		}
		return false;
	}

	private static void copyFile(string source,string dest,bool isRefreshAsset=false){
		if(File.Exists(dest))FileUtil.ReplaceFile(source,dest);
		else FileUtil.CopyFileOrDirectory(source,dest);
		if(isRefreshAsset)AssetDatabase.Refresh();
	}

	private static void copyFileOrDirectory(string source,string dest,bool isRefreshAsset=false){
		FileUtil.CopyFileOrDirectory(source,dest);
		if(isRefreshAsset)AssetDatabase.Refresh();
	}

}
