using UnityEditor;
public class TagsAndLayersImporter{
	public void import(string path,string projectImportSettingPath,string projectName){
		string sourceTagFilePath=path+"/ProjectSettings/TagManager.asset";
		string destTagFilePath=projectImportSettingPath+"/TagManager.asset";
		FileUtil2.copyFile(sourceTagFilePath,destTagFilePath,true);

		string destTagAssetPath="Assets/"+projectName+"/projectImportSetting/TagManager.asset";
		SerializedObject copyTagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagAssetPath));

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
				for(int i=1;i<len;i++){
					SerializedProperty sortingLayerElement=it.GetArrayElementAtIndex(i);
					SerializedProperty nameElement=sortingLayerElement.FindPropertyRelative("name");
					
					setSortingLayer(myTagManager,i,nameElement.stringValue);
				}
			}else if(it.name=="layers"){
				int len=it.arraySize;
				for(int i=8;i<len;i++){
					SerializedProperty layerElement=it.GetArrayElementAtIndex(i);
					setLayer(myTagManager,i,layerElement.stringValue);
				}
			}
		}

		FileUtil.DeleteFileOrDirectory(destTagFilePath);
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
