namespace unity_framework{
	using System.Collections.Generic;
	/// <summary>
	/// 游戏类
	/// <br>管理游戏全局变量、本地数据、场景切换。</br>
	/// <br>不访问关卡内的对象</br>
	/// </summary>
	public sealed class Game:BaseMonoBehaviour{
		
		protected override void Start() {
			base.Start();
			if(!App.instance.isDebug){
				gotoTitleScene();
			}
		}
	
		public void gotoTitleScene(){
			App.instance.sceneLoader.load("Scenes/title");
		}
	
		public void gotoLevelScene(){
			App.instance.sceneLoader.loadAsync("Scenes/level");
		}
	
		protected override void OnDestroy() {
			base.OnDestroy();
		}
	
	
	}

}