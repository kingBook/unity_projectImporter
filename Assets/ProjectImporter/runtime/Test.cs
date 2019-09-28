using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityProjectImporter;

public class Test:MonoBehaviour{
	public Camera camera;
	private void Start() {
		
	}

	private void Update(){
		var pos=Input.mousePosition;
		//pos.z=cam.transform.position.z+10;
		pos.z=11-camera.transform.position.z;
		pos=camera.ScreenToWorldPoint(pos);
		Debug.Log(camera.transform.position.z+","+camera.nearClipPlane+","+pos.z);
		transform.position=pos;


		transform.rotation=camera.transform.rotation;
	}
}