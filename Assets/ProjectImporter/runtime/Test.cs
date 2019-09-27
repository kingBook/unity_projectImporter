using UnityEngine;
using UnityEditor;

public class Test:MonoBehaviour{
	[SerializeField]
	private LayerMask obsLayerMask;
	private void Start() {
		Debug.Log("allLayers:"+Physics.AllLayers);
		Debug.Log("IgnoreRaycastLayer:"+Physics.IgnoreRaycastLayer);
		Debug.Log("DefaultRaycastLayers:"+Physics.DefaultRaycastLayers);
	}
}