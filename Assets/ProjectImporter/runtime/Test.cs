using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		string str="effd2d16da2d75d4886f902f5962d16b";
        Debug.Log(str.Length);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
			string str=getGUID();
			Debug.Log(str+","+str.Length);
		}
    }

	private string getGUID(){
		System.Guid guid=new System.Guid();
		guid=Guid.NewGuid();
		return guid.ToString("N");
	}
}
