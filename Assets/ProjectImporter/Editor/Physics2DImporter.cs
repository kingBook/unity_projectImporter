namespace UnityProjectImporter{
	using UnityEngine;
	using UnityEditor;
    using System.Collections.Generic;

    public class Physics2DImporter:Importer{
		/// <summary>
		/// 导入项目的Physics2DSettings
		/// </summary>
		/// <param name="path">需要导入Physics2DSettings的项目路径</param>
		/// <param name="projectImporterTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void import(string path,string projectImporterTempPath,string projectName){
			//Physics2DSettings.asset 原来的位置
			string sourceTagFilePath=path+"/ProjectSettings/Physics2DSettings.asset";
			//Physics2DSettings.asset 复制过来的位置
			string destTagFilePath=projectImporterTempPath+"/Physics2DSettings.asset";
			//复制 Physics2DSettings.asset
			FileUtil2.copyFile(sourceTagFilePath,destTagFilePath,true);
			//加载并转换成SerializedObject
			string destTagAssetPath=projectImporterTempPath+"/Physics2DSettings.asset";
			SerializedObject copyDynamicsManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagAssetPath));

			Physics2dData physics2dData=ScriptableObject.CreateInstance<Physics2dData>();

			var it=copyDynamicsManager.GetIterator();
			while (it.Next(true)){
				string itName=it.name;
				if(itName=="m_Gravity"){
					Vector2 v2=new Vector2();
					v2.x=it.FindPropertyRelative("x").floatValue;
					v2.y=it.FindPropertyRelative("y").floatValue;
					physics2dData.gravity=v2;
				}else if(itName=="m_DefaultMaterial"){
					//获取默认物理材质
					int fileId=it.FindPropertyRelative("m_FileID").intValue;
					string defaultPhysicsMaterialPath=AssetDatabase.GetAssetPath(fileId);
					physics2dData.defaultMaterial=AssetDatabase.LoadAssetAtPath<PhysicsMaterial2D>(defaultPhysicsMaterialPath);//当没有设置时会自动为None
				}else if(itName=="m_VelocityIterations"){
					physics2dData.velocityIterations=it.intValue;
				}else if(itName=="m_PositionIterations"){
					physics2dData.positionIterations=it.intValue;
				}else if(itName=="m_VelocityThreshold"){
					physics2dData.velocityThreshold=it.floatValue;
				}else if(itName=="m_MaxLinearCorrection"){
					physics2dData.maxLinearCorrection=it.floatValue;
				}else if(itName=="m_MaxAngularCorrection"){
					physics2dData.maxAngularCorrection=it.floatValue;
				}else if(itName=="m_MaxTranslationSpeed"){
					physics2dData.maxTranslationSpeed=it.floatValue;
				}else if(itName=="m_MaxRotationSpeed"){
					physics2dData.maxRotationSpeed=it.floatValue;
				}else if(itName=="m_BaumgarteScale"){
					physics2dData.baumgarteScale=it.floatValue;
				}else if(itName=="m_BaumgarteTimeOfImpactScale"){
					physics2dData.baumgarteTimeOfImpactScale=it.floatValue;
				}else if(itName=="m_TimeToSleep"){
					physics2dData.timeToSleep=it.floatValue;
				}else if(itName=="m_LinearSleepTolerance"){
					physics2dData.linearSleepTolerance=it.floatValue;
				}else if(itName=="m_AngularSleepTolerance"){
					physics2dData.angularSleepTolerance=it.floatValue;
				}else if(itName=="m_DefaultContactOffset"){
					physics2dData.defaultContactOffset=it.floatValue;
				}else if(itName=="m_JobOptions"){
					JobOptions jobOptions=new JobOptions();
					jobOptions.useMultithreading=it.FindPropertyRelative("useMultithreading").boolValue;
					jobOptions.useConsistencySorting=it.FindPropertyRelative("useConsistencySorting").boolValue;
					jobOptions.interpolationPosesPerJob=it.FindPropertyRelative("m_InterpolationPosesPerJob").intValue;
					jobOptions.newContactsPerJob=it.FindPropertyRelative("m_NewContactsPerJob").intValue;
					jobOptions.collideContactsPerJob=it.FindPropertyRelative("m_CollideContactsPerJob").intValue;
					jobOptions.clearFlagsPerJob=it.FindPropertyRelative("m_ClearFlagsPerJob").intValue;
					jobOptions.clearBodyForcesPerJob=it.FindPropertyRelative("m_ClearBodyForcesPerJob").intValue;
					jobOptions.syncDiscreteFixturesPerJob=it.FindPropertyRelative("m_SyncDiscreteFixturesPerJob").intValue;
					jobOptions.syncContinuousFixturesPerJob=it.FindPropertyRelative("m_SyncContinuousFixturesPerJob").intValue;
					jobOptions.findNearestContactsPerJob=it.FindPropertyRelative("m_FindNearestContactsPerJob").intValue;
					jobOptions.updateTriggerContactsPerJob=it.FindPropertyRelative("m_UpdateTriggerContactsPerJob").intValue;
					jobOptions.islandSolverCostThreshold=it.FindPropertyRelative("m_IslandSolverCostThreshold").intValue;
					jobOptions.islandSolverBodyCostScale=it.FindPropertyRelative("m_IslandSolverBodyCostScale").intValue;
					jobOptions.islandSolverContactCostScale=it.FindPropertyRelative("m_IslandSolverContactCostScale").intValue;
					jobOptions.islandSolverJointCostScale=it.FindPropertyRelative("m_IslandSolverJointCostScale").intValue;
					jobOptions.islandSolverBodiesPerJob=it.FindPropertyRelative("m_IslandSolverBodiesPerJob").intValue;
					jobOptions.islandSolverContactsPerJob=it.FindPropertyRelative("m_IslandSolverContactsPerJob").intValue;
					physics2dData.jobOptions=jobOptions;
				}else if(itName=="m_AutoSimulation"){
					physics2dData.autoSimulation=it.boolValue;
				}else if(itName=="m_QueriesHitTriggers"){
					physics2dData.queriesHitTriggers=it.boolValue;
				}else if(itName=="m_QueriesStartInColliders"){
					physics2dData.queriesStartInColliders=it.boolValue;
				}else if(itName=="m_CallbacksOnDisable"){
					physics2dData.callbacksOnDisable=it.boolValue;
				}else if(itName=="m_ReuseCollisionCallbacks"){
					physics2dData.reuseCollisionCallbacks=it.boolValue;
				}else if(itName=="m_AutoSyncTransforms"){
					physics2dData.autoSyncTransforms=it.boolValue;
				}else if(itName=="m_AlwaysShowColliders"){
					physics2dData.alwaysShowColliders=it.boolValue;
				}else if(itName=="m_ShowColliderSleep"){
					physics2dData.showColliderSleep=it.boolValue;
				}else if(itName=="m_ShowColliderContacts"){
					physics2dData.showColliderContacts=it.boolValue;
				}else if(itName=="m_ShowColliderAABB"){
					physics2dData.showColliderAABB=it.boolValue;
				}else if(itName=="m_ContactArrowScale"){
					physics2dData.contactArrowScale=it.floatValue;
				}else if(itName=="m_ColliderAwakeColor"){
					physics2dData.colliderAwakeColor=it.colorValue;
				}else if(itName=="m_ColliderAsleepColor"){
					physics2dData.colliderAsleepColor=it.colorValue;
				}else if(itName=="m_ColliderContactColor"){
					physics2dData.colliderContactColor=it.colorValue;
				}else if(itName=="m_ColliderAABBColor"){
					physics2dData.colliderAABBColor=it.colorValue;
				}else if(itName=="m_LayerCollisionMatrix"){
					int arraySize=it.arraySize;
					int[] intList=new int[arraySize];
					for(int i=0;i<arraySize;i++){
						var element=it.GetArrayElementAtIndex(i);
						intList[i]=element.intValue;
					}
					physics2dData.layerCollisionMatrix=intList;
				}
			}

			AssetDatabase.CreateAsset(physics2dData,"Assets/ProjectImporter/Resources/"+projectName+"_physics2dData.asset");
			//删除复制过来的"Physics2DSettings.asset"
			AssetDatabase.DeleteAsset(destTagFilePath);
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}
	}
}