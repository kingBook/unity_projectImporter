namespace UnityTools{
	using UnityEngine;
	using System.Collections;
    using UnityEditor;
	using System.Collections.Generic;
	using System;

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
			SerializedProperty it=copyDynamicsManager.GetIterator();
			while (it.Next(true)){
				string itName=it.name;
				//Debug.Log(itName);
				if(itName=="m_CurrentQuality"){
					qualityData.currentQuality=it.intValue;
				}else if(itName=="m_QualitySettings"){
					int len=it.arraySize;
					var qualitySettingsList=new QualitySettings[len];
					for(int i=0;i<len;i++){
						SerializedProperty property=it.GetArrayElementAtIndex(i);
						qualitySettingsList[i]=readQualitySettings(property);
					}
					qualityData.qualitySettings=qualitySettingsList;
				}else if(itName=="m_PerPlatformDefaultQuality"){
					qualityData.perPlatformDefaultQuality=readPlatformDefaultQuality(it);
				}
			}

			AssetDatabase.CreateAsset(qualityData,ProjectImporterEditor.resourcePath+"/"+projectName+"_qualityData.asset");
			//删除复制过来的"QualitySettings.asset"
			AssetDatabase.DeleteAsset(destTagFilePath);
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// 读取各个品质级别的设置
		/// </summary>
		/// <param name="property">包含某个品质级别的设置的SerializedProperty数据对象</param>
		/// <returns></returns>
		private QualitySettings readQualitySettings(SerializedProperty property){
			var qualitySettings=new QualitySettings();
			qualitySettings.name=property.FindPropertyRelative("name").stringValue;
			qualitySettings.pixelLightCount=property.FindPropertyRelative("pixelLightCount").intValue;
			qualitySettings.shadows=property.FindPropertyRelative("shadows").intValue;
			qualitySettings.shadowResolution=property.FindPropertyRelative("shadowResolution").intValue;
			qualitySettings.shadowProjection=property.FindPropertyRelative("shadowProjection").intValue;
			qualitySettings.shadowCascades=property.FindPropertyRelative("shadowCascades").intValue;
			qualitySettings.shadowDistance=property.FindPropertyRelative("shadowDistance").floatValue;
			qualitySettings.shadowNearPlaneOffset=property.FindPropertyRelative("shadowNearPlaneOffset").floatValue;
			qualitySettings.shadowCascade2Split=property.FindPropertyRelative("shadowCascade2Split").floatValue;
			qualitySettings.shadowCascade4Split=property.FindPropertyRelative("shadowCascade4Split").vector3Value;
			qualitySettings.shadowmaskMode=property.FindPropertyRelative("shadowmaskMode").intValue;
			qualitySettings.skinWeights=property.FindPropertyRelative("skinWeights").intValue;
			qualitySettings.textureQuality=property.FindPropertyRelative("textureQuality").intValue;
			qualitySettings.anisotropicTextures=property.FindPropertyRelative("anisotropicTextures").intValue;
			qualitySettings.antiAliasing=property.FindPropertyRelative("antiAliasing").intValue;
			qualitySettings.softParticles=property.FindPropertyRelative("softParticles").boolValue;
			qualitySettings.softVegetation=property.FindPropertyRelative("softVegetation").boolValue;
			qualitySettings.realtimeReflectionProbes=property.FindPropertyRelative("realtimeReflectionProbes").boolValue;
			qualitySettings.billboardsFaceCameraPosition=property.FindPropertyRelative("billboardsFaceCameraPosition").boolValue;
			qualitySettings.vSyncCount=property.FindPropertyRelative("vSyncCount").intValue;
			qualitySettings.lodBias=property.FindPropertyRelative("lodBias").floatValue;
			qualitySettings.maximumLODLevel=property.FindPropertyRelative("maximumLODLevel").intValue;
			qualitySettings.streamingMipmapsActive=property.FindPropertyRelative("streamingMipmapsActive").boolValue;
			qualitySettings.streamingMipmapsAddAllCameras=property.FindPropertyRelative("streamingMipmapsAddAllCameras").boolValue;
			qualitySettings.streamingMipmapsMemoryBudget=property.FindPropertyRelative("streamingMipmapsMemoryBudget").floatValue;
			qualitySettings.streamingMipmapsRenderersPerFrame=property.FindPropertyRelative("streamingMipmapsRenderersPerFrame").intValue;
			qualitySettings.streamingMipmapsMaxLevelReduction=property.FindPropertyRelative("streamingMipmapsMaxLevelReduction").intValue;
			qualitySettings.streamingMipmapsMaxFileIORequests=property.FindPropertyRelative("streamingMipmapsMaxFileIORequests").intValue;
			qualitySettings.particleRaycastBudget=property.FindPropertyRelative("particleRaycastBudget").intValue;
			qualitySettings.asyncUploadTimeSlice=property.FindPropertyRelative("asyncUploadTimeSlice").intValue;
			qualitySettings.asyncUploadBufferSize=property.FindPropertyRelative("asyncUploadBufferSize").intValue;
			qualitySettings.asyncUploadPersistentBuffer=property.FindPropertyRelative("asyncUploadPersistentBuffer").boolValue;
			qualitySettings.resolutionScalingFixedDPIFactor=property.FindPropertyRelative("resolutionScalingFixedDPIFactor").floatValue;
			//排除的平台，相当于在ProjectSettings->qualitySettings选项中未勾选的平台
			var excludedTargetPlatformsProp=property.FindPropertyRelative("excludedTargetPlatforms");
			int len=excludedTargetPlatformsProp.arraySize;
			string[] stringList=new string[len];
			for(int i=0;i<len;i++){
				string platform=excludedTargetPlatformsProp.GetArrayElementAtIndex(i).stringValue;
				stringList[i]=platform;
			}
			qualitySettings.excludedTargetPlatforms=stringList;

			return qualitySettings;
		}

		/// <summary>
		/// 读取平台默认品质级别
		/// </summary>
		/// <param name="it">包含平台默认品质级别SerializedProperty数据对象</param>
		/// <returns></returns>
		private PlatformDefaultQuality[] readPlatformDefaultQuality(SerializedProperty it){
			int len=it.arraySize;
			PlatformDefaultQuality[] list=new PlatformDefaultQuality[len];
			for(int i=0;i<len;i++){
				var platform=it.GetArrayElementAtIndex(i);
				PlatformDefaultQuality platformDefaultQuality=new PlatformDefaultQuality();
				platformDefaultQuality.platform=platform.FindPropertyRelative("first").stringValue;
				platformDefaultQuality.qualityLevel=platform.FindPropertyRelative("second").intValue;
				list[i]=platformDefaultQuality;
			}
			return list;
		}
	}
}
