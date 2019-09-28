namespace unity_framework{
	
	using UnityEngine;
	/// <summary>
	/// 数学类
	/// </summary>
	public class Mathk{
		
		/// <summary>
		/// 将任意角度转换为[-180°,180°]，并返回转换后的角度
		/// </summary>
		/// <param name="rotation">需要转换的角度</param>
		/// <returns></returns>
		public static float getRotationTo180(float rotation){
			rotation%=360.0f;
			if     (rotation>180.0f)rotation-=360.0f;
			else if(rotation<-180.0f)rotation+=360.0f;
			return rotation;
		}
			
		/// <summary>
		/// 计算出目标角减当前角的差（取到达目标角度的最近旋转方向）,并返回这个差
		/// </summary>
		/// <param name="rotation">当前角度</param>
		/// <param name="targetRotation">目标角度</param>
		/// <returns></returns>
		public static float getRotationDifference(float rotation,float targetRotation){
			rotation=getRotationTo360(rotation);
			targetRotation=getRotationTo360(targetRotation);
			float offset=targetRotation-rotation;
			if(Mathf.Abs(offset)>180.0f){
				float reDir=offset>=0?-1:1;
				offset=reDir*(360.0f-Mathf.Abs(offset));
			}
			return offset;
		}
			
		/// <summary>
		/// 将任意角度转换为[0°,360°]的值,并返回转换后的值
		/// </summary>
		/// <param name="rotation">需要转换的角度</param>
		/// <returns></returns>
		public static float getRotationTo360(float rotation){
			rotation=getRotationTo180(rotation);
			if(rotation<0) rotation+=360;
			return rotation;
		}
	}

}