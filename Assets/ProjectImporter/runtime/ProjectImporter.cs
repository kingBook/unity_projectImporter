namespace UnityProjectImporter{
	using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ProjectImporter:MonoBehaviour{
		[Tooltip("场景加载器")]
		[SerializeField]
		private SceneLoader _sceneLoader=null;
		
		private static ProjectImporter _instance;

		private BuildSettingsData _buildSettingsData;
		private SortingLayersData _sortingLayersData;

		private void Awake(){
			_instance=this;
			DontDestroyOnLoad(gameObject);//加载新场景时保留
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
			//加载SortingLayersData
			_sortingLayersData=Resources.Load<SortingLayersData>(projectFolderName+"_sortingLayersData");
			//加载项目的主场景
			_sceneLoader.loadAsync(getMainSceneName(_buildSettingsData),LoadSceneMode.Additive);

		}

		/// <summary>
		/// 关闭一个项目
		/// </summary>
		/// <param name="projectName">项目文件夹名</param>
		public void closeProject(string projectFolderName){
			
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

		private void OnDestroy() {
			_instance=null;
		}

		public static ProjectImporter instance{ get => _instance; }
		public SceneLoader sceneLoader{ get => _sceneLoader; }
		public BuildSettingsData buildSettingsData{ get => _buildSettingsData; }
		public SortingLayersData sortingLayersData{ get => _sortingLayersData; }
	}
}