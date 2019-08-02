namespace UnityEngine{ 

	using UnityProjectImporter;

	public struct SortingLayer2{
		
		public static SortingLayer2[] layers{
			get{
				var list=ProjectImporter.instance.sortingLayersData.list;
				int len=list.Length;
				SortingLayer2[] layer2List=new SortingLayer2[len];
				for(int i=0;i<len;i++){
					var uSortingLayer=list[i];
					SortingLayer2 layer2=new SortingLayer2();
					layer2._id=(int)uSortingLayer.uniqueID;
					layer2._name=uSortingLayer.name;
					layer2._value=i;
				}
				return layer2List;
			}
		}

		private int _id;
		public int id { get=>_id; }
		
		private string _name;
		public string name { get=>_name; }

		private int _value;
		public int value { get=>_value; }

		public static int GetLayerValueFromID(int id){
			int layerValue=-1;
			var list=ProjectImporter.instance.sortingLayersData.list;
			int len=list.Length;
			for(int i=0;i<len;i++){
				if(list[i].uniqueID==id){
					layerValue=i;
					break;
				}
			}
			return layerValue;
		}
		
		public static int GetLayerValueFromName(string name){
			int layerValue=-1;
			var list=ProjectImporter.instance.sortingLayersData.list;
			int len=list.Length;
			for(int i=0;i<len;i++){
				if(list[i].name==name){
					layerValue=i;
					break;
				}
			}
			return layerValue;
		}
		
		public static string IDToName(int id){
			var list=ProjectImporter.instance.sortingLayersData.list;
			if(id>-1&&id<list.Length){
				return list[id].name;
			}
			return "<unknown layer>";
		}
		
		public static bool IsValid(int id){
			var list=ProjectImporter.instance.sortingLayersData.list;
			return id>-1&&id<list.Length;
		}
		
		public static int NameToID(string name){
			int id=-1;
			var list=ProjectImporter.instance.sortingLayersData.list;
			int len=list.Length;
			for(int i=0;i<len;i++){
				var uSortingLayer=list[i];
				if(uSortingLayer.name==name){
					id=(int)uSortingLayer.uniqueID;
					break;
				}
			}
			return id;
		}

		public static implicit operator SortingLayer2(SortingLayer sortingLayer){
			var sortingLayer2=new SortingLayer2();
			sortingLayer2._id=sortingLayer.id;
			sortingLayer2._name=sortingLayer.name;
			sortingLayer2._value=sortingLayer.value;
			return sortingLayer2;
		}
		
	}
}