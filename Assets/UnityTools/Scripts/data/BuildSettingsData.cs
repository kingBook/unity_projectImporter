using System;
using UnityEngine;

namespace UnityTools{
	[Serializable]
	public struct SceneData{
		public bool enabled;
		public string path;
	}

	public class BuildSettingsData:ScriptableObject{
		public SceneData[] scenes;
	}
}
