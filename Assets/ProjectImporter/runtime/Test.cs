using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityProjectImporter;

public class Test:MonoBehaviour{
	public Camera cam;
	private void Start() {
		
	}

	/*private void Update(){
		if(Input.GetMouseButtonDown(0)){
			var pos=Input.mousePosition;
			var camera=Camera.main;
			pos.z=camera.nearClipPlane;
			pos=camera.ScreenToWorldPoint(pos);
			transform.position=pos;
		}
	}*/
	void OnGUI()
    {
        Vector3 point = new Vector3();
        Event   currentEvent = Event.current;
        Vector2 mousePos = new Vector2();

        // Get the mouse position from Event.
        // Note that the y position from Event is inverted.
        mousePos.x = currentEvent.mousePosition.x;
        mousePos.y = cam.pixelHeight - currentEvent.mousePosition.y;

        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
		transform.position=point;
        GUILayout.BeginArea(new Rect(20, 20, 250, 120));
        GUILayout.Label("Screen pixels: " + cam.pixelWidth + ":" + cam.pixelHeight);
        GUILayout.Label("Mouse position: " + mousePos);
        GUILayout.Label("World position: " + point.ToString("F3"));
        GUILayout.EndArea();
    }
}