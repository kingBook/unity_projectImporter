namespace UnityEngine{ 
	
	public struct SortingLayer2{
		
		public static SortingLayer[] layers{
			get => SortingLayer.layers;
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

		public static implicit operator SortingLayer2(SortingLayer sortingLayer){
			var sortingLayer2=new SortingLayer2();
			//sortingLayer2.value=sortingLayer.value;
			return sortingLayer2;
		}
		public static implicit operator SortingLayer(SortingLayer2 sortingLayer2){
			var sortingLayer=new SortingLayer();
			//sortingLayer.value=sortingLayer2.value;
			return sortingLayer;
		}
	}
}