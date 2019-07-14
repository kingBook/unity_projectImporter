namespace UnityProjectImporter{
	using UnityEngine;
	public class ProjectImporter:MonoBehaviour{
		private static ProjectImporter _instance;

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
			//����SortingLayersData
			_sortingLayersData=Resources.Load<SortingLayersData>(projectFolderName+"_sortingLayersData");
			//

		}

		/// <summary>
		/// �ر�һ����Ŀ
		/// </summary>
		/// <param name="projectName">��Ŀ�ļ�����</param>
		public void closeProject(string projectFolderName){
			
		}

		private void OnDestroy() {
			_instance=null;
		}

		public ProjectImporter instance{ get => _instance; }

		public SortingLayersData sortingLayersData{ get => _sortingLayersData; }
	}
}