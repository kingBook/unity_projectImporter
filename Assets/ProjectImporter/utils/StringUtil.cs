namespace UnityProjectImporter{
	/// <summary>
	/// 字符串低效方法优化
	/// <br>具体描述： https://docs.unity3d.com/Manual/BestPracticeUnderstandingPerformanceInUnity5.html (Inefficient built-in string APIs部分)</br>
	/// </summary>
	public class StringUtil{
		public static bool endsWith(string a, string b) {
			int ap = a.Length - 1;
			int bp = b.Length - 1;

			while (ap >= 0 && bp >= 0 && a [ap] == b [bp]) {
				ap--;
				bp--;
			}
			return (bp < 0 && a.Length >= b.Length) || 

					(ap < 0 && b.Length >= a.Length);
			}

		public static bool startsWith(string a, string b) {
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
	}
}
