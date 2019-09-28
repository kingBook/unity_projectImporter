namespace unity_framework{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;
	
	/// <summary>
	/// 移动平台:单点触摸控制相机的旋转，两点触摸缩放和平移相机视野。
	/// <br>PC平台：鼠标左键控制相机的旋转，中键缩放相机视野，右键平移相机视野</br>
	/// </summary>
	public class CameraHandle:BaseMonoBehaviour{
		[Tooltip("缩放和旋转围绕的中心")]
		public Transform pivotTransform;
		[Tooltip("上下旋转限制的最小角度")]
		public int verticalAngleMin=10;
		[Tooltip("上下旋转限制的最大角度")]
		public int verticalAngleMax=60;
		[Tooltip("鼠标滚轮的速度倍数")]
		public float scrollWheelMultiple=10;
		[Tooltip("缩放时相机视野最小值")]
		public float fieldOfViewMin=10;
		[Tooltip("缩放时相机视野最大值")]
		public float fieldOfViewMax=100;
		[Tooltip("移动平台视野缩放的倍数")]
		public float fieldOfViewMultiple=0.1f;
	
		/// <summary>
		/// 平移 void(Vector3 velocity)
		/// </summary>
		public event Action<Vector3> onTranslateEvent;
		/// <summary>
		/// 旋转前 void(float h,float v)
		/// </summary>
		public event Action<float,float> onPreRotateEvent;
		/// <summary>
		/// 旋转 void(float h,float v)
		/// </summary>
		public event Action<float,float> onRotateEvent;
		/// <summary>
		/// 缩放 void(float velocity)
		/// </summary>
		public event Action<float> onZoomEvent;
	
		private Camera _camera;
		private float _oldDistance;
		private bool _isRotateBegin;
	
		/// <summary>
		/// 触摸点0/鼠标左键，在触摸/按下开始时是否接触UI
		/// </summary>
		private bool _isPointerOverUIOnBegan0;
		/// <summary>
		/// 触摸点1/鼠标右键，在触摸/按下开始时是否接触UI
		/// </summary>
		private bool _isPointerOverUIOnBegan1;
	
	
	
		protected override void Awake() {
			base.Awake();
			_camera=GetComponent<Camera>();
		}
	
		protected override void Start(){
			base.Start();
			
		}
	
		protected override void Update2() {
			base.Update2();
			
			if(Input.touchSupported){
				if(Input.touchCount==1){
					touchOneHandler();
				}else if(Input.touchCount==2){
					touchTwoHandler();
					_isRotateBegin=false;
				}else{
					_isRotateBegin=false;
				}
			}else{
				mouseHandler();
			}
		}
	
		private void touchOneHandler(){
			Touch touch0=Input.GetTouch(0);
			//接触开始时，触摸点0是否接触UI
			if(touch0.phase==TouchPhase.Began){
				_isPointerOverUIOnBegan0=EventSystem.current.IsPointerOverGameObject(touch0.fingerId);
			}
			//触摸点0在触摸开始时没有接触UI
			if(!_isPointerOverUIOnBegan0){
				float h=touch0.deltaPosition.x*0.5f;
				float v=touch0.deltaPosition.y*0.1f;
				if(!_isRotateBegin){
					_isRotateBegin=true;
					onPreRotateEvent?.Invoke(h,v);
				}
				//单点触摸上下左右旋转
				rotate(h,v);
			}
		}
	
		private void touchTwoHandler(){
			Touch touch0=Input.GetTouch(0);
			Touch touch1=Input.GetTouch(1);
			//接触开始时，触摸点0是否接触UI
			if(touch0.phase==TouchPhase.Began){
				_isPointerOverUIOnBegan0=EventSystem.current.IsPointerOverGameObject(touch0.fingerId);
			}
			//接触开始时，触摸点1是否接触UI
			if(touch1.phase==TouchPhase.Began){
				_isPointerOverUIOnBegan1=EventSystem.current.IsPointerOverGameObject(touch1.fingerId);
			}
			//两个触摸点在触摸开始时都没有接触UI
			if(!_isPointerOverUIOnBegan0&&!_isPointerOverUIOnBegan1){
				//两点触摸缩放视野
				float distance=Vector2.Distance(touch0.position,touch1.position);
				if(touch1.phase==TouchPhase.Began){
					_oldDistance=distance;
				}
				float offset=(distance-_oldDistance)*fieldOfViewMultiple;
				zoomFieldOfView(-offset);
				_oldDistance=distance;
				//两点触摸平移
				Vector2 translateVel=(touch0.deltaPosition+touch1.deltaPosition)*0.5f*-0.01f;
				translate(translateVel);
			}
		}
	
		private void mouseHandler(){
			//按下鼠标左键时，鼠标是否接触UI
			if(Input.GetMouseButtonDown(0)){
				_isPointerOverUIOnBegan0=EventSystem.current.IsPointerOverGameObject();
			}
			//按下鼠标右键时，鼠标是否接触UI
			if(Input.GetMouseButtonDown(1)){
				_isPointerOverUIOnBegan1=EventSystem.current.IsPointerOverGameObject();
			}
			//非移动设备按下鼠标左键旋转
			if(Input.GetMouseButton(0)){
				//鼠标按下左键时没有接触UI
				if(!_isPointerOverUIOnBegan0){
					
					float h=Input.GetAxis("Mouse X");
					float v=Input.GetAxis("Mouse Y");
					h*=10;
					v*=10;
	
					if(!_isRotateBegin){
						_isRotateBegin=true;
						onPreRotateEvent?.Invoke(h,v);
					}
					rotate(h,v);
				}
			}else{
				_isRotateBegin=false;
			}
			//非移动设备滚动鼠标中键缩放视野
			float scroll=scrollWheelMultiple*Input.GetAxis("Mouse ScrollWheel");
			zoomFieldOfView(scroll*10);
			//非移动设备按下鼠标右键平移
			if(Input.GetMouseButton(1)){
				//鼠标按下右键没有接触UI
				if(!_isPointerOverUIOnBegan1){
					float h=Input.GetAxis("Mouse X");
					float v=Input.GetAxis("Mouse Y");
					Vector2 translateVel=new Vector2(-h*0.1f,-v*0.1f);
					translate(translateVel*3);
				}
			}
		}
	
		/// <summary>
		/// 旋转
		/// </summary>
		private void rotate(float h,float v){
			//绕着pivot旋转Y轴，实现左右旋转
			_camera.transform.RotateAround(pivotTransform.position,Vector3.up, h);
			//绕着pivot旋转相机朝向的右侧轴向,实现上下旋转
			int cameraAngleX=(int)_camera.transform.rotation.eulerAngles.x;
			//限制最大速度，避免出错
			const float maxV=5;
			v=Mathf.Clamp(v,-maxV,maxV);
			onRotateEvent?.Invoke(h,v);
			//
			if(v>=0){
				//限制上下旋转最小角度
				if(cameraAngleX>verticalAngleMin){
					_camera.transform.RotateAround(pivotTransform.position,_camera.transform.right,-v);
				}
			}else{
				//限制上下旋转最大角度
				if(cameraAngleX<verticalAngleMax){
					_camera.transform.RotateAround(pivotTransform.position,_camera.transform.right,-v);
				}
			}
		}
	
		/// <summary>
		/// 缩放视野
		/// </summary>
		/// <param name="velocity">缩放视野速度向量</param>
		private void zoomFieldOfView(float velocity){
			if(velocity==0)return;
			onZoomEvent?.Invoke(velocity);
			_camera.fieldOfView=Mathf.Clamp(_camera.fieldOfView+velocity,fieldOfViewMin,fieldOfViewMax);
		}
	
		/// <summary>
		/// 平移
		/// </summary>
		/// <param name="velocity">平移速度向量</param>
		private void translate(Vector3 velocity){
			onTranslateEvent?.Invoke(velocity);
			_camera.transform.Translate(velocity);
		}
	
	}

}