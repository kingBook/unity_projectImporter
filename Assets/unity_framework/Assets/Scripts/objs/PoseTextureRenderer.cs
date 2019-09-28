namespace unity_framework{
	using UnityEngine;
	/// <summary>
	/// 目标展示纹理渲染器
	/// </summary>
	public class PoseTextureRenderer:BaseMonoBehaviour{
		/// <summary>
		/// 用于拍照的相机
		/// </summary>
	    [Tooltip("用于拍照的相机")]
	    public Camera targetCamera;
		/// <summary>
		/// 相机所拍摄的对象的渲染器组件
		/// </summary>
	    [Tooltip("相机所拍摄的对象的渲染器组件")]
	    public Renderer targetRenderer;
		/// <summary>
		/// 用于渲染目标的纹理
		/// </summary>
		[Tooltip("用于渲染目标的纹理")]
		public RenderTexture targetTexture;
		[Tooltip("在Start函数是否设置相机看向目标")]
		public bool isLookToTargetOnStart=false;
	
		private bool _targetCameraActiveRecord;
		private bool _targetRendererActiveRecord;
	
		protected override void Start() {
			base.Start();
			if(isLookToTargetOnStart){
				setCameraLookToTarget();
			}
		}
	
		/// <summary>
		/// 将对目标拍照渲染
		/// </summary>
		/// <returns></returns>
		public void render(){
			//记录激活状态
			_targetCameraActiveRecord=targetCamera.gameObject.activeSelf;
			_targetRendererActiveRecord=targetRenderer.gameObject.activeSelf;
			//激活
			targetCamera.gameObject.SetActive(true);
			targetRenderer.gameObject.SetActive(true);
			//设置相机看向目标，并渲染
	        setCameraLookToTarget();
	        targetCamera.targetTexture=targetTexture;
			targetCamera.Render();
			targetCamera.targetTexture=null;
	        //恢复激活
			targetCamera.gameObject.SetActive(_targetCameraActiveRecord);
			targetRenderer.gameObject.SetActive(_targetRendererActiveRecord);
	    }
	    
		/// <summary>
		/// 设置相机的参数看向目标包围盒
		/// </summary>
	    public void setCameraLookToTarget(){
	        //相机旋转朝向目标对象
	        Bounds bounds=targetRenderer.bounds;
			Vector3 boundsCenter=bounds.center;
	        targetCamera.transform.LookAt(boundsCenter);
	        //包围盒角点
	        Vector3[] points=FuncUtil.getBoundsCorners(boundsCenter,bounds.extents);
	        //所有角点投射到平面
	        Vector3 planeNormal=boundsCenter-targetCamera.transform.position;
	        FuncUtil.worldPointsToPlane(points,points.Length,planeNormal);
	        //平面中心
	        Vector3 planeCenter=Vector3.ProjectOnPlane(boundsCenter,planeNormal);
	        //取平面上各个点与平面中心的最大距离作为相机的视野矩形框大小
	        float halfHeight=getMaxDistanceToPlaneCenter(points,points.Length,planeCenter);
	        //相机与包围盒中心的距离(世界坐标为单位)
	        float distance=Vector3.Distance(boundsCenter,targetCamera.transform.position);
	        //得到视野大小
	        targetCamera.fieldOfView=Mathf.Atan2(halfHeight,distance)*Mathf.Rad2Deg*2;
	    }
	    
	    /// <summary>
	    /// 返回平面上各个点与平面中心的最大距离
	    /// </summary>
	    /// <param name="points">平面上的各个点</param>
	    /// <param name="pointCount">点数量</param>
	    /// <param name="planeCenter">平面中心</param>
	    /// <returns></returns>
	    private float getMaxDistanceToPlaneCenter(Vector3[] points,int pointCount,Vector3 planeCenter){
	        float maxDistance=float.MinValue;
	        for(int i=0;i<pointCount;i++){
	            var vertex=points[i];
	            float distance=Vector3.Distance(vertex,planeCenter);
	            if(distance>maxDistance)maxDistance=distance;
	        }
	        return maxDistance;
	    }
	}

}