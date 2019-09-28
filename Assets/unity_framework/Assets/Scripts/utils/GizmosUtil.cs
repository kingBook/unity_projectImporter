namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// Gizmos工具类
	/// </summary>
	public static class GizmosUtil{
		
		/// <summary>
		/// 根据路径点数组画线
		/// </summary>
		/// <param name="vertices">路径点数组</param>
		public static void drawPath(Vector3[] vertices){
	        int len=vertices.Length;
	        for (int i=0;i<len;i++){
	            int nexti=(i+1)%len;
	            Gizmos.DrawLine(vertices[i],vertices[nexti]);
	        }
	    }
	
		/// <summary>
		/// 用一个球体线框画一个点
		/// </summary>
		/// <param name="point">点位置</param>
		/// <param name="radius">球体线框半径</param>
		public static void drawPoint(Vector3 point,float radius=0.02f){
			Gizmos.DrawWireSphere(point,radius);
		}
	
		/// <summary>
		/// 画包围盒
		/// </summary>
		/// <param name="bounds">包围盒</param>
		public static void drawBounds(Bounds bounds){
			Gizmos.DrawWireCube(bounds.center,bounds.size);
		}
	}

}