namespace UnityTools {
	/// <summary>
	/// 命名空间
	/// </summary>
    public class CSharpNameSpace:IAlignObject{
		
		public static readonly CSharpNameSpace None=new CSharpNameSpace();
	    
		public CSharpNameSpace parent;
		public DotPath name;

		public Segment content;
		/// <summary>命名空间括号内的using</summary>
		public IUsing[] usings;
		public CSharpNameSpace[] nameSpaces;
		public CSharpClass[] classes;
		public CSharpStruct[] structs;
		public CSharpInterface[] interfaces;
		public CSharpEnum[] enums;
		public CSharpDelegate[] delegates;
		
	}
}
