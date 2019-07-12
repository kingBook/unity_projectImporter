using UnityEngine;
public class ProjectImporter:MonoBehaviour{
	private static ProjectImporter _instance;

    private void Awake(){
        _instance=this;
		DontDestroyOnLoad(gameObject);//加载新场景时保留
    }
    
    private void Start(){
        
    }

	public ProjectImporter instance{ get => _instance; }
}