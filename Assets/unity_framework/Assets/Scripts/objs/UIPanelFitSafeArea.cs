namespace unity_framework{
	using UnityEngine;
	/// <summary>
	/// Canvas下的Panel适应刘海屏
	/// </summary>
	public class UIPanelFitSafeArea:BaseMonoBehaviour{
		[Tooltip("如果true，将取屏幕的宽度的0.9进行测试")]
		[SerializeField,SetProperty("isTest")]//此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
		private bool _isTest;
	
		private Rect _safeArea;
	    private Rect _lastSafeArea;
	    private RectTransform _panel;
	
		protected override void Awake(){
			base.Awake();
			_panel=GetComponent<RectTransform>();
			setSafeArea();
	    }
	
		private void setSafeArea(){
			if(_isTest){
				_safeArea=new Rect(0.0f,0.0f,Screen.width*0.9f,Screen.height);//测试：取屏幕宽度的0.9
			}else{
				_safeArea=Screen.safeArea;
			}
			refresh(_safeArea);
		}
	
		protected override void Start(){
			base.Start();
			refresh(_safeArea);
	    }
	
		protected override void Update2() {
			base.Update2();
			refresh(_safeArea);
	    }
	
	    private void refresh(Rect r){
	        if(_lastSafeArea==r)return;
	        _lastSafeArea=r;
	        //
	        //Debug.LogFormat("safeArea.position:{0}, safeArea.size:{1}",r.position,r.size);
	        //Debug.LogFormat("anchorMin:{0},anchorMax:{1}",_panel.anchorMin,_panel.anchorMax);
	        Vector2 anchorMin=r.position;
	        Vector2 anchorMax=r.position+r.size;
	        //anchorMin(左上角)、anchorMax(右下角)表示在屏幕上的百分比位置,在屏幕内的取值范围是[0,1]
	        anchorMin.x/=Screen.width;
	        anchorMin.y/=Screen.height;
	        anchorMax.x/=Screen.width;
	        anchorMax.y/=Screen.height;
	        _panel.anchorMin=anchorMin;
	        _panel.anchorMax=anchorMax;
	       //Debug.LogFormat("anchorMin:{0},anchorMax:{1}",_panel.anchorMin,_panel.anchorMax);
	        //Debug.Log("=====================================================================");
	    }
	
		public bool isTest{
			get => _isTest;
			set{
				_isTest=value;
				if(Application.isPlaying){
					setSafeArea();
				}
			}
		}
	}

}