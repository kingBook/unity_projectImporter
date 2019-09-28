namespace unity_framework{
	using DG.Tweening;
	using UnityEngine;
	/// <summary>
	/// Yoyo动画
	/// </summary>
	public class AnimationYoyo:BaseMonoBehaviour{
		[Tooltip("运动方向")]
		[Range(-180,180)]
		public float angle=90;
		[Tooltip("运动距离")]
		[Range(1,100)]
		public float distance=2;
		[Tooltip("时间(决定运动的快慢)")]
		[Range(0.1f,3.0f)]
		public float duration=0.8f;
	
		protected override void Start(){
			base.Start();
	
			Vector3 pos=transform.position;
	
			Vector3 offset=new Vector3(Mathf.Cos(angle*Mathf.Deg2Rad)*distance,
									   Mathf.Sin(angle*Mathf.Deg2Rad)*distance,
								       0);
	
			transform.position=pos+offset;
			Vector3 endPos=pos+offset*-1;
	
			transform.DOMove(endPos,duration,false).SetLoops(-1,LoopType.Yoyo);
		}
	}

}