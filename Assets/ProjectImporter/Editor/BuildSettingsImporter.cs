namespace UnityProjectImporter{
	using UnityEngine;
	using System.Collections;
    using UnityEditor;

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

			BuildSettingsData buildSettingsData=ScriptableObject.CreateInstance<BuildSettingsData>();
			var it=copyDynamicsManager.GetIterator();
			while (it.Next(true)){
				string itName=it.name;
				if(itName=="m_Scenes"){
					int len=it.arraySize;
					buildSettingsData.scenes=new Scene[len];
					for(int i=0;i<len;i++){
						var element=it.GetArrayElementAtIndex(i);
						Scene scene=new Scene();
						scene.enabled=element.FindPropertyRelative("enabled").boolValue;
						scene.path="Assets/"+projectName+"/"+element.FindPropertyRelative("path").stringValue;
						//scene.guid=element.FindPropertyRelative("guid").stringValue;//没用到
						buildSettingsData.scenes[i]=scene;
					}
				}
			}

			AssetDatabase.CreateAsset(buildSettingsData,"Assets/ProjectImporter/Resources/"+projectName+"_buildSettingsData.asset");
			//删除复制过来的"EditorBuildSettings.asset"
			AssetDatabase.DeleteAsset(destTagFilePath);
			AssetDatabase.Refresh();
		}
	}
}
