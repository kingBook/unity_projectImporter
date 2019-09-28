namespace unity_framework{
	using UnityEngine;
	/// <summary>
	/// 静音按钮，在Start()中:
	/// <br>AudioListener.volume<=0，SwapButtonImage.swapTo(1)，第二张图</br>
	/// <br>AudioListener.volume>0，SwapButtonImage.swapTo(0)，第一张图</br>
	/// </summary>
	[RequireComponent(typeof(SwapButtonImage))]
	public class ButtonMute:BaseMonoBehaviour{
		
		protected override void Start() {
			base.Start();
			SwapButtonImage swapButtonImage=GetComponent<SwapButtonImage>();
			swapButtonImage.swapTo(AudioListener.volume<=0?1:0);
		}
	
	}

}