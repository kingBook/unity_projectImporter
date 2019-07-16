namespace UnityProjectImporter{
	using UnityEngine;
	using System.Collections;
    using UnityEditor;

    public class QualityImporter:Importer{
		/// <summary>
		/// 导入项目的QualitySettings
		/// </summary>
		/// <param name="path">需要导入QualitySettings的项目路径</param>
		/// <param name="projectImporterTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void import(string path,string projectImporterTempPath,string projectName){
			//QualitySettings.asset 原来的位置
			string sourceTagFilePath=path+"/ProjectSettings/QualitySettings.asset";
			//QualitySettings.asset 复制过来的位置
			string destTagFilePath=projectImporterTempPath+"/QualitySettings.asset";
			//复制 QualitySettings.asset
			FileUtil2.copyFile(sourceTagFilePath,destTagFilePath,true);
			//加载并转换成SerializedObject
			string destTagAssetPath=projectImporterTempPath+"/QualitySettings.asset";
			SerializedObject copyDynamicsManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagAssetPath));

			QualityData qualityData=ScriptableObject.CreateInstance<QualityData>();
			var it=copyDynamicsManager.GetIterator();
			while (it.Next(true)){
				string itName=it.name;
				if(itName=="m_CurrentQuality"){
					qualityData.currentQuality=it.intValue;
				}else if(itName=="m_QualitySettings"){
					
				}else if(itName=="m_PerPlatformDefaultQuality"){
					
				}
			}

			AssetDatabase.CreateAsset(qualityData,"Assets/ProjectImporter/Resources/"+projectName+"_qualityData.asset");
			//删除复制过来的"QualitySettings.asset"
			AssetDatabase.DeleteAsset(destTagFilePath);
			AssetDatabase.Refresh();
		}
	}
}
