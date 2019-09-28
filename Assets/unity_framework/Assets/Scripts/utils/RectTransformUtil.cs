namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// RectTransform工具类
	/// </summary>
	public class RectTransformUtil{
		
	/// <summary>
	/// 返回指定RectTransform屏幕坐标矩形
	/// </summary>
	/// <param name="rectTransform">指定的RectTransform</param>
	/// <param name="canvasLocalScale">Canvas的缩放量</param>
	/// <returns></returns>
	public static Rect getScreenRect(RectTransform rectTransform,Vector3 canvasLocalScale){
		Rect rect=rectTransform.rect;
		//根据Canvas缩放
		rect.width*=canvasLocalScale.x;
		rect.height*=canvasLocalScale.y;
		rect.position*=canvasLocalScale;
		//计算矩形左下角
		Vector2 leftBottom=(Vector2)rectTransform.position+rect.position;
		rect.position=leftBottom;
		return rect;
	}
	}

}