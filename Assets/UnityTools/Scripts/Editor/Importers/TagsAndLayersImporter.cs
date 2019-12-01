namespace UnityTools{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using UnityEditor;
	using UnityEngine;
    using YamlDotNet.RepresentationModel;

    public class TagsAndLayersImporter:Importer{
		/// <summary>
		/// 导入项目的TagsAndLayers 
		/// </summary>
		/// <param name="path">需要导入TagsAndLayers的项目路径</param>
		/// <param name="currentProjectTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void import(string path,string currentProjectTempPath,string projectName){
			//需要导入的TagManager.asset的路径
			string settingsFilePath=path+"/ProjectSettings/TagManager.asset";

			StreamReader streamReader=new StreamReader(settingsFilePath,Encoding.UTF8);
			YamlStream yaml=new YamlStream();
			yaml.Load(streamReader);
			streamReader.Dispose();
			streamReader.Close();

			YamlNode rootNode=yaml.Documents[0].RootNode;
			YamlNode firstNode=rootNode["TagManager"];
			YamlSequenceNode tags=(YamlSequenceNode)firstNode["tags"];
			YamlSequenceNode layers=(YamlSequenceNode)firstNode["layers"];
			YamlSequenceNode sortingLayers=(YamlSequenceNode)firstNode["m_SortingLayers"];
			
			
			//当前项目的TagManager.asset的路径
			string myFilePath=ProjectImporterEditor.currentProjectPath+"/ProjectSettings/TagManager.asset";
			streamReader=new StreamReader(myFilePath,Encoding.UTF8);
			//记录头3行
			string[] myHeadLines={streamReader.ReadLine(),streamReader.ReadLine(),streamReader.ReadLine()};
			YamlStream myYaml=new YamlStream();
			myYaml.Load(streamReader);
			streamReader.Dispose();
			streamReader.Close();
			
			YamlNode myRootNode=myYaml.Documents[0].RootNode;
			YamlNode myFirstNode=myRootNode["TagManager"];
			//YamlSequenceNode myTags=(YamlSequenceNode)myFirstNode["tags"];
			YamlSequenceNode myLayers=(YamlSequenceNode)myFirstNode["layers"];
			YamlSequenceNode mySortingLayers=(YamlSequenceNode)myFirstNode["m_SortingLayers"];
			
			importTags(tags);
			importSortingLayers(sortingLayers,mySortingLayers,projectName);
			importLayers(layers,myLayers,projectName);

			//保存修改到当前项目的TagManager.asset
			StreamWriter streamWriter=new StreamWriter(myFilePath);
			for (int i=0;i<3;i++)streamWriter.WriteLine(myHeadLines[i]);
			myYaml.Save(streamWriter,false);
			streamWriter.Dispose();
			streamWriter.Close();
			//AssetDatabase.Refresh();
			
		}

		#region importTags
		private void importTags(YamlSequenceNode tags){
			foreach(YamlScalarNode tag in tags){
				UnityEditorInternal.InternalEditorUtility.AddTag(tag.Value);
				//addTag(tag.Value);
			}
		}

		/*private void addTag(string tag){
			if(isHasTag(tag))return;
			SerializedObject tagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty it=tagManager.FindProperty("tags");

			int count=it.arraySize;
			it.InsertArrayElementAtIndex(count);
			SerializedProperty newElement=it.GetArrayElementAtIndex(count);
			newElement.stringValue=tag;//添加tag

			tagManager.ApplyModifiedProperties();
		}*/

		private bool isHasTag(string tag){
			string[] tags=UnityEditorInternal.InternalEditorUtility.tags;
			int len=tags.Length;
			for (int i=0;i<len;i++){
				if(tags[i].Equals(tag)){
					return true;
				}
			}
			return false;
		}
		#endregion importTags

		#region importLayers
		private void importLayers(YamlSequenceNode layers,YamlSequenceNode myLayers,string projectName){
			List<string> strings=new List<string>();
			int i=0;
			foreach(YamlScalarNode layer in layers){
				if(i>=8){
					setLayer(myLayers,i,layer.Value);
				}
				strings.Add(layer.Value);
				i++;
			}
			var layersData= ScriptableObject.CreateInstance<LayersData>();
			layersData.list=strings.ToArray();
			string layersDataPath=ProjectImporterEditor.resourcePath+"/"+projectName+"_layersData.asset";
			AssetDatabase.CreateAsset(layersData,layersDataPath);
		}

		private void setLayer(YamlSequenceNode myLayers,int index,string layerValue){
			if(string.IsNullOrEmpty(layerValue))return;
			int i=0;
			foreach(YamlScalarNode myLayer in myLayers){
				if(i==index){
					myLayer.Value="layer_"+index;//重命名Layer
					break;
				}
				i++;
			}
		}
		#endregion importLayers

		#region importSortingLayers
		/// <summary>
		/// 导入指定项目的SortingLayers到当前项目
		/// </summary>
		/// <param name="sortingLayers"></param>
		/// <param name="mySortingLayers"></param>
		/// <param name="projectName"></param>
		private void importSortingLayers(YamlSequenceNode sortingLayers,YamlSequenceNode mySortingLayers,string projectName){
			int i=0;
			List<USortingLayer> uSortingLayers=new List<USortingLayer>();
			foreach(YamlMappingNode sortingLayer in sortingLayers){
				YamlScalarNode nameElement=(YamlScalarNode)sortingLayer["name"];
				YamlScalarNode uniqueIDElement=(YamlScalarNode)sortingLayer["uniqueID"];
				//YamlScalarNode lockedElement=(YamlScalarNode)sortingLayer["locked"];
				if(i>=1){
					setSortingLayerToCurrentProject(mySortingLayers,i,nameElement.Value);
				}

				var uSortingLayer=new USortingLayer();
				uSortingLayer.name=nameElement.Value;
				uSortingLayer.uniqueID=uint.Parse(uniqueIDElement.Value);
				uSortingLayers.Add(uSortingLayer);

				i++;
			}

			SortingLayersData sortingLayersData= ScriptableObject.CreateInstance<SortingLayersData>();
			sortingLayersData.list=uSortingLayers.ToArray();
			string sortingLayersDataPath=ProjectImporterEditor.resourcePath+"/"+projectName+"_sortingLayersData.asset";
			AssetDatabase.CreateAsset(sortingLayersData,sortingLayersDataPath);
		}

		/// <summary>
		/// 设置index指定的SortingLayer到当前项目
		/// </summary>
		/// <param name="mySortingLayers"></param>
		/// <param name="index"></param>
		/// <param name="layer"></param>
		private void setSortingLayerToCurrentProject(YamlSequenceNode mySortingLayers,int index,string layer){
			//计算当前项目SortingLayer的长度
			int mySortingLayersLen=0;
			foreach(var item in mySortingLayers){ mySortingLayersLen++; }
			//如果index超过当前项目的SortingLayers总数,则在尾部插入一个元素
			//if(index>=mySortingLayersLen)mySortingLayers.InsertArrayElementAtIndex(mySortingLayersLen);
			if(index>=mySortingLayersLen){
				var nSortingLayer=new YamlMappingNode();
				uint uniqueID=getSortingLayerUniqueID(mySortingLayers);
				nSortingLayer.Add("name",new YamlScalarNode("sortLayer_"+index));
				nSortingLayer.Add("uniqueID",new YamlScalarNode(uniqueID.ToString()));
				nSortingLayer.Add("locked",new YamlScalarNode("0"));
				mySortingLayers.Add(nSortingLayer);
			}
		}

		/// <summary>
		/// 返回一个在当前项目SortingLayers中唯一的ID
		/// </summary>
		/// <param name="sortingLayers">"m_SortingLayers"序列化属性</param>
		/// <returns></returns>
		private uint getSortingLayerUniqueID(YamlSequenceNode sortingLayers){
			uint id=0;
			while(true){
				id=(uint)Random.Range(0,uint.MaxValue);
				if(isUniqueSortingLayerID(id,sortingLayers)){
					break;
				}
			}
			return id;
		}

		/// <summary>
		/// 判断id在当前项目SortingLayers中是否唯一的
		/// </summary>
		/// <param name="sortingLayerID"></param>
		/// <param name="sortingLayers">"m_SortingLayers"序列化属性</param>
		/// <returns></returns>
		private bool isUniqueSortingLayerID(uint sortingLayerID,YamlSequenceNode sortingLayers){
			bool isUnique=true;
			int i=0;
			foreach(YamlMappingNode sortingLayer in sortingLayers){
				YamlScalarNode nameElement=(YamlScalarNode)sortingLayer["name"];
				YamlScalarNode uniqueIDElement=(YamlScalarNode)sortingLayer["uniqueID"];
				//YamlScalarNode lockedElement=(YamlScalarNode)sortingLayer["locked"];
				if(uint.Parse(uniqueIDElement.Value)==sortingLayerID){
					isUnique=false;
					break;
				}
				i++;
			}
			return isUnique;
		}
		#endregion importSortingLayers
	}
}