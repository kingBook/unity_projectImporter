namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// 根据当前应用程序的语言激活/吊销列表中的GameObject
	/// </summary>
	public class LanguageSwitcher:BaseMonoBehaviour{
		[Tooltip("英文时，需要激活的GameObject列表")]
		public GameObject[] enList;
		[Tooltip("中文时，需要激活的GameObject列表")]
		public GameObject[] cnList;
	
		protected override void Awake(){
			base.Awake();
			if(App.instance){
				activeWithLanguage(App.instance.language);
			}
		}
	
		protected override void Start(){
			base.Start();
			activeWithLanguage(App.instance.language);
			App.instance.onChangeLanguage+=onChangeLanguage;
		}
	
		private void activeWithLanguage(Language language){
			if(language==Language.AUTO)return;
			GameObject[] activeList=null;
			GameObject[] deactiveList=null;
			if(language==Language.EN){
				activeList=enList;
				deactiveList=cnList;
			}else if(language==Language.CN){
				activeList=cnList;
				deactiveList=enList;
			}
	
			int i=activeList.Length;
			while(--i>=0){
				activeList[i].SetActive(true);
			}
	
			i=deactiveList.Length;
			while(--i>=0){
				deactiveList[i].SetActive(false);
			}
		}
		
		private void onChangeLanguage(Language language){
			activeWithLanguage(language);
		}
	
		protected override void OnDestroy(){
			App.instance.onChangeLanguage-=onChangeLanguage;
			base.OnDestroy();
		}
	}

}