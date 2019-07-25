namespace unity_tags{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	
	public class Main : MonoBehaviour{
		
	    void Start(){
			/*for(int i=0;i<100;i++){
				Debug.Log(Random.Range(int.MinValue,int.MaxValue));
			}*/
			Debug.Log(int.MinValue+","+int.MaxValue);
	        Invoke("enterNewScene",2);
	    }
	
	    
		private void enterNewScene(){
			SceneManager2.LoadSceneAsync("New Scene",LoadSceneMode.Additive);
		}
	
	    void Update(){
	        
	    }
	}

}