namespace unity_framework{
	using UnityEngine;
	using UnityEngine.UI;
	/// <summary>
	/// 数字图片文本
	/// </summary>
	public class NumberImageText:BaseMonoBehaviour{
		[Tooltip("每一个Image表示一个数字位，最右是个位")]
		public Image[] images;
		[Tooltip("0-9的数字Sprite,用于切换")]
		public Sprite[] numSprites;
		[Tooltip("显示的数字")]
		public uint number=0;
		[Tooltip("在Update函数中更新显示的间隔<秒>")]
		public float updateInterval=0.4f;
	
		private float _time=0;
	
		protected override void Start(){
			base.Start();
			updateText();
		}
	
		protected override void Update2(){
			base.Update2();
			if(Time.time-_time>0.4f){//间隔指定的秒数更新
				_time=Time.time;
				updateText();
			}
		}
	
		/// <summary>
		/// 更新显示与number变量一致,一般不需要手动调用，默认会在Update函数按照指定的间隔更新
		/// </summary>
		public void updateText(){
			string countStr=number.ToString();
			int i=images.Length;
			int bitCount=countStr.Length;//表示：个、十、百、千、万
			//从向左遍历数字图片，当要显示的数字位数超过图片的位数，将不显示
			while(--i>=0){
				bitCount--;
				if(bitCount>=0){
					int bitValue=int.Parse(countStr[bitCount].ToString());//数字字符串某位的值
					images[i].sprite=numSprites[bitValue];
				}else{
					images[i].sprite=numSprites[0];
				}
			}
		}
	
	}

}