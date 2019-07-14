namespace UnityProjectImporter{
	using UnityEditor;
	using UnityEngine;

	public class PhysicsImporter{
		/// <summary>
		/// 导入项目的Physics
		/// </summary>
		/// <param name="path">需要导入Physics的项目路径</param>
		/// <param name="projectImporterTempPath">临时文件夹</param>
		/// <param name="projectName">需要导入项目名称</param>
		public void import(string path,string projectImporterTempPath,string projectName){
			//DynamicsManager.asset原来的位置
			string sourceTagFilePath=path+"/ProjectSettings/DynamicsManager.asset";
			//DynamicsManager.asset复制过来的位置
			string destTagFilePath=projectImporterTempPath+"/DynamicsManager.asset";
			//复制DynamicsManager.asset
			FileUtil2.copyFile(sourceTagFilePath,destTagFilePath,true);
			//加载并转换成SerializedObject
			string destTagAssetPath=projectImporterTempPath+"/DynamicsManager.asset";
			SerializedObject copyTagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath(destTagAssetPath));
			//加载当前项目的DynamicsManager.asset并转换成SerializedObject
			SerializedObject myTagManager=new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/DynamicsManager.asset")[0]);

			var it=copyTagManager.GetIterator();
			while (it.NextVisible(true)){
				Debug.Log(it.name);
				/*if (it.name=="tags"){
					int len=it.arraySize;
					for(int i=0;i<len;i++){
						SerializedProperty tagElement=it.GetArrayElementAtIndex(i);
						
						//addTag(tagElement.stringValue);
					}
				}*/
			}
			//删除复制过来的"DynamicsManager.asset"
			FileUtil.DeleteFileOrDirectory(destTagFilePath);
			AssetDatabase.Refresh();

		}
	}

}