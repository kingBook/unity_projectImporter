using System.IO;

namespace UnityTools {
	/// <summary>
	/// .cs文件
	/// </summary>
	public class CSharpFile:IAlignObject{
		
		public FileInfo fileInfo;
		public string fileString;
		public Segment[] lineComments;
		public Segment[] blockComments;
		public Segment[] strings;
		
		#region SubObjects
		/// <summary>.cs文件内所有命名空间声明外的using</summary>
		public IUsing[] usings;
		public CSharpNameSpace[] nameSpaces;
		public CSharpClass[] classes;
		public CSharpStruct[] structs;
		public CSharpInterface[] interfaces;
		public CSharpEnum[] enums;
		public CSharpDelegate[] delegates;
		#endregion
	}
}