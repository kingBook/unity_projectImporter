namespace UnityProjectImporter{
	using UnityEngine;
	using System.Collections;
	using UnityEngine.SceneManagement;
	using UnityEngine.UI;
	/// <summary>
	/// 场景加载器
	/// </summary>
	public sealed class SceneLoader:MonoBehaviour{
		[Tooltip("进度条滑块")]
		public Image imageMid;
		[Tooltip("百分比文本框")]
		public Text txt;
		[Tooltip("场景加完成后，是否调用SceneManager.SetActiveScene(scene)激活场景")]
		public bool isSetActiveScene=true;

		private AsyncOperation _asyncOperation;

		private void Awake() {
			CanvasGroup canvasGroup=GetComponent<CanvasGroup>();
			canvasGroup.alpha=1.0f;

			gameObject.SetActive(false);
		}
		
		private void OnEnable() {
			SceneManager.sceneLoaded+=onSceneLoaded;
		}

		/// <summary>
		/// Additive模式同步加载场景
		/// </summary>
		/// <param name="sceneName"></param>
		public void load(string sceneName){
			load(sceneName,LoadSceneMode.Additive);
		}
		/// <summary>
		/// 同步加载场景
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="mode"></param>
		public void load(string sceneName,LoadSceneMode mode){
			SceneManager.LoadScene(sceneName,mode);
		}

		/// <summary>
		/// Additive模式异步加载场景，将显示进度条
		/// </summary>
		/// <param name="sceneName"></param>
		public void loadAsync(string sceneName){
			loadAsync(sceneName,LoadSceneMode.Additive);
		}
		/// <summary>
		/// 异步加载场景，将显示进度条
		/// </summary>
		/// <param name="sceneName"></param>
		/// <param name="mode"></param>
		public void loadAsync(string sceneName,LoadSceneMode mode){
			gameObject.SetActive(true);
			imageMid.fillAmount=0;
			StartCoroutine(loadSceneAsync(sceneName,mode));
		}

		IEnumerator loadSceneAsync(string sceneName,LoadSceneMode mode){
			_asyncOperation=SceneManager.LoadSceneAsync(sceneName,mode);
			_asyncOperation.completed+=onAsyncComplete;
			_asyncOperation.allowSceneActivation=false;
			while(!_asyncOperation.isDone){
				float progress=_asyncOperation.progress;
				if(progress>=0.9f){
					_asyncOperation.allowSceneActivation=true;
					imageMid.fillAmount=1.0f;
					txt.text="loading 100%...";
				}else{
					imageMid.fillAmount=progress;
					txt.text="loading "+Mathf.FloorToInt(progress*100)+"%...";
				}
				yield return null;
			}
		}

		private void onAsyncComplete(AsyncOperation asyncOperation){
			gameObject.SetActive(false);
			_asyncOperation.completed-=onAsyncComplete;
			_asyncOperation=null;
		}
		
		private void onSceneLoaded(Scene scene,LoadSceneMode mode){
			if(isSetActiveScene){
				SceneManager.SetActiveScene(scene);
			}
			gameObject.SetActive(false);
		}
		
		private void OnDisable() {
			SceneManager.sceneLoaded-=onSceneLoaded;
		}

		private void OnDestroy(){
			if(_asyncOperation!=null){
				_asyncOperation.completed-=onComplete;
			}
		}

	}
}
