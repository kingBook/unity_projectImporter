namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// Transform工具类
	/// </summary>
	public static class TransformUtil{
		
		/// <summary>
		/// 返回一个Transform组件的子节点列表
		/// </summary>
		/// <param name="transform"></param>
		/// <returns></returns>
		public static Transform[] getTransformChildren(Transform transform){
			int childCount=transform.childCount;
			Transform[] children=new Transform[childCount];
			for(int i=0;i<childCount;i++){
				children[i]=transform.GetChild(i);
			}
			return children;
		}
	}

}