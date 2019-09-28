namespace unity_framework{
	using DG.Tweening;
	using UnityEngine;
	using UnityEngine.UI;
	/// <summary>
	/// 从指定的起始位置移动到当前位置
	/// </summary>
	public class MoveFromOnCanvas:BaseMonoBehaviour{
		[Tooltip("运动的起始位置(Canvas设计分辨率下的AnchoredPosition)")]
		public Vector2 from;
		[Tooltip("运动持续时间")]
		[Range(0,10)]
		public float duration=1.5f;
	
		/// <summary>
		/// void(MoveFromOnCanvas target)
		/// </summary>
		public event System.Action<MoveFromOnCanvas> onCompleteEvent;
	
		protected override void Start() {
			base.Start();
			RectTransform rectTransform=(RectTransform)transform;
			Vector2 recordPos=rectTransform.anchoredPosition;
			rectTransform.anchoredPosition=from;
	
			rectTransform.DOAnchorPos(recordPos,duration).onComplete=onComplete;
		}
	
		private void onComplete() {
			onCompleteEvent?.Invoke(this);
		}
	
		protected override void OnDestroy() {
			transform.DOKill();
			base.OnDestroy();
		}
	}

}