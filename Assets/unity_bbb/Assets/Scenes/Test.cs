namespace unity_bbb{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	
	public class Test : MonoBehaviour
	{
	    // Start is called before the first frame update
	    void Start()
	    {
	        Vector3 v=new Vector3(1.0f,0.0f,0.0f);
			Quaternion rotation=Quaternion.Euler(0.0f,-45.0f,0.0f);
	
			var result=rotation*v;
			Debug.Log(result);
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}

}