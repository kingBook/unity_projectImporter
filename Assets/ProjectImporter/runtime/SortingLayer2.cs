namespace UnityProjectImporter{ 
	using UnityEngine;
	using System.Collections;

	public struct SortingLayer2{
		private static SortingLayer[] _layers=null;
		//需要改
		public static SortingLayer[] layers{
			get => _layers;
		}

		public int id { get; }
		
		public string name { get; }
		public int value { get; }
		//需要改
		public static int GetLayerValueFromID(int id){
			return SortingLayer.GetLayerValueFromID(id);
		}
		//需要改
		public static int GetLayerValueFromName(string name){
			return SortingLayer.GetLayerValueFromName(name);
		}
		//需要改
		public static string IDToName(int id){
			return SortingLayer.IDToName(id);
		}
		//需要改
		public static bool IsValid(int id){
			return SortingLayer.IsValid(id);
		}
		//需要改
		public static int NameToID(string name){
			return SortingLayer.NameToID(name);
		}
	}
}