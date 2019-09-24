namespace UnityProjectImporter{
	using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ProjectImporter:MonoBehaviour{
		[Tooltip("场景加载器")]
		[SerializeField]
		private SceneLoader _sceneLoader=null;
		
		private static ProjectImporter _instance;

		private BuildSettingsData _buildSettingsData;
		private QualityData _qualityData;
		private SortingLayersData _sortingLayersData;
		private LayersData _layersData;
		private TimeData _timeData;

		private void Awake(){
			_instance=this;
		}
		
		private void Start(){
			//test
			openProject("unity_parkinggame");
		}

		/// <summary>
		/// 打开一个项目
		/// </summary>
		/// <param name="projectName">项目文件夹名</param>
		public void openProject(string projectFolderName){
			//加载BuildSettingsData
			_buildSettingsData=Resources.Load<BuildSettingsData>(projectFolderName+"_buildSettingsData");
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
		/// <param name="projectName">项目文件夹名</param>
		public void closeProject(string projectFolderName){
			//setTimeWithData(_defaultTimeData);
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
			//UnityEngine.QualitySettings.textureQuality=qualitySettings.textureQuality;
			//UnityEngine.QualitySettings.anisotropicTextures=qualitySettings.anisotropicTextures;
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
			//UnityEngine.QualitySettings.excludedTargetPlatforms=qualitySettings.excludedTargetPlatforms;
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
		public QualityData qualityData{ get => _qualityData; }
		public SortingLayersData sortingLayersData{ get => _sortingLayersData; }
		public LayersData layersData{ get => _layersData; }
		public TimeData timeData{ get => _timeData; }
	}
}