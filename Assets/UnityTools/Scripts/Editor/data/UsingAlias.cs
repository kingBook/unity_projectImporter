namespace UnityTools{
	using UnityEngine;
	using System.Collections;
	/// <summary>
	/// Using别名，如:"using Project = PC.MyCompany.Project;"
	/// </summary>
	public struct UsingAlias:IUsing{
		public SectionString name;
		public SectionString value;

		public string ToString(string fileString){
			return string.Format("name:{0} value:{1}",name.ToString(fileString),value.ToString(fileString));
		}
	}
}
