namespace UnityProjectImporter{
	using UnityEngine;
	using UnityEditor;

	public class TimeImporter:Importer{
		/// <summary>
		/// 导入项目的TimeSettings
		/// </summary>
		/// <param name="path">需要导入TimeSettings的项目路径</param>
		/// <param name="projectImporterTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void import(string path,string projectImporterTempPath,string projectName){
			//TimeManager.asset 原来的位置
			string sourceTagFilePath=path+"/ProjectSettings/TimeManager.asset";
			//TimeManager.asset 复制过来的位置
			string destTagFilePath=projectImporterTempPath+"/TimeManager.asset";
			//复制 TimeManager.asset
			FileUtil2.copyFile(sourceTagFilePath,destTagFilePath,true);
			//加载并转换成SerializedObject
			string destTagAssetPath=projectImporterTempPath+"/TimeManager.asset";
			SerializedObject copyDynamicsManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagAssetPath));

			TimeData timeData=ScriptableObject.CreateInstance<TimeData>();
			var it=copyDynamicsManager.GetIterator();
			while (it.Next(true)){
				string itName=it.name;
				if(itName=="Fixed Timestep"){
					timeData.fixedTimestep=it.floatValue;
				}else if(itName=="Maximum Allowed Timestep"){
					timeData.maximumAllowedTimestep=it.floatValue;
				}else if(itName=="m_TimeScale"){
					timeData.timeScale=it.floatValue;
				}else if(itName=="Maximum Particle Timestep"){
					timeData.maximumParticleTimestep=it.floatValue;
				}
			}

			AssetDatabase.CreateAsset(timeData,"Assets/ProjectImporter/Resources/"+projectName+"_timeData.asset");
			//删除复制过来的"TimeManager.asset"
			AssetDatabase.DeleteAsset(destTagFilePath);
			AssetDatabase.Refresh();
		}
	}
}
