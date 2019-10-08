namespace UnityTools{
	using UnityEngine;
	using System.Collections;

	[System.Serializable]
	public struct SceneData{
		public bool enabled;
		public string path;
	}

	public class BuildSettingsData:ScriptableObject{
		public SceneData[] scenes;
	}
}
