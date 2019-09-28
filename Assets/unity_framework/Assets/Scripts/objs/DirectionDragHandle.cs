namespace unity_framework{
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;
	/// <summary>
	/// 方向手柄(拖动中心的滑块控制方向)
	/// <br>通过angleNormal属性,获取方向角度力，x、y值范围[-1,1]</br>
	/// </summary>
	public class DirectionDragHandle:ScrollRect{
		private readonly float idleAlpha=0.5f;
		private readonly float activeAlpha=1.0f;
		private float _radius=0f;
		private Vector2 _angleNormal;
		private Vector2 _initPos;
		private int _fingerId=-1;
		private RectTransform _rt;
		private CanvasGroup _canvasGroup;
	
		protected override void Awake() {
			base.Awake();
			_rt=transform as RectTransform;
			_canvasGroup=GetComponent<CanvasGroup>();
		}
	
		protected override void Start(){
	        base.Start();
	        //计算摇杆块的半径
	        _radius=_rt.sizeDelta.x*0.5f;
			_initPos=_rt.anchoredPosition;
			
			_canvasGroup.alpha=idleAlpha;
	    }
	
		public override void OnBeginDrag(PointerEventData eventData){
			base.OnBeginDrag(eventData);
			_canvasGroup.alpha=activeAlpha;
		}
	
		public override void OnDrag (PointerEventData eventData){
	        base.OnDrag(eventData);
			var contentPostion=content.anchoredPosition;
			if (contentPostion.magnitude>_radius){
				contentPostion=contentPostion.normalized*_radius;
				SetContentAnchoredPosition(contentPostion);
			}
			_angleNormal=contentPostion.normalized;
	    }
	
		public override void OnEndDrag(PointerEventData eventData){
			base.OnEndDrag(eventData);
			_angleNormal=Vector2.zero;
			_canvasGroup.alpha=idleAlpha;
		}
	
		private void Update(){
			if(Input.touchSupported){
				Touch[] touchs=Input.touches;
				foreach(Touch touch in touchs){
					if(_fingerId==-1){
						if(RectTransformUtility.RectangleContainsScreenPoint(_rt,touch.position)){
							if(touch.phase==TouchPhase.Began){
								if(touch.position.x>_initPos.x&&touch.position.y>_initPos.y){
									moveHandleToPos(touch.position);
									_canvasGroup.alpha=activeAlpha;
									_fingerId=touch.fingerId;
								}
							}
						}
					}else if(touch.fingerId==_fingerId){
						if(touch.phase==TouchPhase.Ended){
							_fingerId=-1;
							_rt.anchoredPosition=_initPos;
							_canvasGroup.alpha=idleAlpha;
						}
					}
				}
			}else{
				if(Input.GetMouseButtonDown(0)){
					Vector2 mousePos=Input.mousePosition;
					if(RectTransformUtility.RectangleContainsScreenPoint(_rt,mousePos)){
						if(mousePos.x>_initPos.x&&mousePos.y>_initPos.y){
							moveHandleToPos(mousePos);
							_canvasGroup.alpha=activeAlpha;
						}
					}
				}else if(Input.GetMouseButtonUp(0)){
					_rt.anchoredPosition=_initPos;
					_canvasGroup.alpha=idleAlpha;
				}
			}
		}
	
		private void moveHandleToPos(Vector2 pos){
			CanvasScaler canvasScaler=GetComponentInParent<CanvasScaler>();
	
			//屏幕分辨率与设计分辨率的缩放因子
			float scaleX=Screen.width/canvasScaler.referenceResolution.x;
			float scaleY=Screen.height/canvasScaler.referenceResolution.y;
	
			//加权平均值
			float averageValue=scaleX*(1-canvasScaler.matchWidthOrHeight)+scaleY*(canvasScaler.matchWidthOrHeight);
	
			pos/=averageValue;
	
			pos-=_rt.sizeDelta*0.5f;
			Vector2 offset=pos-_rt.offsetMin;
	
			_rt.offsetMin=pos;
			_rt.offsetMax=_rt.offsetMax+offset;
		}
	
		public Vector2 angleNormal{ get=>_angleNormal;}
	
	}

}