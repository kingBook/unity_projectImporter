using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityTools{
	/// <summary>
	/// 场景加载器
	/// </summary>
	public sealed class SceneLoader:MonoBehaviour{
		
		[Tooltip("场景加完成后，是否调用SceneManager.SetActiveScene(scene)设置为激活场景")]
		public bool isActiveSceneOnLoaded=true;

		[Tooltip("进度条")]
		[SerializeField]
		private Progressbar m_progressbar=null;

		private AsyncOperation m_asyncOperation;

		private void Awake() {
			
		}

		private void OnEnable() {
			SceneManager.sceneLoaded+=OnSceneLoaded;
		}

		/// <summary>
		/// Additive模式同步加载场景
		/// </summary>
		/// <param name="sceneName">场景在BuildSettings窗口的路径或名称</param>
		public void Load(string sceneName){
			Load(sceneName,LoadSceneMode.Additive);
		}
		/// <summary>
		/// 同步加载场景
		/// </summary>
		/// <param name="sceneName">场景在BuildSettings窗口的路径或名称</param>
		/// <param name="mode">加载模式</param>
		public void Load(string sceneName,LoadSceneMode mode){
			SceneManager.LoadScene(sceneName,mode);
			//为了能够侦听场景加载完成时设置为激活场景,所以激活
			gameObject.SetActive(true);
			m_progressbar.gameObject.SetActive(true);
			m_progressbar.SetProgress(1.0f);
		}

		/// <summary>
		/// Additive模式异步加载场景，将显示进度条
		/// </summary>
		/// <param name="sceneName">场景在BuildSettings窗口的路径或名称</param>
		public void LoadAsync(string sceneName){
			LoadAsync(sceneName,LoadSceneMode.Additive);
		}
		/// <summary>
		/// 异步加载场景，将显示进度条
		/// </summary>
		/// <param name="sceneName">场景在BuildSettings窗口的路径或名称</param>
		/// <param name="mode">加载模式,默认为：LoadSceneMode.Additive</param>
		public void LoadAsync(string sceneName,LoadSceneMode mode){
			gameObject.SetActive(true);
			m_progressbar.gameObject.SetActive(true);
			m_progressbar.SetProgress(0.0f);
			StartCoroutine(LoadSceneAsync(sceneName,mode));
		}

		IEnumerator LoadSceneAsync(string sceneName,LoadSceneMode mode){
			m_asyncOperation=SceneManager.LoadSceneAsync(sceneName,mode);
			m_asyncOperation.completed+=OnAsyncComplete;
			m_asyncOperation.allowSceneActivation=false;
			while(!m_asyncOperation.isDone){
				float progress=m_asyncOperation.progress;
				if(progress>=0.9f){
					m_asyncOperation.allowSceneActivation=true;
					m_progressbar.SetProgress(1.0f);
					m_progressbar.SetText("loading 100%...");
				}else{
					m_progressbar.SetProgress(progress);
					m_progressbar.SetText("loading "+Mathf.FloorToInt(progress*100)+"%...");
				}
				yield return null;
			}
		}

		private void OnAsyncComplete(AsyncOperation asyncOperation){
			gameObject.SetActive(false);
			m_progressbar.gameObject.SetActive(false);
			m_asyncOperation.completed-=OnAsyncComplete;
			m_asyncOperation=null;
		}

		private void OnSceneLoaded(Scene scene,LoadSceneMode mode){
			if(isActiveSceneOnLoaded){
				SceneManager.SetActiveScene(scene);
			}
			gameObject.SetActive(false);
			m_progressbar.gameObject.SetActive(false);
		}

		private void OnDisable() {
			SceneManager.sceneLoaded-=OnSceneLoaded;
		}

		private void OnDestroy(){
			if(m_asyncOperation!=null){
				m_asyncOperation.completed-=OnAsyncComplete;
			}
		}

	}
}
