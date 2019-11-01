namespace UnityTools{
	using UnityEngine;
	using System.Collections;

	public struct CSharpClass{

		/// <summary>
		/// <para>类名称字段，不是单独的一个名称如："class App:BaseApp&lt;App&gt;{"中的"App"就存在多个，</para>
		/// <para>将记录"class"到"{"之间的所有类名称字段，nameWords[0]表示当前类的名称。</para>
		/// <para>（已跳过尖括号&lt;&gt;里的空格，每个字段长度都相等，都是同一个名称）</para>
		/// </summary>
		public SegmentString[] nameWords;

		/// <summary>
		/// xx.xx.xx,ClassA,IName,IName<xx>
		/// </summary>
		public SegmentString[] extends;

		//public CSharpProperty[] properties;
		//public CsharpEvent[] events;
		//public CSharpDelegate[] delegates;
		//public CSharpMethod[] methods;
		
	}
}