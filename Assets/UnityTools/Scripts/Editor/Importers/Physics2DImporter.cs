using System.Text;
namespace UnityTools{
	using UnityEngine;
	using UnityEditor;
    using System.Collections.Generic;
    using System.IO;
    using YamlDotNet.RepresentationModel;
    using System;

    public class Physics2DImporter:Importer{
		/// <summary>
		/// 导入项目的Physics2DSettings
		/// </summary>
		/// <param name="path">需要导入Physics2DSettings的项目路径</param>
		/// <param name="currentProjectTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void import(string path,string currentProjectTempPath,string projectName){
			//需要导入的Physics2DSettings.asset的路径
			string settingsFilePath=path+"/ProjectSettings/Physics2DSettings.asset";

			StreamReader streamReader=new StreamReader(settingsFilePath,Encoding.UTF8);
			YamlStream yaml=new YamlStream();
			yaml.Load(streamReader);
			streamReader.Dispose();
			streamReader.Close();

			YamlNode rootNode=yaml.Documents[0].RootNode;
			YamlMappingNode firstNode=(YamlMappingNode)rootNode["Physics2DSettings"];
			
			Physics2dData physics2dData=ScriptableObject.CreateInstance<Physics2dData>();
			foreach(var item in firstNode){
				var keyNode=(YamlScalarNode)item.Key;
				var valueNode=item.Value;
				if(keyNode.Value=="m_Gravity"){
					Vector2 v2=new Vector2();
					v2.x=float.Parse(valueNode["x"].ToString());
					v2.y=float.Parse(valueNode["y"].ToString());
					physics2dData.gravity=v2;
				}else if(keyNode.Value=="m_DefaultMaterial"){
					//获取默认物理材质
					int fileId=int.Parse(valueNode["fileID"].ToString());
					string defaultPhysicsMaterialPath=AssetDatabase.GetAssetPath(fileId);
					physics2dData.defaultMaterial=AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(defaultPhysicsMaterialPath);//当没有设置时会自动为None
				}else if(keyNode.Value=="m_VelocityIterations"){
					physics2dData.velocityIterations=int.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_PositionIterations"){
					physics2dData.positionIterations=int.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_VelocityThreshold"){
					physics2dData.velocityThreshold=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_MaxLinearCorrection"){
					physics2dData.maxLinearCorrection=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_MaxAngularCorrection"){
					physics2dData.maxAngularCorrection=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_MaxTranslationSpeed"){
					physics2dData.maxTranslationSpeed=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_MaxRotationSpeed"){
					physics2dData.maxRotationSpeed=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_BaumgarteScale"){
					physics2dData.baumgarteScale=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_BaumgarteTimeOfImpactScale"){
					physics2dData.baumgarteTimeOfImpactScale=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_TimeToSleep"){
					physics2dData.timeToSleep=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_LinearSleepTolerance"){
					physics2dData.linearSleepTolerance=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_AngularSleepTolerance"){
					physics2dData.angularSleepTolerance=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_DefaultContactOffset"){
					physics2dData.defaultContactOffset=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_JobOptions"){
					JobOptions jobOptions=new JobOptions();
					jobOptions.useMultithreading=valueNode["useMultithreading"].ToString()=="1";
					jobOptions.useConsistencySorting=valueNode["useConsistencySorting"].ToString()=="1";
					jobOptions.interpolationPosesPerJob=int.Parse(valueNode["m_InterpolationPosesPerJob"].ToString());
					jobOptions.newContactsPerJob=int.Parse(valueNode["m_NewContactsPerJob"].ToString());
					jobOptions.collideContactsPerJob=int.Parse(valueNode["m_CollideContactsPerJob"].ToString());
					jobOptions.clearFlagsPerJob=int.Parse(valueNode["m_ClearFlagsPerJob"].ToString());
					jobOptions.clearBodyForcesPerJob=int.Parse(valueNode["m_ClearBodyForcesPerJob"].ToString());
					jobOptions.syncDiscreteFixturesPerJob=int.Parse(valueNode["m_SyncDiscreteFixturesPerJob"].ToString());
					jobOptions.syncContinuousFixturesPerJob=int.Parse(valueNode["m_SyncContinuousFixturesPerJob"].ToString());
					jobOptions.findNearestContactsPerJob=int.Parse(valueNode["m_FindNearestContactsPerJob"].ToString());
					jobOptions.updateTriggerContactsPerJob=int.Parse(valueNode["m_UpdateTriggerContactsPerJob"].ToString());
					jobOptions.islandSolverCostThreshold=int.Parse(valueNode["m_IslandSolverCostThreshold"].ToString());
					jobOptions.islandSolverBodyCostScale=int.Parse(valueNode["m_IslandSolverBodyCostScale"].ToString());
					jobOptions.islandSolverContactCostScale=int.Parse(valueNode["m_IslandSolverContactCostScale"].ToString());
					jobOptions.islandSolverJointCostScale=int.Parse(valueNode["m_IslandSolverJointCostScale"].ToString());
					jobOptions.islandSolverBodiesPerJob=int.Parse(valueNode["m_IslandSolverBodiesPerJob"].ToString());
					jobOptions.islandSolverContactsPerJob=int.Parse(valueNode["m_IslandSolverContactsPerJob"].ToString());
					physics2dData.jobOptions=jobOptions;
				}else if(keyNode.Value=="m_AutoSimulation"){
					physics2dData.autoSimulation=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_QueriesHitTriggers"){
					physics2dData.queriesHitTriggers=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_QueriesStartInColliders"){
					physics2dData.queriesStartInColliders=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_CallbacksOnDisable"){
					physics2dData.callbacksOnDisable=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_ReuseCollisionCallbacks"){
					physics2dData.reuseCollisionCallbacks=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_AutoSyncTransforms"){
					physics2dData.autoSyncTransforms=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_AlwaysShowColliders"){
					physics2dData.alwaysShowColliders=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_ShowColliderSleep"){
					physics2dData.showColliderSleep=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_ShowColliderContacts"){
					physics2dData.showColliderContacts=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_ShowColliderAABB"){
					physics2dData.showColliderAABB=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_ContactArrowScale"){
					physics2dData.contactArrowScale=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_ColliderAwakeColor"){
					physics2dData.colliderAwakeColor=readColor(valueNode);
				}else if(keyNode.Value=="m_ColliderAsleepColor"){
					physics2dData.colliderAsleepColor=readColor(valueNode);
				}else if(keyNode.Value=="m_ColliderContactColor"){
					physics2dData.colliderContactColor=readColor(valueNode);
				}else if(keyNode.Value=="m_ColliderAABBColor"){
					physics2dData.colliderAABBColor=readColor(valueNode);
				}else if(keyNode.Value=="m_LayerCollisionMatrix"){
					string matrixString=valueNode.ToString();
					int[] intList=new int[32];
					for(int i=0;i<32;i++){
						int value=Convert.ToInt32(matrixString.Substring(i*8,8),16);
						int a=value&0xFF;
						int r=value>>8&0xFF;
						int g=value>>16&0xFF;
						int b=value>>24&0xFF;
						value=b|(g<<8)|(r<<16)|(a<<24);
						intList[i]=value;
					}
					physics2dData.layerCollisionMatrix=intList;
				}
			}
			AssetDatabase.CreateAsset(physics2dData,ProjectImporterEditor.resourcePath+"/"+projectName+"_physics2dData.asset");
			AssetDatabase.Refresh();
		}

		private Color readColor(YamlNode valueNode){
			Color color=new Color();
			color.r=float.Parse(valueNode["r"].ToString());
			color.g=float.Parse(valueNode["g"].ToString());
			color.b=float.Parse(valueNode["b"].ToString());
			color.a=float.Parse(valueNode["a"].ToString());
			return color;
		}
	}
}