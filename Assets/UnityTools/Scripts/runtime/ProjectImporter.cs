namespace UnityTools{
	using UnityEngine;
	using UnityEngine.SceneManagement;
	
	public class ProjectImporter:MonoBehaviour{
		[Tooltip("场景加载器")]
		[SerializeField]
		private SceneLoader _sceneLoader=null;
		
		private static ProjectImporter _instance;

		private BuildSettingsData _buildSettingsData;
		private PhysicsData _physicsData;
		private Physics2dData _physics2dData;
		private QualityData _qualityData;
		private SortingLayersData _sortingLayersData;
		private LayersData _layersData;
		private TimeData _timeData;

		private void Awake(){
			_instance=this;
		}
		
		private void Start(){
			//test
			openProject("unity_uiDemo");
			//Invoke("onTimeout",3);
		}
		/*private void onTimeout(){
			Debug.Log("onTimeout");
			closeProject("unity_tags");
		}*/

		/// <summary>
		/// 打开一个项目
		/// </summary>
		/// <param name="projectFolderName">项目文件夹名</param>
		public void openProject(string projectFolderName){
			//加载BuildSettingsData
			_buildSettingsData=Resources.Load<BuildSettingsData>(projectFolderName+"_buildSettingsData");
			//加载PhysicsData
			_physicsData=Resources.Load<PhysicsData>(projectFolderName+"_physicsData");
			setPhysicsWithData(_physicsData);
			//加载Physics2dData
			_physics2dData=Resources.Load<Physics2dData>(projectFolderName+"_physics2dData");
			setPhysics2dWithData(_physics2dData);
			//加载QualityData
			_qualityData=Resources.Load<QualityData>(projectFolderName+"_qualityData");
			int qualityLevel=getPlatformDefaultQualityLevel(qualityData);
			QualitySettings2.setQualityLevelValue(qualityLevel);//初始化QualitySettings2.qualityLevel
			setQualityWithSettings(qualityData.qualitySettings[qualityLevel]);
			//加载SortingLayersData
			_sortingLayersData=Resources.Load<SortingLayersData>(projectFolderName+"_sortingLayersData");
			//加载LayersData
			_layersData=Resources.Load<LayersData>(projectFolderName+"_layersData");
			//加载TimeData
			_timeData=Resources.Load<TimeData>(projectFolderName+"_timeData");
			setTimeWithData(_timeData);
			//加载项目的主场景
			_sceneLoader.loadAsync(getMainSceneName(_buildSettingsData),LoadSceneMode.Additive);
		}

		/// <summary>
		/// 关闭一个项目
		/// </summary>
		/// <param name="projectFolderName">项目文件夹名</param>
		public void closeProject(string projectFolderName){
			//卸载项目的所有场景
			unloadProjectAllScenes(projectFolderName);
			//
			var defaultPhysicsData=Resources.Load<PhysicsData>("default_physicsData");
			setPhysicsWithData(defaultPhysicsData);
			//
			var defaultPhysics2dData=Resources.Load<Physics2dData>("default_physics2dData");
			setPhysics2dWithData(defaultPhysics2dData);
			//
			var defaultQualityData=Resources.Load<QualityData>("default_qualityData");
			int qualityLevel=getPlatformDefaultQualityLevel(defaultQualityData);
			setQualityWithSettings(defaultQualityData.qualitySettings[qualityLevel]);
			//
			var defaultTimeData=Resources.Load<TimeData>("default_timeData");
			setTimeWithData(defaultTimeData);
		}
		
		/// <summary>
		/// 重命名项目
		/// </summary>
		/// <param name="oldName">旧的名称</param>
		/// <param name="newName">新的名称</param>
		public void renameProject(string oldName, string newName){
			
		}

		/// <summary>
		/// 卸载指定项目的所有场景
		/// </summary>
		/// <param name="projectFolderName"></param>
		private void unloadProjectAllScenes(string projectFolderName){
			var projectBuildSettingsData=Resources.Load<BuildSettingsData>(projectFolderName+"_buildSettingsData");
			int i=projectBuildSettingsData.scenes.Length;
			while(--i>=0){
				string scenePath=projectBuildSettingsData.scenes[i].path;
				Scene scene=SceneManager.GetSceneByPath(scenePath);
				if(scene.IsValid()){
					SceneManager.UnloadSceneAsync(scenePath);
				}
			}
		}

		/// <summary>
		/// 返回项目的主场景路径名称
		/// </summary>
		/// <param name="buildSettingsData"></param>
		/// <returns></returns>
		private string getMainSceneName(BuildSettingsData buildSettingsData){
			string sceneName="";
			var scenes=buildSettingsData.scenes;
			int len=scenes.Length;
			for(int i=0;i<len;i++){
				var scene=scenes[i];
				if(scene.enabled){
					sceneName=scene.path;
					break;
				}
			}
			return sceneName;
		}
		
		/// <summary>根据指定的数据设置3d物理引擎参数</summary>
		public void setPhysicsWithData(PhysicsData physicsData){
			Physics.gravity=physicsData.gravity;
			//physicsData.defaultMaterial;
			Physics.bounceThreshold=physicsData.bounceThreshold;
			Physics.sleepThreshold=physicsData.sleepThreshold;
			Physics.defaultContactOffset=physicsData.defaultContactOffset;
			Physics.defaultSolverIterations=physicsData.defaultSolverIterations;
			Physics.defaultSolverVelocityIterations=physicsData.defaultSolverVelocityIterations;
			Physics.queriesHitBackfaces=physicsData.queriesHitBackfaces;
			Physics.queriesHitTriggers=physicsData.queriesHitTriggers;
			//physicsData.enableAdaptiveForce;
			Physics.interCollisionDistance=physicsData.clothInterCollisionDistance;
			Physics.interCollisionStiffness=physicsData.clothInterCollisionStiffness;
			//physicsData.contactsGeneration;
			//设置layerCollisionMatrix
			int layerValue=0;
			for(int i=0;i<32;i++){
				layerValue=1<<i;
				for(int j=0;j<32;j++){
					bool ignore=(layerValue&physicsData.layerCollisionMatrix[j])==0;
					Physics.IgnoreLayerCollision(i,j,ignore);
				}
			}
			//
			Physics.autoSimulation=physicsData.autoSimulation;
			Physics.autoSyncTransforms=physicsData.autoSyncTransforms;
			Physics.reuseCollisionCallbacks=physicsData.reuseCollisionCallbacks;
			Physics.interCollisionSettingsToggle=physicsData.clothInterCollisionSettingsToggle;
			Physics.clothGravity=physicsData.clothGravity;
			//physicsData.contactPairsMode;
			//physicsData.broadphaseType;
			//physicsData.worldBounds;
			//physicsData.worldSubdivisions;
			//physicsData.frictionType;
			//physicsData.enableEnhancedDeterminism;
			//physicsData.enableUnifiedHeightmaps;
			Physics.defaultMaxAngularSpeed=physicsData.defaultMaxAngularSpeed;
		}
		
		/// <summary>根据指定的数据设置2d物理引擎参数</summary>
		public void setPhysics2dWithData(Physics2dData physics2dData){
			Physics2D.gravity=physics2dData.gravity;
			//physics2dData.defaultMaterial;
			Physics2D.velocityIterations=physics2dData.velocityIterations;
			Physics2D.positionIterations=physics2dData.positionIterations;
			Physics2D.velocityThreshold=physics2dData.velocityThreshold;
			Physics2D.maxLinearCorrection=physics2dData.maxLinearCorrection;
			Physics2D.maxAngularCorrection=physics2dData.maxAngularCorrection;
			Physics2D.maxTranslationSpeed=physics2dData.maxTranslationSpeed;
			Physics2D.maxRotationSpeed=physics2dData.maxRotationSpeed;
			Physics2D.baumgarteScale=physics2dData.baumgarteScale;
			Physics2D.baumgarteTOIScale=physics2dData.baumgarteTimeOfImpactScale;
			Physics2D.timeToSleep=physics2dData.timeToSleep;
			Physics2D.linearSleepTolerance=physics2dData.linearSleepTolerance;
			Physics2D.angularSleepTolerance=physics2dData.angularSleepTolerance;
			Physics2D.defaultContactOffset=physics2dData.defaultContactOffset;
			//设置jobOptions
			var jobOptions=new PhysicsJobOptions2D();
			jobOptions.useMultithreading=physics2dData.jobOptions.useMultithreading;
			jobOptions.useConsistencySorting=physics2dData.jobOptions.useConsistencySorting;
			jobOptions.interpolationPosesPerJob=physics2dData.jobOptions.interpolationPosesPerJob;
			jobOptions.newContactsPerJob=physics2dData.jobOptions.newContactsPerJob;
			jobOptions.collideContactsPerJob=physics2dData.jobOptions.collideContactsPerJob;
			jobOptions.clearFlagsPerJob=physics2dData.jobOptions.clearFlagsPerJob;
			jobOptions.clearBodyForcesPerJob=physics2dData.jobOptions.clearBodyForcesPerJob;
			jobOptions.syncDiscreteFixturesPerJob=physics2dData.jobOptions.syncDiscreteFixturesPerJob;
			jobOptions.syncContinuousFixturesPerJob=physics2dData.jobOptions.syncContinuousFixturesPerJob;
			jobOptions.findNearestContactsPerJob=physics2dData.jobOptions.findNearestContactsPerJob;
			jobOptions.updateTriggerContactsPerJob=physics2dData.jobOptions.updateTriggerContactsPerJob;
			jobOptions.islandSolverCostThreshold=physics2dData.jobOptions.islandSolverCostThreshold;
			jobOptions.islandSolverBodyCostScale=physics2dData.jobOptions.islandSolverBodyCostScale;
			jobOptions.islandSolverContactCostScale=physics2dData.jobOptions.islandSolverContactCostScale;
			jobOptions.islandSolverJointCostScale=physics2dData.jobOptions.islandSolverJointCostScale;
			jobOptions.islandSolverBodiesPerJob=physics2dData.jobOptions.islandSolverBodiesPerJob;
			jobOptions.islandSolverContactsPerJob=physics2dData.jobOptions.islandSolverContactsPerJob;
			Physics2D.jobOptions=jobOptions;
			//
			Physics2D.autoSimulation=physics2dData.autoSimulation;
			Physics2D.queriesHitTriggers=physics2dData.queriesHitTriggers;
			Physics2D.queriesStartInColliders=physics2dData.queriesStartInColliders;
			Physics2D.callbacksOnDisable=physics2dData.callbacksOnDisable;
			Physics2D.reuseCollisionCallbacks=physics2dData.reuseCollisionCallbacks;
			Physics2D.autoSyncTransforms=physics2dData.autoSyncTransforms;
			Physics2D.alwaysShowColliders=physics2dData.alwaysShowColliders;
			Physics2D.showColliderSleep=physics2dData.showColliderSleep;
			Physics2D.showColliderContacts=physics2dData.showColliderContacts;
			Physics2D.showColliderAABB=physics2dData.showColliderAABB;
			Physics2D.contactArrowScale=physics2dData.contactArrowScale;
			Physics2D.colliderAwakeColor=physics2dData.colliderAwakeColor;
			Physics2D.colliderAsleepColor=physics2dData.colliderAsleepColor;
			Physics2D.colliderContactColor=physics2dData.colliderContactColor;
			Physics2D.colliderAABBColor=physics2dData.colliderAABBColor;
			//设置layerCollisionMatrix
			int layerValue=0;
			for(int i=0;i<32;i++){
				layerValue=1<<i;
				for(int j=0;j<32;j++){
					bool ignore=(layerValue&physics2dData.layerCollisionMatrix[j])==0;
					Physics2D.IgnoreLayerCollision(i,j,ignore);
				}
			}
		}
		
		#region setQualityWithData
		/// <summary>
		/// 根据一个UnityProjectImporter.QualitySettings设置品质
		/// </summary>
		/// <param name="qualitySettings">UnityProjectImporter.QualitySettings</param>
		public void setQualityWithSettings(QualitySettings qualitySettings){
			UnityEngine.QualitySettings.pixelLightCount=qualitySettings.pixelLightCount;
			UnityEngine.QualitySettings.shadows=(ShadowQuality)qualitySettings.shadows;
			UnityEngine.QualitySettings.shadowResolution=(ShadowResolution)qualitySettings.shadowResolution;
			UnityEngine.QualitySettings.shadowProjection=(ShadowProjection)qualitySettings.shadowProjection;
			UnityEngine.QualitySettings.shadowCascades=qualitySettings.shadowCascades;
			UnityEngine.QualitySettings.shadowDistance=qualitySettings.shadowDistance;
			UnityEngine.QualitySettings.shadowNearPlaneOffset=qualitySettings.shadowNearPlaneOffset;
			UnityEngine.QualitySettings.shadowCascade2Split=qualitySettings.shadowCascade2Split;
			UnityEngine.QualitySettings.shadowCascade4Split=qualitySettings.shadowCascade4Split;
			UnityEngine.QualitySettings.shadowmaskMode=(ShadowmaskMode)qualitySettings.shadowmaskMode;
			UnityEngine.QualitySettings.skinWeights=(SkinWeights)qualitySettings.skinWeights;
			//ualitySettings.textureQuality;
			//qualitySettings.anisotropicTextures;
			UnityEngine.QualitySettings.antiAliasing=qualitySettings.antiAliasing;
			UnityEngine.QualitySettings.softParticles=qualitySettings.softParticles;
			UnityEngine.QualitySettings.softVegetation=qualitySettings.softVegetation;
			UnityEngine.QualitySettings.realtimeReflectionProbes=qualitySettings.realtimeReflectionProbes;
			UnityEngine.QualitySettings.billboardsFaceCameraPosition=qualitySettings.billboardsFaceCameraPosition;
			UnityEngine.QualitySettings.vSyncCount=qualitySettings.vSyncCount;
			UnityEngine.QualitySettings.lodBias=qualitySettings.lodBias;
			UnityEngine.QualitySettings.maximumLODLevel=qualitySettings.maximumLODLevel;
			UnityEngine.QualitySettings.streamingMipmapsActive=qualitySettings.streamingMipmapsActive;
			UnityEngine.QualitySettings.streamingMipmapsAddAllCameras=qualitySettings.streamingMipmapsAddAllCameras;
			UnityEngine.QualitySettings.streamingMipmapsMemoryBudget=qualitySettings.streamingMipmapsMemoryBudget;
			UnityEngine.QualitySettings.streamingMipmapsRenderersPerFrame=qualitySettings.streamingMipmapsRenderersPerFrame;
			UnityEngine.QualitySettings.streamingMipmapsMaxLevelReduction=qualitySettings.streamingMipmapsMaxLevelReduction;
			UnityEngine.QualitySettings.streamingMipmapsMaxFileIORequests=qualitySettings.streamingMipmapsMaxFileIORequests;
			UnityEngine.QualitySettings.particleRaycastBudget=qualitySettings.particleRaycastBudget;
			UnityEngine.QualitySettings.asyncUploadTimeSlice=qualitySettings.asyncUploadTimeSlice;
			UnityEngine.QualitySettings.asyncUploadBufferSize=qualitySettings.asyncUploadBufferSize;
			UnityEngine.QualitySettings.asyncUploadPersistentBuffer=qualitySettings.asyncUploadPersistentBuffer;
			UnityEngine.QualitySettings.resolutionScalingFixedDPIFactor=qualitySettings.resolutionScalingFixedDPIFactor;
			//qualitySettings.excludedTargetPlatforms;
		}
		
		/// <summary>返回当前运行时平台的默认品质级别</summary>
		public int getPlatformDefaultQualityLevel(QualityData qualityData){
			RuntimePlatform platform=Application.platform;
			if(platform==RuntimePlatform.IPhonePlayer){
				return getDefaultQualityLevelWithPlatformName(qualityData,"iPhone");
			}else if(platform==RuntimePlatform.Android){
				return getDefaultQualityLevelWithPlatformName(qualityData,"Android");
			}else if(platform==RuntimePlatform.WebGLPlayer){
				return getDefaultQualityLevelWithPlatformName(qualityData,"WebGL");
			}else if(platform==RuntimePlatform.WindowsPlayer||platform==RuntimePlatform.OSXPlayer||platform==RuntimePlatform.LinuxPlayer){
				return getDefaultQualityLevelWithPlatformName(qualityData,"Standalone");
			}else if(platform==RuntimePlatform.PS4){
				return getDefaultQualityLevelWithPlatformName(qualityData,"PS4");
			}else if(platform==RuntimePlatform.WSAPlayerARM||platform==RuntimePlatform.WSAPlayerX86||platform==RuntimePlatform.WSAPlayerX64){
				return getDefaultQualityLevelWithPlatformName(qualityData,"Windows Store Apps");
			}else if(platform==RuntimePlatform.XboxOne){
				return getDefaultQualityLevelWithPlatformName(qualityData,"XboxOne");
			}else if(platform==RuntimePlatform.tvOS){
				return getDefaultQualityLevelWithPlatformName(qualityData,"tvOS");
			} 
			//默认返回编辑器中高亮显示的品质设置
			return qualityData.currentQuality;
		}
		
		/// <summary>根据平台名称返回平台默认的品质级别</summary>
		private int getDefaultQualityLevelWithPlatformName(QualityData qualityData,string platformName){
			PlatformDefaultQuality[] platformDefaultQualities=qualityData.perPlatformDefaultQuality;
			int len=platformDefaultQualities.Length;
			for(int i=0;i<len;i++){
				PlatformDefaultQuality platformDefaultQuality=platformDefaultQualities[i];
				if(platformDefaultQuality.platform==platformName){
					return platformDefaultQuality.qualityLevel;
				}
			}
			Debug.LogError("没找到"+platformName+"平台的默认品质级别，请确认平台："+platformName+"是否存在。");
			return -1;
		}
		#endregion setQualityWithData

		private void setTimeWithData(TimeData timeData){
			Time.fixedDeltaTime=timeData.fixedTimestep;
			Time.maximumDeltaTime=timeData.maximumAllowedTimestep;
			Time.timeScale=timeData.timeScale;
			Time.maximumParticleDeltaTime=timeData.maximumParticleTimestep;
		}

		private void OnDestroy() {
			_instance=null;
		}
		
		public static ProjectImporter instance{ get => _instance; }
		public SceneLoader sceneLoader{ get => _sceneLoader; }
		public BuildSettingsData buildSettingsData{ get => _buildSettingsData; }
		public PhysicsData physicsData{ get => _physicsData; }
		public QualityData qualityData{ get => _qualityData; }
		public SortingLayersData sortingLayersData{ get => _sortingLayersData; }
		public LayersData layersData{ get => _layersData; }
		public TimeData timeData{ get => _timeData; }
	}
}