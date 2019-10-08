namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using UnityEditor;
    using System.Collections.Generic;

    public class BuildSettingsImporter:Importer{
		/// <summary>
		/// 导入项目的BuildSettings
		/// </summary>
		/// <param name="path">需要导入BuildSettings的项目路径</param>
		/// <param name="projectImporterTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void import(string path,string projectImporterTempPath,string projectName){
			//EditorBuildSettings.asset 原来的位置
			string sourceTagFilePath=path+"/ProjectSettings/EditorBuildSettings.asset";
			//EditorBuildSettings.asset 复制过来的位置
			string destTagFilePath=projectImporterTempPath+"/EditorBuildSettings.asset";
			//复制 EditorBuildSettings.asset
			FileUtil2.copyFile(sourceTagFilePath,destTagFilePath,true);
			//加载并转换成SerializedObject
			string destTagAssetPath=projectImporterTempPath+"/EditorBuildSettings.asset";
			SerializedObject copyDynamicsManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagAssetPath));
			//BuildSettings窗口场景列表
			List<EditorBuildSettingsScene> editorBuildsettingsscenes=new List<EditorBuildSettingsScene>();
			editorBuildsettingsscenes.AddRange(EditorBuildSettings.scenes);
			//创建发布设置数据
			BuildSettingsData buildSettingsData=ScriptableObject.CreateInstance<BuildSettingsData>();
			var it=copyDynamicsManager.GetIterator();
			while (it.Next(true)){
				string itName=it.name;
				if(itName=="m_Scenes"){
					int len=it.arraySize;
					buildSettingsData.scenes=new SceneData[len];
					for(int i=0;i<len;i++){
						var element=it.GetArrayElementAtIndex(i);
						//场景数据结构体
						SceneData sceneData=new SceneData();
						sceneData.enabled=element.FindPropertyRelative("enabled").boolValue;
						sceneData.path="Assets/"+projectName+"/"+element.FindPropertyRelative("path").stringValue;
						//添加到场景发布设置数据
						buildSettingsData.scenes[i]=sceneData;
						//合并到当前BuildSettings窗口列表
						var editorBuildSettingsScene=new EditorBuildSettingsScene(sceneData.path,sceneData.enabled);
						editorBuildsettingsscenes.Add(editorBuildSettingsScene);
					}
				}
			}
			//赋值到BuildSettings窗口场景列表
			EditorBuildSettings.scenes=editorBuildsettingsscenes.ToArray();
			//保存发布设置数据到本地
			AssetDatabase.CreateAsset(buildSettingsData,"Assets/ProjectImporter/Resources/"+projectName+"_buildSettingsData.asset");
			//删除复制过来的"EditorBuildSettings.asset"
			AssetDatabase.DeleteAsset(destTagFilePath);
			AssetDatabase.Refresh();
		}
	}
}
