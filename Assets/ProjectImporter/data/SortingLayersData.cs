namespace UnityProjectImporter{ 
	using UnityEngine;

	[System.Serializable]
	public struct USortingLayer{
		public string name;
		public uint uniqueID;
	}

	public class SortingLayersData:ScriptableObject{
		public USortingLayer[] list;
	}
}
