using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace UnityTools{
	public class TimeImporter:Importer{
		/// <summary>
		/// 导入项目的TimeSettings
		/// </summary>
		/// <param name="path">需要导入TimeSettings的项目路径</param>
		/// <param name="currentProjectTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void Import(string path,string currentProjectTempPath,string projectName){
			//需要导入的TimeManager.asset的路径
			string settingsFilePath=path+"/ProjectSettings/TimeManager.asset";

			StreamReader streamReader=new StreamReader(settingsFilePath,Encoding.UTF8);
			YamlStream yaml=new YamlStream();
			yaml.Load(streamReader);
			streamReader.Dispose();
			streamReader.Close();

			YamlNode rootNode=yaml.Documents[0].RootNode;
			YamlMappingNode firstNode=(YamlMappingNode)rootNode["TimeManager"];

			TimeData timeData=ScriptableObject.CreateInstance<TimeData>();
			foreach(var item in firstNode){
				var keyNode=(YamlScalarNode)item.Key;
				var valueNode=(YamlScalarNode)item.Value;
				if(keyNode.Value=="Fixed Timestep"){
					timeData.fixedTimestep=float.Parse(valueNode.Value);
				}else if(keyNode.Value=="Maximum Allowed Timestep"){
					timeData.maximumAllowedTimestep=float.Parse(valueNode.Value);
				}else if(keyNode.Value=="m_TimeScale"){
					timeData.timeScale=float.Parse(valueNode.Value);
				}else if(keyNode.Value=="Maximum Particle Timestep"){
					timeData.maximumParticleTimestep=float.Parse(valueNode.Value);
				}
			}

			AssetDatabase.CreateAsset(timeData,ProjectImporterEditor.resourcePath+"/"+projectName+"_timeData.asset");
			AssetDatabase.Refresh();
		}
	}
}
