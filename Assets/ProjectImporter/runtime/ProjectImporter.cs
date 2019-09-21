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
			openProject("unity_tags");
			
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
			setQualityWithData(_qualityData);
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
		private void setQualityWithData(QualityData qualityData){
			///UnityEngine.QualitySettings.
			
			QualitySettings qualitySettings=getPlatformDefaultQualitySettings(qualityData);

			
		}

		/// <summary>返回当前运行时平台的默认品质设置</summary>
		private QualitySettings getPlatformDefaultQualitySettings(QualityData qualityData){
			RuntimePlatform platform=Application.platform;
			if(platform==RuntimePlatform.IPhonePlayer){
				return getQualitySettingsWithPlatformName(qualityData,"iPhone");
			}else if(platform==RuntimePlatform.Android){
				return getQualitySettingsWithPlatformName(qualityData,"Android");
			}else if(platform==RuntimePlatform.WebGLPlayer){
				return getQualitySettingsWithPlatformName(qualityData,"WebGL");
			}else if(platform==RuntimePlatform.WindowsPlayer||platform==RuntimePlatform.OSXPlayer||platform==RuntimePlatform.LinuxPlayer){
				return getQualitySettingsWithPlatformName(qualityData,"Standalone");
			}else if(platform==RuntimePlatform.PS4){
				return getQualitySettingsWithPlatformName(qualityData,"PS4");
			}else if(platform==RuntimePlatform.WSAPlayerARM||platform==RuntimePlatform.WSAPlayerX86||platform==RuntimePlatform.WSAPlayerX64){
				return getQualitySettingsWithPlatformName(qualityData,"Windows Store Apps");
			}else if(platform==RuntimePlatform.XboxOne){
				return getQualitySettingsWithPlatformName(qualityData,"XboxOne");
			}else if(platform==RuntimePlatform.tvOS){
				return getQualitySettingsWithPlatformName(qualityData,"tvOS");
			} 
			//默认返回编辑器中高亮显示的品质设置
			return qualityData.qualitySettings[qualityData.currentQuality];
		}
		private QualitySettings getQualitySettingsWithPlatformName(QualityData qualityData,string platformName){
			int len=qualityData.qualitySettings.Length;
			for(int i=0;i<len;i++){
				QualitySettings tempQualitySettings=qualityData.qualitySettings[i];
				if(tempQualitySettings.name==platformName){
					return tempQualitySettings;
				}

			}
			Debug.LogError("没找到"+platformName+"平台的品质设置数据，请确认平台："+platformName+"是否存在。");
			return new QualitySettings();
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