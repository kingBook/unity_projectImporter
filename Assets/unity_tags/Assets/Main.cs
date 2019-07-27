namespace unity_tags{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.SceneManagement;
	
	public class Main : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			Invoke("createNewScene",2.0f);
	
		}
	
		private void createNewScene(){
			SceneManager2	.  LoadSceneAsync("Scenes/New Scene");
		}
	
		// Update is called once per frame
		void Update()
		{
			
		}
	}
	

}