using System;
using UnityEngine;

namespace UnityTools{
	[Serializable]
	public struct USortingLayer{
		public string name;
		public uint uniqueID;
	}

	public class SortingLayersData:ScriptableObject{
		public USortingLayer[] list;
	}
}
