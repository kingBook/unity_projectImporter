using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public class ProjectImporter:Editor{
	
	private static readonly string projectPath=Environment.CurrentDirectory;

	[MenuItem("ProImporter/import")]
    public static void import(){
		importProject("E:/kingBook/projects/unity-tags");
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

		//导入tags和Layers
		importTagsAndLayers(path,projectImportSettingPath,projectName);
	}

	private static void importTagsAndLayers(string path,string projectImportSettingPath,string projectName){
		string sourceTagFilePath=path+"/ProjectSettings/TagManager.asset";
		string destTagFilePath=projectImportSettingPath+"/TagManager.asset";
		copyFile(sourceTagFilePath,destTagFilePath,true);

		string destTagAssetPath="Assets/"+projectName+"/projectImportSetting/TagManager.asset";
		SerializedObject copyTagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagAssetPath));

		SerializedObject myTagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

		var it=copyTagManager.GetIterator();
		while (it.NextVisible(true)){
			if (it.name=="tags"){
				int len=it.arraySize;
				for(int i=0;i<len;i++){
					SerializedProperty tagElement=it.GetArrayElementAtIndex(i);
					UnityEditorInternal.InternalEditorUtility.AddTag(tagElement.stringValue);
					//addTag(tagElement.stringValue);
				}
			}else if(it.name=="layers"){
				int len=it.arraySize;
				for(int i=8;i<len;i++){
					SerializedProperty layerElement=it.GetArrayElementAtIndex(i);
					setLayer(myTagManager,i,layerElement.stringValue);
				}
			}else if(it.name=="m_SortingLayers"){
				int len=it.arraySize;
				for(int i=1;i<len;i++){
					SerializedProperty sortingLayerElement=it.GetArrayElementAtIndex(i);
					SerializedProperty nameElement=sortingLayerElement.FindPropertyRelative("name");
					
					setSortingLayer(myTagManager,i,nameElement.stringValue);
				}
			}
		}

		FileUtil.DeleteFileOrDirectory(destTagFilePath);
		AssetDatabase.Refresh();

	}

	private static void addTag(string tag){
		if(isHasTag(tag))return;
		SerializedObject tagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
		SerializedProperty it=tagManager.FindProperty("tags");

		int count=it.arraySize;
		it.InsertArrayElementAtIndex(count);
		SerializedProperty newElement=it.GetArrayElementAtIndex(count);
		newElement.stringValue=tag;

		tagManager.ApplyModifiedProperties();
	}

	private static bool isHasTag(string tag){
		string[] tags=UnityEditorInternal.InternalEditorUtility.tags;
		int len=tags.Length;
		for (int i=0;i<len;i++){
			if(tags[i].Equals(tag)){
				return true;
			}
		}
		return false;
	}

	private static void setLayer(SerializedObject myTagManager,int index,string layer){
		if(string.IsNullOrEmpty(layer))return;

		SerializedProperty it=myTagManager.FindProperty("layers");

		int len=it.arraySize;
				
		SerializedProperty element=it.GetArrayElementAtIndex(index);
		element.stringValue="layer_"+index;
		myTagManager.ApplyModifiedProperties();
	}

	private static void setSortingLayer(SerializedObject myTagManager,int index,string layer){
		SerializedProperty it=myTagManager.FindProperty("m_SortingLayers");

		int len=it.arraySize;
		if(index>=len)it.InsertArrayElementAtIndex(len);

		SerializedProperty element=it.GetArrayElementAtIndex(index);
		SerializedProperty nameElement=element.FindPropertyRelative("name");
		nameElement.stringValue="sortLayer_"+index;

		myTagManager.ApplyModifiedProperties();
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
