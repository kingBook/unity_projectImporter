using UnityEngine;
using System.Collections;
using UnityProjectImporter;

public class TimeImporter:Importer{
	/// <summary>
	/// 导入项目的TimeSettings
	/// </summary>
	/// <param name="path">需要导入TimeSettings的项目路径</param>
	/// <param name="projectImporterTempPath">临时文件夹</param>
	/// <param name="projectName">需要导入项目名称</param>
	public override void import(string path,string projectImporterTempPath,string projectName){
		base.import(path,projectImporterTempPath,projectName);
	}
}
