namespace UnityTools{
	using UnityEngine;
	using System.Collections;

	public struct CSharpClass{

		/// <summary>
		/// 类所在的命名空间
		/// </summary>
		public CSharpNameSpace nameSpace;
		
		/// <summary>
		/// <para>单个名称或名称加尖括号</para>
		/// <para><see cref="Segment"/>/<see cref="WordAngleBrackets"/></para>
		/// <para>如："App"、"App&lt;BaseApp&gt;"</para>
		/// </summary>
		public IString name;

		/// <summary>
		/// <para>继承的类</para>
		/// <para><see cref="Segment"/>/<see cref="WordAngleBrackets"/></para>
		/// <para>如："xx.xx.xx","ClassA","IName","IName&lt;xx&gt;","HelloD &lt;xxx.xx.HelloF&lt;object,object,object&gt;&gt;"</para>
		/// </summary>
		public IString baseClass;

		/// <summary>
		/// 实现的接口
		/// 如："xx.xx.xx","ClassA","IName","IName&lt;xx&gt;","HelloD &lt;xxx.xx.HelloF&lt;object,object,object&gt;&gt;"
		/// </summary>
		public IString[] implementInterfaces;

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