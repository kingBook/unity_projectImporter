namespace UnityProjectImporter{
	using UnityEngine;
	using System.Collections;

	[System.Serializable]
	public struct Scene{
		public bool enabled;
		public string path;
	}

	public class BuildSettingsData:ScriptableObject{
		public Scene[] scenes;
	}
}
