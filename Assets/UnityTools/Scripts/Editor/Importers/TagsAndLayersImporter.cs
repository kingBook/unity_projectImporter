namespace UnityTools{
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
			//string assetPath=path+"/ProjectSettings/TagManager.asset";

			//TagManager.asset原来的位置
			string sourcePath=path+"/ProjectSettings/TagManager.asset";
			//TagManager.asset复制过来的位置
			string destPath=currentProjectTempPath+"/TagManager.asset";
			//复制TagManager.asset
			FileUtil2.copyFile(sourcePath,destPath,true);

			StreamReader streamReader=new StreamReader(destPath, Encoding.UTF8);
			YamlStream yaml=new YamlStream();
			yaml.Load(streamReader);
			//必须Dispose，否则导致无法删除复制过来的.asset
			streamReader.Dispose();

			YamlNode rootNode=yaml.Documents[0].RootNode;
			YamlNode firstNode=rootNode["TagManager"];
			YamlSequenceNode tags=(YamlSequenceNode)firstNode["tags"];
			YamlSequenceNode layers=(YamlSequenceNode)firstNode["layers"];
			YamlSequenceNode m_SortingLayers=(YamlSequenceNode)firstNode["m_SortingLayers"];
			
			importTags(tags);
			importSortingLayers(m_SortingLayers,myTagManager,projectName);
			/*
			//TagManager.asset原来的位置
			string sourcePath=path+"/ProjectSettings/TagManager.asset";
			//TagManager.asset复制过来的位置
			string destPath=tempPath+"/TagManager.asset";
			//复制TagManager.asset
			FileUtil2.copyFile(sourcePath,destPath,true);
			//加载并转换成SerializedObject
			SerializedObject copyTagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destPath));
			//加载当前项目的TagManager.asset并转换成SerializedObject
			SerializedObject myTagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			
			var it=copyTagManager.GetIterator();
			while (it.NextVisible(true)){
				if (it.name=="tags"){
					importTags(it);
				}else if(it.name=="m_SortingLayers"){
					importSortingLayers(it,myTagManager,projectName);
				}else if(it.name=="layers"){
					importLayers(it,myTagManager,projectName);
				}
			}
			//删除复制过来的"TagManager.asset"
			AssetDatabase.DeleteAsset(destPath);
			AssetDatabase.Refresh();*/
			
			FileUtil.DeleteFileOrDirectory(destPath);
		}

		#region importTags
		private void importTags(YamlSequenceNode tags){
			foreach(YamlScalarNode tag in tags){
				UnityEditorInternal.InternalEditorUtility.AddTag(tag.Value);
				//addTag(tag.Value);
			}
		}

		private void addTag(string tag){
			if(isHasTag(tag))return;
			SerializedObject tagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty it=tagManager.FindProperty("tags");

			int count=it.arraySize;
			it.InsertArrayElementAtIndex(count);
			SerializedProperty newElement=it.GetArrayElementAtIndex(count);
			newElement.stringValue=tag;//添加tag

			tagManager.ApplyModifiedProperties();
		}

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
		private void importLayers(SerializedProperty layers,SerializedObject myTagManager,string projectName){
			int len=layers.arraySize;

			string[] strings=new string[len];

			for(int i=0;i<len;i++){
				var layerElement=layers.GetArrayElementAtIndex(i);
				if(i>=8){
					setLayer(myTagManager,i,layerElement.stringValue);
				}
				strings[i]=layerElement.stringValue;
			}

			var layersData= ScriptableObject.CreateInstance<LayersData>();
			layersData.list=strings;
			string layersDataPath=ProjectImporterEditor.resourcePath+"/"+projectName+"_layersData.asset";
			AssetDatabase.CreateAsset(layersData,layersDataPath);
		}

		private void setLayer(SerializedObject myTagManager,int index,string layer){
			if(string.IsNullOrEmpty(layer))return;

			SerializedProperty it=myTagManager.FindProperty("layers");

			int len=it.arraySize;
		
			SerializedProperty element=it.GetArrayElementAtIndex(index);
			element.stringValue="layer_"+index;//重命名Layer
			myTagManager.ApplyModifiedProperties();
		}
		#endregion importLayers

		#region importSortingLayers
		/// <summary>
		/// 导入指定项目的SortingLayers到当前项目
		/// </summary>
		/// <param name="sortingLayers"></param>
		/// <param name="myTagManager"></param>
		/// <param name="projectName"></param>
		private void importSortingLayers(SerializedProperty sortingLayers,SerializedObject myTagManager,string projectName){
			int len=sortingLayers.arraySize;

			var uSortingLayers=new USortingLayer[len];
			for(int i=0;i<len;i++){
				var sortingLayerElement=sortingLayers.GetArrayElementAtIndex(i);
				var nameElement=sortingLayerElement.FindPropertyRelative("name");
				var uniqueIDElement=sortingLayerElement.FindPropertyRelative("uniqueID");
				if(i>=1){
					setSortingLayerToCurrentProject(myTagManager,i,nameElement.stringValue);
				}
				var uSortingLayer=new USortingLayer();
				uSortingLayer.name=nameElement.stringValue;
				uSortingLayer.uniqueID=(uint)uniqueIDElement.intValue;//这里SerializedProperty不支持uintValue读取，所以只能读intValue再转换
				uSortingLayers[i]=uSortingLayer;
			}

			SortingLayersData sortingLayersData= ScriptableObject.CreateInstance<SortingLayersData>();
			sortingLayersData.list=uSortingLayers;
			string sortingLayersDataPath=ProjectImporterEditor.resourcePath+"/"+projectName+"_sortingLayersData.asset";
			AssetDatabase.CreateAsset(sortingLayersData,sortingLayersDataPath);
		}

		/// <summary>
		/// 设置index指定的SortingLayer到当前项目
		/// </summary>
		/// <param name="myTagManager"></param>
		/// <param name="index"></param>
		/// <param name="layer"></param>
		private void setSortingLayerToCurrentProject(SerializedObject myTagManager,int index,string layer){
			var mySortingLayers=myTagManager.FindProperty("m_SortingLayers");
			//如果index超过当前项目的SortingLayers总数,则在尾部插入一个元素
			int len=mySortingLayers.arraySize;
			if(index>=len)mySortingLayers.InsertArrayElementAtIndex(len);
			//需要设置的SortingLayer
			var mySortingLayerElement=mySortingLayers.GetArrayElementAtIndex(index);
			//改名
			var nameElement=mySortingLayerElement.FindPropertyRelative("name");
			nameElement.stringValue="sortLayer_"+index;//重命名SortingLayer
			//设置一个唯一的id
			var uniqueIDElement=mySortingLayerElement.FindPropertyRelative("uniqueID");
			if(!isUniqueSortingLayerID((uint)uniqueIDElement.intValue,index,mySortingLayers)){
				uniqueIDElement.intValue=getSortingLayerUniqueID(mySortingLayers,index);
			}
			//应用修改
			myTagManager.ApplyModifiedProperties();
		}

		/// <summary>
		/// 返回一个在当前项目SortingLayers中唯一的ID
		/// </summary>
		/// <param name="sortingLayers">"m_SortingLayers"序列化属性</param>
		/// <param name="curIndex">当前层在sortingLayers中的下标</param>
		/// <returns></returns>
		private int getSortingLayerUniqueID(SerializedProperty sortingLayers,int curIndex){
			int id=0;
			while(true){
				//uniqueID:取得区间[0,int.MaxValue]。
				//注意：在编辑里设置的最大值是uint.MaxValue,
				//但是SerializedProperty只支持设置intValue,所有这里选择int.MaxValue
				id=Random.Range(0,int.MaxValue);
				if(isUniqueSortingLayerID((uint)id,curIndex,sortingLayers)){
					break;
				}
			}
			return id;
		}

		/// <summary>
		/// 判断id在当前项目SortingLayers中是否唯一的
		/// </summary>
		/// <param name="sortingLayerID"></param>
		/// <param name="curIndex">当前层在sortingLayers中的下标</param>
		/// <param name="sortingLayers">"m_SortingLayers"序列化属性</param>
		/// <returns></returns>
		private bool isUniqueSortingLayerID(uint sortingLayerID,int curIndex,SerializedProperty sortingLayers){
			bool isUnique=true;
			int i=sortingLayers.arraySize;
			while(--i>=0){
				if(i==curIndex)continue;
				var element=sortingLayers.GetArrayElementAtIndex(i);
				var uniqueIDElement=element.FindPropertyRelative("uniqueID");
				//这里SerializedProperty不支持uintValue读取，所以只能读intValue再转换
				if((uint)uniqueIDElement.intValue==sortingLayerID){
					isUnique=false;
					break;
				}
			}
			return isUnique;
		}
		#endregion importSortingLayers
	}
}