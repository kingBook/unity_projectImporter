namespace UnityProjectImporter{
	using UnityEngine;
	public class ProjectImporter:MonoBehaviour{
		private static ProjectImporter _instance;

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
			//加载SortingLayersData
			_sortingLayersData=Resources.Load<SortingLayersData>(projectFolderName+"_sortingLayersData");
			//

		}

		/// <summary>
		/// 关闭一个项目
		/// </summary>
		/// <param name="projectName">项目文件夹名</param>
		public void closeProject(string projectFolderName){
			
		}

		private void OnDestroy() {
			_instance=null;
		}

		public ProjectImporter instance{ get => _instance; }

		public SortingLayersData sortingLayersData{ get => _sortingLayersData; }
	}
}