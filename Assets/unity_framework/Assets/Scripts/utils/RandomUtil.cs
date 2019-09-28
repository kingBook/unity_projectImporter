namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	using System.Collections.Generic;
	/// <summary>
	/// 随机工具类
	/// </summary>
	public static class RandomUtil{
		/// <summary>
		/// 返回一个指定长度的随机int数组，数组元素范围是在[min,max)区间内(包括min,排除max)不重复的整数。
		/// <br>注意：参数length的取值范围必须在[1,max-min]区间内。</br>
		/// <br>小于1时自动取值：1。</br>
		/// <br>大于max-min时自动取值：max-min。</br>
		/// </summary>
		public static int[] getRandomUniqueIntList(int min,int max,int length){
			int sourceLength=max-min;
			length=Mathf.Clamp(length,1,sourceLength);
	
			int[] results=new int[length];
			
			List<int> sourceList=new List<int>(sourceLength);
			int i;
			for(i=0;i<sourceLength;i++){
				sourceList.Add(min+i);
			}
	
			int randomIndex;
			for(i=0;i<length;i++){
				randomIndex=Random.Range(0,sourceList.Count);
				results[i]=sourceList[randomIndex];
				sourceList.RemoveAt(randomIndex);
			}
			return results;
		}
	
	}

}