using UnityEngine;
using UnityEditor;

public class Test:MonoBehaviour{
	[SerializeField]
	private LayerMask obsLayerMask;
	private void Start() {
		Debug.Log(obsLayerMask.value);
	}
}