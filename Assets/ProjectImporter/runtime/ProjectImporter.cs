namespace UnityProjectImporter{
	using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ProjectImporter:MonoBehaviour{
		[Tooltip("����������")]
		[SerializeField]
		private SceneLoader _sceneLoader=null;
		
		private static ProjectImporter _instance;

		private BuildSettingsData _buildSettingsData;
		private SortingLayersData _sortingLayersData;

		private void Awake(){
			_instance=this;
			DontDestroyOnLoad(gameObject);//�����³���ʱ����
		}
		
		private void Start(){
			//test
			openProject("unity_tags");
			
		}

		/// <summary>
		/// ��һ����Ŀ
		/// </summary>
		/// <param name="projectName">��Ŀ�ļ�����</param>
		public void openProject(string projectFolderName){
			//����BuildSettingsData
			_buildSettingsData=Resources.Load<BuildSettingsData>(projectFolderName+"_buildSettingsData");
			//����SortingLayersData
			_sortingLayersData=Resources.Load<SortingLayersData>(projectFolderName+"_sortingLayersData");
			//������Ŀ��������
			_sceneLoader.loadAsync(getMainSceneName(_buildSettingsData),LoadSceneMode.Additive);

		}

		/// <summary>
		/// �ر�һ����Ŀ
		/// </summary>
		/// <param name="projectName">��Ŀ�ļ�����</param>
		public void closeProject(string projectFolderName){
			
		}

		/// <summary>
		/// ������Ŀ��������·������
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