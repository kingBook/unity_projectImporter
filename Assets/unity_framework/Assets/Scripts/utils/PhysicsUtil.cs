namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	
	public class PhysicsUtil{
		/// <summary>
		/// 返回离射线原点最近的RaycastHit,如果没有找到将返回new RaycastHit()
		/// </summary>
		/// <param name="ray">射线</param>
		/// <param name="layerMask">用于射线计算的LayerMask，如：LayerMask2.GetMask("ItemModel")。</param>
		/// <returns></returns>
		public static RaycastHit getClosestRaycastHit(Ray ray,int layerMask){
			RaycastHit result=new RaycastHit();
			RaycastHit[] hits=Physics.RaycastAll(ray,Mathf.Infinity,layerMask);
			float minDistance=float.MaxValue;
			int len=hits.Length;
			for(int i=0;i<len;i++){
				RaycastHit hit=hits[i];
				if(hit.distance<minDistance){
					minDistance=hit.distance;
					result=hit;
				}
			}
			return result;
		}
	
		/// <summary>
		/// 返回射线投射到一个世界坐标包围盒的碰撞信息。
		/// <br>RaycastHit.collider!=null表示射线与包围盒发生碰撞。</br>
		/// </summary>
		/// <param name="ray">射线</param>
		/// <param name="worldBounds">世界坐标包围盒</param>
		/// <returns></returns>
		public static RaycastHit getRaycastBounds(Ray ray,Bounds worldBounds){
			var gameObject=new GameObject();
			gameObject.transform.position=worldBounds.center;
			var boxCollider=gameObject.AddComponent<BoxCollider>();
			boxCollider.size=worldBounds.size;
			RaycastHit hitInfo;
			boxCollider.Raycast(ray,out hitInfo,Mathf.Infinity);
			Object.Destroy(gameObject);
			return hitInfo;
		}
	
		/// <summary>
		/// 返回包围盒与碰撞器是否相交
		/// </summary>
		/// <param name="bounds">包围盒</param>
		/// <param name="collider">碰撞器</param>
		/// <returns></returns>
		public static bool getBoundsIntersectsCollider<T>(Bounds bounds,T collider)where T:Collider{
			return bounds.Intersects(collider.bounds);
		}
	
		/// <summary>
		/// 返回包围盒与碰撞器列表是否有相交
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="bounds">包围盒</param>
		/// <param name="colliders">碰撞器列表</param>
		/// <returns></returns>
		public static bool getBoundsIntersectsColliders<T>(Bounds bounds,T[] colliders)where T:Collider{
			int len=colliders.Length;
			for(int i=0;i<len;i++){
				if(getBoundsIntersectsCollider(bounds,colliders[i])){
					return true;
				}
			}
			return false;
		}
		
	}

}