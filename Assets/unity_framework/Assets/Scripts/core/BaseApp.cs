namespace unity_framework{
	using System;
	using UnityEngine;
	
	public enum Language{AUTO,CN,EN}
		
	/// <summary>
	/// 整个应用程序的单例抽象类(基类)
	/// <br>子类的以下方法：FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject，</br>
	/// <br>将使用以下代替：FixedUpdate2、Update2、LateUpdate2、OnGUI2、OnRenderObject2。</br>
	/// </summary>
	public abstract class BaseApp<T>:BaseMonoBehaviour where T:class,new(){
		
		protected static T _instance;
		/// <summary>
		/// 应用程序的单例实例
		/// </summary>
		public static T instance{ get => _instance; }
	
		[Tooltip("用于调试其它场景需要调用该脚本，" +
		 "\n在Hierarchy中拖入该脚本所在的.unity文件时，" +
		 "\n不执行载入标题、其他场景等，将在代码中判定实现" +
		 "\n发布项目时必须为false。")
		]
		[SerializeField]
		private bool _isDebug=false;
	
		/// <summary>
		/// 改变语言事件
		/// </summary>
		public event Action<Language> onChangeLanguage;
		[Tooltip("AUTO:运行时根据系统语言决定是CN/EN " +
		 "\nCN:中文 " +
		 "\nEN:英文")
		]
		[SerializeField,SetProperty("language")]//此处使用SetProperty序列化setter方法，用法： https://github.com/LMNRY/SetProperty
		protected Language _language=Language.AUTO;
	
		[Tooltip("进度条")]
		[SerializeField]
		private Progressbar _progressbar=null;
	
		[Tooltip("文件加载器")]
		[SerializeField]
		private FileLoader _fileLoader=null;
	
		[Tooltip("场景加载器")]
		[SerializeField]
		private SceneLoader _sceneLoader=null;
	
		[Tooltip("更新管理器")]
		[SerializeField]
		private UpdateManager _updateManager=null;
	
		/// <summary>
		/// 暂停或恢复事件，在调用setPause(bool)时方法发出
		/// </summary>
		public event Action<bool> onPauseOrResume;
		private bool _isPause;
	
		private bool _isFirstOpen;
	
		protected override void Awake() {
			base.Awake();
			_instance=this as T;
	
			initFirstOpenApp();
	
			if(_language==Language.AUTO){
				initLanguage();
			}
		}
	
		private void initFirstOpenApp(){
			const string key="isFirstOpenApp";
			_isFirstOpen=PlayerPrefs.GetInt(key,1)==1;
			if(_isFirstOpen) {
				PlayerPrefs.SetInt(key,0);
				PlayerPrefs.Save();
			}
		}
	
		private void initLanguage(){
			bool isCN=Application.systemLanguage==SystemLanguage.Chinese;
			isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseSimplified;
			isCN=isCN||Application.systemLanguage==SystemLanguage.ChineseTraditional;
			_language=isCN?Language.CN:Language.EN;
			//改变语言事件
			onChangeLanguage?.Invoke(_language);
		}
	
		/// <summary>
		/// 设置暂停/恢复更新、物理模拟
		/// </summary>
		/// <param name="isPause">是否暂停</param>
		/// <param name="isSetPhysics">是否设置物理引擎</param>
		/// <param name="isSetVolume">是否设置音量</param>
		public void setPause(bool isPause,bool isSetPhysics=true, bool isSetVolume=true){
			if(_isPause==isPause)return;
			_isPause=isPause;
			if(isSetPhysics){
				//暂停或恢复3D物理模拟
				Physics.autoSimulation=!_isPause;
				//暂停或恢复2D物理模拟
				Physics2D.autoSimulation=!_isPause;
			}
			if(isSetVolume){
				AudioListener.pause=_isPause;
			}
			//发出事件
			onPauseOrResume?.Invoke(isPause);
		}
		
		protected override void OnDestroy(){
			base.OnDestroy();
			//不需要销毁_instance
			//_instance=null;
		}
	
		public bool isDebug{ get => _isDebug; }
		
		/// <summary>
		/// 应用程序的语言
		/// </summary>
		public Language language{
			get => _language;
			set{
				_language=value;
				onChangeLanguage?.Invoke(_language);
			}
		}
	
		/// <summary>
		/// 进度条
		/// </summary>
		public Progressbar progressbar{ get => _progressbar; }
	
		/// <summary>
		/// 文件加载器
		/// </summary>
		public FileLoader fileLoader{ get => _fileLoader; }
	
		/// <summary>
		/// 场景加载器(有进度条)
		/// </summary>
		public SceneLoader sceneLoader{ get => _sceneLoader; }
	
		/// <summary>
		/// 更新管理器
		/// </summary>
		public UpdateManager updateManager{ get => _updateManager; }
	
		/// <summary>
		/// 是否已暂停
		/// </summary>
		public bool isPause{ get => _isPause; }
	
		/// <summary>
		/// 是否第一次打开当前应用
		/// </summary>
		public bool isFirstOpen{ get => _isFirstOpen; }
	
	
	}

}