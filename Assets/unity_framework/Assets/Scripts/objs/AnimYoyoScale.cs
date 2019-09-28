namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	using DG.Tweening;
	/// <summary>
	/// Yoyo缩放动画
	/// </summary>
	public class AnimYoyoScale:BaseMonoBehaviour{
	
		public float endScale=1.05f;
		public float duration=1;
	
		protected override void Start() {
			base.Start();
			transform.DOScale(endScale,duration).SetLoops(-1,LoopType.Yoyo);
		}
	}

}