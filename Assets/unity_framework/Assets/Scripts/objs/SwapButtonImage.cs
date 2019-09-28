namespace unity_framework{
	using UnityEngine;
	using UnityEngine.UI;
	
	/// <summary>
	/// 点击按钮时，切换按钮上的图片
	/// </summary>
	public class SwapButtonImage:BaseMonoBehaviour{
		[Tooltip("来回切换的两张图片")]
		public Sprite[] sprites;
		[Tooltip("切换的图片(未指定时自动从当前对象组件列表中获取)")]
		public Image image=null;
		[Tooltip("是否侦听点击按钮事件自动切换")]
		public bool isSwapOnClick=true;
		private Button _button;
		protected override void Awake() {
			base.Awake();
			
			_button=GetComponent<Button>();
			if(isSwapOnClick){
				_button.onClick.AddListener(onClick);
			}
			if(image==null){
				image=GetComponent<Image>();
			}
		}
	
		private void onClick(){
			if(image.sprite==sprites[0]){
				swapTo(1);
			}else{
				swapTo(0);
			}
		}
	
		public void	swapTo(int spriteId){
			image.sprite=sprites[spriteId];
		}
	
		protected override void OnDestroy() {
			if(isSwapOnClick){
				_button.onClick.RemoveListener(onClick);
			}
			base.OnDestroy();
		}
	}

}