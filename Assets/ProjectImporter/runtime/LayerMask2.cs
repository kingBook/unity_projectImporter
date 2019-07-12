using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LayerMask2{
		public int value;

		//需要改
		public static int GetMask(params string[] layerNames){
			return LayerMask.GetMask(layerNames);
		}

		//需要改
		public static string LayerToName(int layer){
			return LayerMask.LayerToName(layer);
		}

		//需要改
		public static int NameToLayer(string layerName){
			return LayerMask.NameToLayer(layerName);
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
