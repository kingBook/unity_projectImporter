namespace UnityEngine{
	using System.Collections.Generic;
	using UnityTools;

	public struct LayerMask2{
		public int value;

		public static int GetMask(params string[] layerNames){
			int result=0;
			//去除重复项
			List<string> layerNameList=new List<string>(layerNames);
			for (int i=0; i<layerNameList.Count;i++){
				for (int j=layerNameList.Count-1;j>i;j--){
					if (layerNameList[i]==layerNameList[j]){
						layerNameList.RemoveAt(j);
					}
				}
			}
			layerNames=layerNameList.ToArray();
			//
			string[] layers=ProjectImporter.instance.layersData.list;
			int layerNamesLen=layerNames.Length;
			int layersLen=layers.Length;
			for(int i=0;i<layerNamesLen;i++){
				for(int j=8;j<layersLen;j++){
					if(layerNames[i]==layers[j]){
						result+=1<<j;
						break;
					}
				}
			}
			return result;
		}

		public static string LayerToName(int layer){
			return ProjectImporter.instance.layersData.list[layer];
		}

		public static int NameToLayer(string layerName){
			int result=-1;
			string[] layers=ProjectImporter.instance.layersData.list;
			int len=layers.Length;
			for(int i=0;i<len;i++){
				if(layers[i]==layerName){
					result=i;
					break;
				}
			}
			return result;
		}

		public static implicit operator int(LayerMask2 mask){
			return mask.value;
		}
		public static implicit operator LayerMask2(int intVal){
			LayerMask2 mask=new LayerMask2();
			mask.value=intVal;
			return mask;
		}

		public static implicit operator LayerMask2(LayerMask mask){
			LayerMask2 mask2=new LayerMask2();
			mask2.value=mask.value;
			return mask2;
		}
		public static implicit operator LayerMask(LayerMask2 mask2){
			LayerMask mask=new LayerMask();
			mask.value=mask2.value;
			return mask;
		}
	}
}
