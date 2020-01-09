using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace UnityTools {
	public class QualityImporter:Importer{
		/// <summary>
		/// 导入项目的QualitySettings
		/// </summary>
		/// <param name="path">需要导入QualitySettings的项目路径</param>
		/// <param name="currentProjectTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void Import(string path,string currentProjectTempPath,string projectName){
			//需要导入的QualitySettings.asset的路径
			string settingsFilePath=path+"/ProjectSettings/QualitySettings.asset";

			StreamReader streamReader=new StreamReader(settingsFilePath,Encoding.UTF8);
			YamlStream yaml=new YamlStream();
			yaml.Load(streamReader);
			streamReader.Dispose();
			streamReader.Close();

			YamlNode rootNode=yaml.Documents[0].RootNode;
			YamlMappingNode firstNode=(YamlMappingNode)rootNode["QualitySettings"];

			QualityData qualityData=ScriptableObject.CreateInstance<QualityData>();
			//
			qualityData.currentQuality=int.Parse(firstNode["m_CurrentQuality"].ToString());
			//
			YamlSequenceNode qualitySettingsNode=(YamlSequenceNode)firstNode["m_QualitySettings"];
			List<QualitySettings> qualitySettingsList=new List<QualitySettings>();
			foreach(var yamlNode in qualitySettingsNode){
				var item=(YamlMappingNode)yamlNode;
				qualitySettingsList.Add(ReadQualitySettings(item));
			}
			qualityData.qualitySettings=qualitySettingsList.ToArray();
			//
			YamlMappingNode perPlatformDefaultQualityNode=(YamlMappingNode)firstNode["m_PerPlatformDefaultQuality"];
			qualityData.perPlatformDefaultQuality=ReadPlatformDefaultQuality(perPlatformDefaultQualityNode);
			//
			AssetDatabase.CreateAsset(qualityData,ProjectImporterEditor.resourcePath+"/"+projectName+"_qualityData.asset");
			AssetDatabase.Refresh();
		}

		/// <summary>
		/// 读取各个品质级别的设置
		/// </summary>
		/// <param name="item">包含某个品质级别的设置的数据对象</param>
		/// <returns></returns>
		private QualitySettings ReadQualitySettings(YamlMappingNode item){
			var qualitySettings=new QualitySettings();
			qualitySettings.name=item["name"].ToString();
			qualitySettings.pixelLightCount=int.Parse(item["pixelLightCount"].ToString());
			qualitySettings.shadows=int.Parse(item["shadows"].ToString());
			qualitySettings.shadowResolution=int.Parse(item["shadowResolution"].ToString());
			qualitySettings.shadowProjection=int.Parse(item["shadowProjection"].ToString());
			qualitySettings.shadowCascades=int.Parse(item["shadowCascades"].ToString());
			qualitySettings.shadowDistance=float.Parse(item["shadowDistance"].ToString());
			qualitySettings.shadowNearPlaneOffset=float.Parse(item["shadowNearPlaneOffset"].ToString());
			qualitySettings.shadowCascade2Split=float.Parse(item["shadowCascade2Split"].ToString());

			var shadowCascade4Split=item["shadowCascade4Split"];
			var shadowCascade4SplitV3=new Vector3();
			shadowCascade4SplitV3.x=float.Parse(shadowCascade4Split["x"].ToString());
			shadowCascade4SplitV3.y=float.Parse(shadowCascade4Split["y"].ToString());
			shadowCascade4SplitV3.z=float.Parse(shadowCascade4Split["z"].ToString());
			qualitySettings.shadowCascade4Split=shadowCascade4SplitV3;

			qualitySettings.shadowmaskMode=int.Parse(item["shadowmaskMode"].ToString());

			//blendWeights/skinWeights,当建项目未做任何品质设置更改时是blendWeights,更改一次后是skinWeights
			bool isSkinWeights=HasKeyWithinMappingNode(item,"skinWeights");
			if(isSkinWeights){
				qualitySettings.skinWeights=int.Parse(item["skinWeights"].ToString());
			}else{
				qualitySettings.skinWeights=int.Parse(item["blendWeights"].ToString());
			}
			qualitySettings.textureQuality=int.Parse(item["textureQuality"].ToString());
			qualitySettings.anisotropicTextures=int.Parse(item["anisotropicTextures"].ToString());
			qualitySettings.antiAliasing=int.Parse(item["antiAliasing"].ToString());
			qualitySettings.softParticles=item["softParticles"].ToString()=="1";
			qualitySettings.softVegetation=item["softVegetation"].ToString()=="1";
			qualitySettings.realtimeReflectionProbes=item["realtimeReflectionProbes"].ToString()=="1";
			qualitySettings.billboardsFaceCameraPosition=item["billboardsFaceCameraPosition"].ToString()=="1";
			qualitySettings.vSyncCount=int.Parse(item["vSyncCount"].ToString());
			qualitySettings.lodBias=float.Parse(item["lodBias"].ToString());
			qualitySettings.maximumLODLevel=int.Parse(item["maximumLODLevel"].ToString());

			//skinWeights,才有
			if(isSkinWeights){
				qualitySettings.streamingMipmapsActive=item["streamingMipmapsActive"].ToString()=="1";
				qualitySettings.streamingMipmapsAddAllCameras=item["streamingMipmapsAddAllCameras"].ToString()=="1";
				qualitySettings.streamingMipmapsMemoryBudget=float.Parse(item["streamingMipmapsMemoryBudget"].ToString());
				qualitySettings.streamingMipmapsRenderersPerFrame=int.Parse(item["streamingMipmapsRenderersPerFrame"].ToString());
				qualitySettings.streamingMipmapsMaxLevelReduction=int.Parse(item["streamingMipmapsMaxLevelReduction"].ToString());
				qualitySettings.streamingMipmapsMaxFileIORequests=int.Parse(item["streamingMipmapsMaxFileIORequests"].ToString());
			}

			qualitySettings.particleRaycastBudget=int.Parse(item["particleRaycastBudget"].ToString());
			qualitySettings.asyncUploadTimeSlice=int.Parse(item["asyncUploadTimeSlice"].ToString());
			qualitySettings.asyncUploadBufferSize=int.Parse(item["asyncUploadBufferSize"].ToString());

			//skinWeights,才有
			if(isSkinWeights){
				qualitySettings.asyncUploadPersistentBuffer=item["asyncUploadPersistentBuffer"].ToString()=="1";
			}

			qualitySettings.resolutionScalingFixedDPIFactor=float.Parse(item["resolutionScalingFixedDPIFactor"].ToString());
			//排除的平台，相当于在ProjectSettings->qualitySettings选项中未勾选的平台
			YamlSequenceNode excludedTargetPlatformsNode=(YamlSequenceNode)item["excludedTargetPlatforms"];
			List<string> stringList=new List<string>();
			foreach(var platform in excludedTargetPlatformsNode){
				stringList.Add(platform.ToString());
			}
			qualitySettings.excludedTargetPlatforms=stringList.ToArray();
			return qualitySettings;
		}

		/// <summary>
		/// 读取平台默认品质级别
		/// </summary>
		/// <param name="item">包含平台默认品质级别数据对象</param>
		/// <returns></returns>
		private PlatformDefaultQuality[] ReadPlatformDefaultQuality(YamlMappingNode item){
			List<PlatformDefaultQuality> list=new List<PlatformDefaultQuality>();
			foreach(var platform in item){
				PlatformDefaultQuality platformDefaultQuality=new PlatformDefaultQuality();
				platformDefaultQuality.platform=platform.Key.ToString();
				platformDefaultQuality.qualityLevel=int.Parse(platform.Value.ToString());
				list.Add(platformDefaultQuality);
			}
			return list.ToArray();
		}

		private bool HasKeyWithinMappingNode(YamlMappingNode node,string key){
			foreach(var item in node) {
				if(item.Key.ToString()==key)return true;
			}
			return false;
		}
	}
}
