namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	using UnityEngine.SceneManagement;
	
	public class UICallbacksTitle:UICallbacksBase{
	
		///<summary>清除本地存储的数据</summary>
		public void clearLocalData(){
	        PlayerPrefs.DeleteAll();
	        PlayerPrefs.Save();
	    }
	
	}

}