namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	using UnityEngine.UI;
	/// <summary>
	/// 根据语言交换图片
	/// </summary>
	public class LanguageSwapImage:BaseMonoBehaviour{
		public Sprite spriteEN;
		public Sprite spriteCN;
		private Image _image;
	
		protected override void Awake() {
			base.Awake();
			_image=GetComponent<Image>();
			if(App.instance!=null){
				swapImageToLanguage(App.instance.language);
			}
		}
	
		protected override void Start(){
			base.Start();
			swapImageToLanguage(App.instance.language);
			App.instance.onChangeLanguage+=onChangeLanguage;
		}
	
		private void onChangeLanguage(Language language){
			swapImageToLanguage(language);
		}
	
		private void swapImageToLanguage(Language language){
			if(language==Language.EN){
				_image.sprite=spriteEN;
			}else if(language==Language.CN){
				_image.sprite=spriteCN;
			}
		}
	
		protected override void OnDestroy(){
			App.instance.onChangeLanguage-=onChangeLanguage;
			base.OnDestroy();
		}
	}

}