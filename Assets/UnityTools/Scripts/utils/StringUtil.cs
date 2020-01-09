using System.Text.RegularExpressions;

namespace UnityTools{
	/// <summary>
	/// 字符串低效方法优化
	/// <br>具体描述： https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html (Inefficient built-in string APIs部分)</br>
	/// </summary>
	public static class StringUtil{
		public static bool EndsWith(string a, string b) {
			int ap = a.Length - 1;
			int bp = b.Length - 1;

			while (ap >= 0 && bp >= 0 && a [ap] == b [bp]) {
				ap--;
				bp--;
			}
			return (bp < 0 && a.Length >= b.Length) || 

					(ap < 0 && b.Length >= a.Length);
			}

		public static bool StartsWith(string a, string b) {
			int aLen = a.Length;
			int bLen = b.Length;
			int ap = 0; int bp = 0;

			while (ap < aLen && bp < bLen && a [ap] == b [bp]) {
				ap++;
				bp++;
			}

			return (bp == bLen && aLen >= bLen) || 

					(ap == aLen && bLen >= aLen);
		}

		/// <summary>
		/// 返回字符串尾部的数字字符，如果尾部没有数字时返回空字符串
		/// </summary>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string GetEndNumberString(string text){
			var regex=new Regex(@"\d",RegexOptions.RightToLeft);
		
			string numberString="";
			int i=text.Length;
			while(--i>=0){ 
				Match match=regex.Match(text,i,1);
				if(match.Success){
					numberString=match.Value+numberString;
				}else{
					break;
				}
			}
			return numberString;
		}
	}
}
