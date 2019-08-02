namespace unity_tags{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	public class Main : MonoBehaviour{
		void Start(){
			int mask=LayerMask2.GetMask("yu","mao","gou","","ren","j49");
			Debug.Log(mask);
			Array arr=SortingLayer2.layers;
			for(int i=0;i<arr.Length;i++){
				SortingLayer2 layer=(SortingLayer2)arr.GetValue(i);
				Debug.Log(layer.name);
			}
		}
	}
	

}