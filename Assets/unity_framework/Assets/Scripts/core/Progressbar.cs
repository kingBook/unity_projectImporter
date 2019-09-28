namespace unity_framework{
	using UnityEngine;
	using UnityEngine.UI;
	
	public class Progressbar:BaseMonoBehaviour{
	
		[Tooltip("进度条滑块")]
		[SerializeField]
		private Image _imageMid=null;
	
		[Tooltip("百分比文本框")]
		[SerializeField]
		private Text _text=null;
	
		/// <summary>
		/// 设置显示的进度
		/// </summary>
		/// <param name="ratio">范围：[0,1]</param>
		public void setProgress(float ratio){
			_imageMid.fillAmount=Mathf.Clamp01(ratio);
		}
	
		/// <summary>
		/// 设置进度条上的文本
		/// </summary>
		/// <param name="textString">显示的字符串</param>
		public void setText(string textString){
			_text.text=textString;
		}
	}

}