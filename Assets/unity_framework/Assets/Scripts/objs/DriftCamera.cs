namespace unity_framework{
	using UnityEngine;
	/// <summary>
	/// 控制相机跟随目标
	/// </summary>
	public class DriftCamera:BaseMonoBehaviour{
		
		[System.Serializable]
		public class AdvancedOptions{
			public bool updateCameraInFixedUpdate=true;
			public bool updateCameraInUpdate;
			public bool updateCameraInLateUpdate;
			[Space]
	        [Tooltip("是否检测穿过遮挡物并处理")]
			public bool isCheckCrossObs=true;
	        [Tooltip("遮挡物LayerMask")]
			public LayerMask obsLayerMask=-1;
		}
		
		[System.Serializable]
		public enum PositionMode{
			Top,//0
			
			TopLeftForward,
			TopForward,
			TopRightForward,
			TopRight,
			TopRightBack,
			TopBack,
			TopLeftBack,
			TopLeft,//8
			
			LeftForward,
			Forward,
			RightForward,
			Right,
			RightBack,
			Back,
			LeftBack,
			Left,//16
			
			Bottom,//17
			
			BottomLeftForward,
			BottomForward,
			BottomRightForward,
			BottomRight,
			BottomRightBack,
			BottomBack,
			BottomLeftBack,
			BottomLeft//25
		}
		public static readonly Vector3[] positionModeVerties=new Vector3[]{
			new Vector3(0,1,0),
			
			new Vector3(-1,1,1),
			new Vector3(0,1,1),
			new Vector3(1,1,1),
			new Vector3(1,1,0),
			new Vector3(1,1,-1),
			new Vector3(0,1,-1),
			new Vector3(-1,1,-1),
			new Vector3(-1,1,0),
			
			new Vector3(-1,0,1),
			new Vector3(0,0,1),
			new Vector3(1,0,1),
			new Vector3(1,0,0),
			new Vector3(1,0,-1),
			new Vector3(0,0,-1),
			new Vector3(-1,0,-1),
			new Vector3(-1,0,0),
			
			new Vector3(0,-1,0),
			
			new Vector3(-1,-1,1),
			new Vector3(0,-1,1),
			new Vector3(1,-1,1),
			new Vector3(1,-1,0),
			new Vector3(1,-1,-1),
			new Vector3(0,-1,-1),
			new Vector3(-1,-1,-1),
			new Vector3(-1,-1,0)
		};
	
		public float smoothing=6.0f;
		[Tooltip("相机朝向的目标点")]
		public Transform lookAtTarget;
		[Tooltip("相机相对于目标点的单位化位置")]
		public Vector3 originPositionNormalized=new Vector3(0.2f,0.68f,-1.0f);
		[Tooltip("相机与目标点的距离")]
		public float distance=4.0f;
		public AdvancedOptions advancedOptions;
		
		protected override void Start(){
			base.Start();
		}
	
		protected override void FixedUpdate2(){
			base.FixedUpdate2();
			if(advancedOptions.updateCameraInFixedUpdate){
				updateCamera();
			}
		}
	
		protected override void Update2(){
			base.Update2();
			if(advancedOptions.updateCameraInUpdate){
				updateCamera();
			}
		}
	
		protected override void LateUpdate2(){
			base.LateUpdate2();
			if(advancedOptions.updateCameraInLateUpdate){
				updateCamera();
			}
		}
		
		private void updateCamera(){
			if(lookAtTarget==null)return;
	        //计算相机原点
			Vector3 offset=originPositionNormalized*distance;
			offset=lookAtTarget.rotation*offset;
			Vector3 positionTarget=lookAtTarget.position+offset;
	        //遮挡检测
			if(advancedOptions.isCheckCrossObs){
				checkCrossObsViewField(ref positionTarget);
			}
	        //移动相机
			transform.position=Vector3.Lerp(transform.position,positionTarget,Time.deltaTime*smoothing);
	        //旋转相机朝向
			transform.LookAt(lookAtTarget);
		}
		
	    
	    /// <summary>
	    /// 检测遮挡并处理
	    /// </summary>
		private void checkCrossObsViewField(ref Vector3 positionTarget){
			if(!isCrossObs(positionTarget))return;
			for(int i=0;i<17;i++){
	            //取一个相机测试点检测是否遮挡
				Vector3 normalized=positionModeVerties[i];
				Vector3 offset=normalized*distance;
				offset=lookAtTarget.rotation*offset;
				Vector3 testPosTarget=lookAtTarget.position+offset;
	            //球形插值运算取测试点检测是否遮挡
				float t=0.0f;
				for(int j=0;j<5;j++){
					t+=0.2f;
					Vector3 checkPos=Vector3.Slerp(positionTarget,testPosTarget,t);
					if(!isCrossObs(checkPos)){
	                    //没有被遮挡，返回该测试点
						positionTarget=checkPos;
						return;
					}
				}
				
			}
		}
		
	    /// <summary>
	    /// 是否被遮挡
	    /// </summary>
		private bool isCrossObs(Vector3 positionTarget){
			Ray ray=new Ray(positionTarget,lookAtTarget.position-positionTarget);
			float maxDistance=Vector3.Distance(lookAtTarget.position,positionTarget);
			
			const int bufferLen=50;
			RaycastHit[] buffer=new RaycastHit[bufferLen];
			Physics.RaycastNonAlloc(ray,buffer,maxDistance,advancedOptions.obsLayerMask);
			
			for(int i=0;i<bufferLen;i++){
				RaycastHit raycastHit=buffer[i];
				if(raycastHit.collider!=null){
					return true;
				}
			}
			return false;
		}
		
	}

}