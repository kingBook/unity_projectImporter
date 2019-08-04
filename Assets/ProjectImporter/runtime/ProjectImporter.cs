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

		private void setQualityWithData(QualityData qualityData){
			
		}

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