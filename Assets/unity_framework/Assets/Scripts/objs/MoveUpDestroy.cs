namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	using DG.Tweening;
	/// <summary>
	/// 向上移动一定的距离后销毁
	/// </summary>
	public class MoveUpDestroy:BaseMonoBehaviour{
	
		[Tooltip("向上移动的距离")]
		[Range(10,100)]
		[SerializeField]
		private float _moveUpDistance=40;
	
		[Tooltip("向上移动的持续时间")]
		[Range(0,100)]
		[SerializeField]
		private float _duration=1.0f;
	
		protected override void Start() {
			base.Start();
			transform.DOMoveY(transform.position.y+_moveUpDistance,_duration).onComplete=()=>{
				Destroy(gameObject);
			};
		}
	
		protected override void OnDestroy() {
			transform.DOKill();
			base.OnDestroy();
		}
	}

}