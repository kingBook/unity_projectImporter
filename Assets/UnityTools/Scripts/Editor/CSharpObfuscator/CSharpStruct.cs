namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// 结构体
	/// </summary>
	public struct CSharpStruct{
		
		/// <summary>
		/// 所在的命名空间
		/// </summary>
		public CSharpNameSpace nameSpace;
        
        /// <summary>
		/// <para>单词+尖括号/单词</para>
		/// <para><see cref="Segment"/>/<see cref="WordAngleBrackets"/></para>
		/// <para>如："xxx&lt;...&gt;","xxx"</para>
		/// </summary>
		public IString name;

		/// <summary>
		/// 实现的接口
		/// 如："xxx.xxx.xxx&lt;...&gt;","xxx&lt;...&gt;","xxx.xxx.xxx","xxx"
		/// </summary>
		public IString[] implements;

		/// <summary>
		/// 泛型约束列表
		/// </summary>
		public CSharpGenericConstraint[] genericConstraints;	


		
		//public CSharpProperty[] properties;
		//public CsharpEvent[] events;
		//public CSharpDelegate[] delegates;
		//public CSharpMethod[] methods;
	}
}
