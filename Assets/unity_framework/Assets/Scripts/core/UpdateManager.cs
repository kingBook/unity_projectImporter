namespace unity_framework{
	using System.Collections.Generic;
	using UnityEngine;
	/// <summary>
	/// 更新管理器
	/// <br>统一调用FixedUpdate、Update、LateUpdate、OnGUI、OnRenderObject</br>
	/// <br>解决在压力状态下引起的效率低下问题，</br>
	/// <br>具体描述:https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity8.html （Update managers部分）</br>
	/// </summary>
	public sealed class UpdateManager:MonoBehaviour{
		private List<IUpdate> _list=new List<IUpdate>();
		private bool _isPause;
	
		private void Start() {
			App.instance.onPauseOrResume+=onPauseOrResume;
		}
	
		private void onPauseOrResume(bool isPause){
			_isPause=isPause;
		}
	
		private void FixedUpdate(){
			if(_isPause)return;
			int i=_list.Count;
			while(--i>=0){
				_list[i].FixedUpdate();
			}
		}
		private void Update(){
			if(_isPause)return;
			int i=_list.Count;
			while(--i>=0){
				_list[i].Update();
			}
		}
		private void LateUpdate(){
			if(_isPause)return;
			int i=_list.Count;
			while(--i>=0){
				_list[i].LateUpdate();
			}
		}
		private void OnGUI(){
			if(_isPause)return;
			int i=_list.Count;
			while(--i>=0){
				_list[i].OnGUI();
			}
		}
		private void OnRenderObject(){
			if(_isPause)return;
			int i=_list.Count;
			while(--i>=0){
				_list[i].OnRenderObject();
			}
		}
		
		public void add(IUpdate item){
			//if(_list.Contains(item))return;//这个方法非常慢
			_list.Insert(0,item);
		}
	
		public void remove(IUpdate item){
			_list.Remove(item);
		}
	
		public void clear(){
			_list.Clear();
		}
	
		private void OnDestroy() {
			App.instance.onPauseOrResume-=onPauseOrResume;
		}
	}

}