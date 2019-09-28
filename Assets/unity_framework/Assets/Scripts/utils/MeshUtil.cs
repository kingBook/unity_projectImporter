namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// 网格工具类
	/// </summary>
	public class MeshUtil{
		/// <summary>
		/// 返回射线命中的子网格索引号
		/// <br>如果没有子网格或未成功匹配则返回-1</br>
		/// </summary>
		/// <param name="mesh"></param>
		/// <param name="hitInfo"></param>
		/// <returns></returns>
		public static int getRaycastHitSubMeshIndex(Mesh mesh,RaycastHit hitInfo){
			//命中三角形的顶点索引
			int[] triangles=mesh.triangles;
			int triangleIndex=hitInfo.triangleIndex;
			int i0=triangles[triangleIndex*3];
			int i1=triangles[triangleIndex*3+1];
			int i2=triangles[triangleIndex*3+2];
			
			int subMeshCount=mesh.subMeshCount;
			for(int i=0;i<subMeshCount;i++){
				var indices=mesh.GetIndices(i);
				int vertexCount=indices.Length;
				
				for(int j=0;j<vertexCount;j+=3){
					int j0=indices[j];
					int j1=indices[j+1];
					int j2=indices[j+2];
					//如果子网格的索引与命中三角形匹配，则表示是命中的子网格
					if(i0==j0&&i1==j1&&i2==j2){
						return i;
					}
				}
			}
			return -1;
		}
	
		/// <summary>
		/// 返回子网格的本地坐标包围盒
		/// </summary>
		/// <param name="mesh">网格</param>
		/// <param name="subMeshIndex">子网格索引号</param>
		/// <returns></returns>
		public static Bounds getSubMeshLocalBounds(Mesh mesh,int subMeshIndex){
			Vector3[] vertices=mesh.vertices;
	
			int[] indices=mesh.GetIndices(subMeshIndex);
			uint indexCount=mesh.GetIndexCount(subMeshIndex);
	
			Vector3[] subMeshVertices=new Vector3[indexCount];
			for(int i=0;i<indexCount;i++){
				subMeshVertices[i]=vertices[indices[i]];
			}
	
			Mesh tempMesh=new Mesh();
			tempMesh.vertices=subMeshVertices;
			tempMesh.RecalculateBounds();
	
			return tempMesh.bounds;
		}
	
		/// <summary>
		/// 返回子网格的世界坐标包围盒
		/// </summary>
		/// <param name="mesh">网格</param>
		/// <param name="subMeshIndex">子网格索引号</param>
		/// <param name="meshTransform">网格所在游戏对象的Transform组件</param>
		/// <returns></returns>
		public static Bounds getSubMeshWorldBounds(Mesh mesh,int subMeshIndex,Transform meshTransform){
			Bounds bounds=getSubMeshLocalBounds(mesh,subMeshIndex);
			bounds.center=meshTransform.TransformPoint(bounds.center);
			return bounds;
		}
	}

}