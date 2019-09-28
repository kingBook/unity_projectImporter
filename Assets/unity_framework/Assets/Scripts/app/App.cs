namespace unity_framework{
	using UnityEngine;
	/// <summary>
	/// 整个应用程序的单例类
	/// <br>此类的以下方法：FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject，</br>
	/// <br>将使用以下代替：FixedUpdate2、Update2、LateUpdate2、OnGUI2、OnRenderObject2。</br>
	/// </summary>
	public sealed class App:BaseApp<App>{
		
		private Game _game;
	
		
		protected override void Awake(){
			base.Awake();
			var gameObject=new GameObject("Game");
			_game=gameObject.AddComponent<Game>();
			_game.transform.parent=transform;
		}
		public Game game{ get => _game; }
	}
	

}