namespace UnityTools{
	using System;
    using System.IO;
    using System.Text;
    using UnityEditor;
	using UnityEngine;
    using YamlDotNet.RepresentationModel;

    public class PhysicsImporter:Importer{
		/// <summary>
		/// 导入项目的PhysicsSettings
		/// </summary>
		/// <param name="path">需要导入PhysicsSettings的项目路径</param>
		/// <param name="currentProjectTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void Import(string path,string currentProjectTempPath,string projectName){
			//需要导入的DynamicsManager.asset的路径
			string settingsFilePath=path+"/ProjectSettings/DynamicsManager.asset";

			StreamReader streamReader=new StreamReader(settingsFilePath,Encoding.UTF8);
			YamlStream yaml=new YamlStream();
			yaml.Load(streamReader);
			streamReader.Dispose();
			streamReader.Close();

			YamlNode rootNode=yaml.Documents[0].RootNode;
			YamlMappingNode firstNode=(YamlMappingNode)rootNode["PhysicsManager"];

			PhysicsData physicsData=ScriptableObject.CreateInstance<PhysicsData>();
			foreach(var item in firstNode){
				var keyNode=(YamlScalarNode)item.Key;
				var valueNode=item.Value;
				if(keyNode.Value=="m_Gravity"){
					physicsData.gravity=ReadVector3(valueNode);
				}else if(keyNode.Value=="m_DefaultMaterial"){
					//获取默认物理材质
					int fileId=int.Parse(valueNode["fileID"].ToString());
					string defaultPhysicsMaterialPath=AssetDatabase.GetAssetPath(fileId);
					physicsData.defaultMaterial=AssetDatabase.LoadAssetAtPath<PhysicMaterial>(defaultPhysicsMaterialPath);//当没有设置时会自动为None
				}else if(keyNode.Value=="m_BounceThreshold"){
					physicsData.bounceThreshold=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_SleepThreshold"){
					physicsData.sleepThreshold=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_DefaultContactOffset"){
					physicsData.defaultContactOffset=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_DefaultSolverIterations"){
					physicsData.defaultSolverIterations=int.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_DefaultSolverVelocityIterations"){
					physicsData.defaultSolverVelocityIterations=int.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_QueriesHitBackfaces"){
					physicsData.queriesHitBackfaces=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_QueriesHitTriggers"){
					physicsData.queriesHitTriggers=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_EnableAdaptiveForce"){
					physicsData.enableAdaptiveForce=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_ClothInterCollisionDistance"){
					physicsData.clothInterCollisionDistance=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_ClothInterCollisionStiffness"){
					physicsData.clothInterCollisionStiffness=float.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_ContactsGeneration"){
					physicsData.contactsGeneration=int.Parse(valueNode.ToString());
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
					physicsData.layerCollisionMatrix=intList;
				}else if(keyNode.Value=="m_AutoSimulation"){
					physicsData.autoSimulation=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_AutoSyncTransforms"){
					physicsData.autoSyncTransforms=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_ReuseCollisionCallbacks"){
					physicsData.reuseCollisionCallbacks=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_ClothInterCollisionSettingsToggle"){
					physicsData.clothInterCollisionSettingsToggle=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_ClothGravity"){
					physicsData.clothGravity=ReadVector3(valueNode);
				}else if(keyNode.Value=="m_ContactPairsMode"){
					physicsData.contactPairsMode=int.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_BroadphaseType"){
					physicsData.broadphaseType=int.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_WorldBounds"){
					Vector3 center=ReadVector3(valueNode["m_Center"]);
					Vector3 extent=ReadVector3(valueNode["m_Extent"]);
					physicsData.worldBounds=new Bounds(center,extent*2);
				}else if(keyNode.Value=="m_WorldSubdivisions"){
					physicsData.worldSubdivisions=int.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_FrictionType"){
					physicsData.frictionType=int.Parse(valueNode.ToString());
				}else if(keyNode.Value=="m_EnableEnhancedDeterminism"){
					physicsData.enableEnhancedDeterminism=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_EnableUnifiedHeightmaps"){
					physicsData.enableUnifiedHeightmaps=valueNode.ToString()=="1";
				}else if(keyNode.Value=="m_DefaultMaxAngularSpeed"){
					physicsData.defaultMaxAngularSpeed=float.Parse(valueNode.ToString());
				}
			}

			AssetDatabase.CreateAsset(physicsData,ProjectImporterEditor.resourcePath+"/"+projectName+"_physicsData.asset");
			AssetDatabase.Refresh();
		}

		private Vector3 ReadVector3(YamlNode node){
			Vector3 vector3=new Vector3();
			vector3.x=float.Parse(node["x"].ToString());
			vector3.y=float.Parse(node["y"].ToString());
			vector3.z=float.Parse(node["z"].ToString());
			return vector3;
		}
	}

}