using UnityEngine;
public class ProjectImporter:MonoBehaviour{
	private static ProjectImporter _instance;

    private void Awake(){
        _instance=this;
		DontDestroyOnLoad(gameObject);//�����³���ʱ����
    }
    
    private void Start(){
        
    }

	public ProjectImporter instance{ get => _instance; }
}