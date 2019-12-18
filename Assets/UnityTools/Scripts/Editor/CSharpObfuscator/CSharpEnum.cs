namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// 枚举
	/// </summary>
	public struct CSharpEnum{
		
		/// <summary>
		/// 所在的命名空间
		/// </summary>
		public CSharpNameSpace nameSpace;
        
        /// <summary>
		/// <para>单词</para>
		/// <para>如："xxx"</para>
		/// </summary>
		public IString name;

		/// <summary>
		/// 基类型,声明其成员的类型
		/// 如：byte、sbyte、int、uint等
		/// </summary>
		public IString baseType;



	}
}