namespace UnityProjectImporter{
	using UnityEditor;
	using UnityEngine;

	public class TagsAndLayersImporter:Importer{
		/// <summary>
		/// 导入项目的TagsAndLayers 
		/// </summary>
		/// <param name="path">需要导入TagsAndLayers的项目路径</param>
		/// <param name="projectImporterTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public override void import(string path,string projectImporterTempPath,string projectName){
			//TagManager.asset原来的位置
			string sourceTagFilePath=path+"/ProjectSettings/TagManager.asset";
			//TagManager.asset复制过来的位置
			string destTagFilePath=projectImporterTempPath+"/TagManager.asset";
			//复制TagManager.asset
			FileUtil2.copyFile(sourceTagFilePath,destTagFilePath,true);
			//加载并转换成SerializedObject
			SerializedObject copyTagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagFilePath));
			//加载当前项目的TagManager.asset并转换成SerializedObject
			SerializedObject myTagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);

			var it=copyTagManager.GetIterator();
			while (it.NextVisible(true)){
				if (it.name=="tags"){
					int len=it.arraySize;
					for(int i=0;i<len;i++){
						SerializedProperty tagElement=it.GetArrayElementAtIndex(i);
						UnityEditorInternal.InternalEditorUtility.AddTag(tagElement.stringValue);
						//addTag(tagElement.stringValue);
					}
				}else if(it.name=="m_SortingLayers"){
					int len=it.arraySize;

					string[] strings=new string[len];

					for(int i=0;i<len;i++){
						SerializedProperty sortingLayerElement=it.GetArrayElementAtIndex(i);
						SerializedProperty nameElement=sortingLayerElement.FindPropertyRelative("name");
						if(i>=1){
							setSortingLayer(myTagManager,i,nameElement.stringValue);
						}
						strings[i]=nameElement.stringValue;
					}

					SortingLayersData sortingLayersData= ScriptableObject.CreateInstance<SortingLayersData>();
					sortingLayersData.list=strings;
					AssetDatabase.CreateAsset(sortingLayersData,"Assets/ProjectImporter/Resources/"+projectName+"_sortingLayersData.asset");
				}else if(it.name=="layers"){
					int len=it.arraySize;

					string[] strings=new string[len];

					for(int i=0;i<len;i++){
						SerializedProperty layerElement=it.GetArrayElementAtIndex(i);
						if(i>=8){
							setLayer(myTagManager,i,layerElement.stringValue);
						}
						strings[i]=layerElement.stringValue;
					}

					SortingLayersData layersData= ScriptableObject.CreateInstance<SortingLayersData>();
					layersData.list=strings;
					AssetDatabase.CreateAsset(layersData,"Assets/ProjectImporter/Resources/"+projectName+"_layersData.asset");
				}
			}
			//删除复制过来的"TagManager.asset"
			AssetDatabase.DeleteAsset(destTagFilePath);
			AssetDatabase.Refresh();

		}

		private static void addTag(string tag){
			if(isHasTag(tag))return;
			SerializedObject tagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
			SerializedProperty it=tagManager.FindProperty("tags");

			int count=it.arraySize;
			it.InsertArrayElementAtIndex(count);
			SerializedProperty newElement=it.GetArrayElementAtIndex(count);
			newElement.stringValue=tag;//添加tag

			tagManager.ApplyModifiedProperties();
		}

		private static bool isHasTag(string tag){
			string[] tags=UnityEditorInternal.InternalEditorUtility.tags;
			int len=tags.Length;
			for (int i=0;i<len;i++){
				if(tags[i].Equals(tag)){
					return true;
				}
			}
			return false;
		}

		private static void setLayer(SerializedObject myTagManager,int index,string layer){
			if(string.IsNullOrEmpty(layer))return;

			SerializedProperty it=myTagManager.FindProperty("layers");

			int len=it.arraySize;
		
			SerializedProperty element=it.GetArrayElementAtIndex(index);
			element.stringValue="layer_"+index;//重命名Layer
			myTagManager.ApplyModifiedProperties();
		}

		private static void setSortingLayer(SerializedObject myTagManager,int index,string layer){
			SerializedProperty it=myTagManager.FindProperty("m_SortingLayers");

			int len=it.arraySize;
			if(index>=len)it.InsertArrayElementAtIndex(len);

			SerializedProperty element=it.GetArrayElementAtIndex(index);
			SerializedProperty nameElement=element.FindPropertyRelative("name");
			nameElement.stringValue="sortLayer_"+index;//重命名SortingLayer

			myTagManager.ApplyModifiedProperties();
		}
	
	}
}