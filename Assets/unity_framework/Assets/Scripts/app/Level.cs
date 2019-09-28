namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// 关卡类
	/// <br>管理关卡内的对象。</br>
	/// </summary>
	public sealed class Level:BaseMonoBehaviour{
		private Game _game;
	
		protected override void Start() {
			base.Start();
			_game=App.instance.game;
		}
	
		public void vectory(){
			
		}
	
		public void failure(){
			
	
		}
	
	
		protected override void OnDestroy() {
			base.OnDestroy();
		}
	
	}

}