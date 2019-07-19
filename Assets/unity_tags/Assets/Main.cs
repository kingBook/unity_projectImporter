namespace unity_tags{
	using System.Collections;	using System.Collections.Generic;	using UnityEngine;	using UnityEngine.SceneManagement;		public class Main : MonoBehaviour{			    void Start(){	        Invoke("enterNewScene",2);	    }		    		private void enterNewScene(){			SceneManager.LoadSceneAsync("New Scene");		}		    void Update(){	        	    }	}
}