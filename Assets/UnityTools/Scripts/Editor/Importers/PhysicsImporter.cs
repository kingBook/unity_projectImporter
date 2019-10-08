namespace UnityTools{
	using System;
	using UnityEditor;
	using UnityEngine;

	public class PhysicsImporter:Importer{
		/// <summary>
		/// 导入项目的PhysicsSettings
		/// </summary>
		/// <param name="path">需要导入PhysicsSettings的项目路径</param>
		/// <param name="projectImporterTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void import(string path,string projectImporterTempPath,string projectName){
			//DynamicsManager.asset原来的位置
			string sourceTagFilePath=path+"/ProjectSettings/DynamicsManager.asset";
			//DynamicsManager.asset复制过来的位置
			string destTagFilePath=projectImporterTempPath+"/DynamicsManager.asset";
			//复制DynamicsManager.asset
			FileUtil2.copyFile(sourceTagFilePath,destTagFilePath,true);
			//加载并转换成SerializedObject
			string destTagAssetPath=projectImporterTempPath+"/DynamicsManager.asset";
			SerializedObject copyDynamicsManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagAssetPath));

			PhysicsData physicsData=ScriptableObject.CreateInstance<PhysicsData>();

			var it=copyDynamicsManager.GetIterator();
			while (it.Next(true)){
				string itName=it.name;
				if(itName=="m_Gravity"){
					Vector3 v3=new Vector3();
					v3.x=it.FindPropertyRelative("x").floatValue;
					v3.y=it.FindPropertyRelative("y").floatValue;
					v3.z=it.FindPropertyRelative("z").floatValue;
					physicsData.gravity=v3;
				}else if(itName=="m_DefaultMaterial"){
					//获取默认物理材质
					int fileId=it.FindPropertyRelative("m_FileID").intValue;
					string defaultPhysicsMaterialPath = AssetDatabase.GetAssetPath(fileId);
					physicsData.defaultMaterial=AssetDatabase.LoadAssetAtPath<PhysicMaterial>(defaultPhysicsMaterialPath);//当没有设置时会自动为None
				}else if(itName=="m_BounceThreshold"){
					physicsData.bounceThreshold=it.floatValue;
				}else if(itName=="m_SleepThreshold"){
					physicsData.sleepThreshold=it.floatValue;
				}else if(itName=="m_DefaultContactOffset"){
					physicsData.defaultContactOffset=it.floatValue;
				}else if(itName=="m_DefaultSolverIterations"){
					physicsData.defaultSolverIterations=it.intValue;
				}else if(itName=="m_DefaultSolverVelocityIterations"){
					physicsData.defaultSolverVelocityIterations=it.intValue;
				}else if(itName=="m_QueriesHitBackfaces"){
					physicsData.queriesHitBackfaces=it.boolValue;
				}else if(itName=="m_QueriesHitTriggers"){
					physicsData.queriesHitTriggers=it.boolValue;
				}else if(itName=="m_EnableAdaptiveForce"){
					physicsData.enableAdaptiveForce=it.boolValue;
				}else if(itName=="m_ClothInterCollisionDistance"){
					physicsData.clothInterCollisionDistance=it.floatValue;
				}else if(itName=="m_ClothInterCollisionStiffness"){
					physicsData.clothInterCollisionStiffness=it.floatValue;
				}else if(itName=="m_ContactsGeneration"){
					physicsData.contactsGeneration=it.intValue;
				}else if(itName=="m_LayerCollisionMatrix"){
					int arraySize=it.arraySize;
					int[] intList=new int[arraySize];
					for(int i=0;i<arraySize;i++){
						var element=it.GetArrayElementAtIndex(i);
						intList[i]=element.intValue;
					}
					physicsData.layerCollisionMatrix=intList;
				}else if(itName=="m_AutoSimulation"){
					physicsData.autoSimulation=it.boolValue;
				}else if(itName=="m_AutoSyncTransforms"){
					physicsData.autoSyncTransforms=it.boolValue;
				}else if(itName=="m_ReuseCollisionCallbacks"){
					physicsData.reuseCollisionCallbacks=it.boolValue;
				}else if(itName=="m_ClothInterCollisionSettingsToggle"){
					physicsData.clothInterCollisionSettingsToggle=it.boolValue;
				}else if(itName=="m_ClothGravity"){
					physicsData.clothGravity=it.vector3Value;
				}else if(itName=="m_ContactPairsMode"){
					physicsData.contactPairsMode=it.intValue;
				}else if(itName=="m_BroadphaseType"){
					physicsData.broadphaseType=it.intValue;
				}else if(itName=="m_WorldBounds"){
					physicsData.worldBounds=it.boundsValue;
				}else if(itName=="m_WorldSubdivisions"){
					physicsData.worldSubdivisions=it.intValue;
				}else if(itName=="m_FrictionType"){
					physicsData.frictionType=it.intValue;
				}else if(itName=="m_EnableEnhancedDeterminism"){
					physicsData.enableEnhancedDeterminism=it.boolValue;
				}else if(itName=="m_EnableUnifiedHeightmaps"){
					physicsData.enableUnifiedHeightmaps=it.boolValue;
				}else if(itName=="m_DefaultMaxAngularSpeed"){
					physicsData.defaultMaxAngularSpeed=it.floatValue;
				}
			}

			AssetDatabase.CreateAsset(physicsData,ProjectImporterEditor.resourcePath+"/"+projectName+"_physicsData.asset");
			//删除复制过来的"DynamicsManager.asset"
			AssetDatabase.DeleteAsset(destTagFilePath);
			AssetDatabase.Refresh();

		}
	}

}