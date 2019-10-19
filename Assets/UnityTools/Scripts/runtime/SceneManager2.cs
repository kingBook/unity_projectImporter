namespace UnityEngine.SceneManagement {
	using System;
	using UnityEngine;
    using UnityTools;

    public class SceneManager2{
		public static Scene GetSceneByName(string name){
			name=getBuildSettingsScenePath(name,false);
			return SceneManager.GetSceneByName(name);
		}

		public static Scene GetSceneByPath(string scenePath){
			scenePath=getBuildSettingsScenePath(scenePath);
			return SceneManager.GetSceneByPath(scenePath);
		}


		public static void LoadScene(string sceneName,LoadSceneMode mode){
			sceneName=getBuildSettingsScenePath(sceneName);
			SceneManager.LoadScene(sceneName,mode);
		}
		
		public static void LoadScene(string sceneName){
			sceneName=getBuildSettingsScenePath(sceneName);
			SceneManager.LoadScene(sceneName);
		}
		
		public static Scene LoadScene(string sceneName,LoadSceneParameters parameters){
			sceneName=getBuildSettingsScenePath(sceneName);
			return SceneManager.LoadScene(sceneName,parameters);
		}
		
		public static void LoadScene(int sceneBuildIndex,LoadSceneMode mode){
			SceneManager.LoadScene(sceneBuildIndex,mode);
		}
		
		public static void LoadScene(int sceneBuildIndex){
			SceneManager.LoadScene(sceneBuildIndex);
		}
		
		public static Scene LoadScene(int sceneBuildIndex,LoadSceneParameters parameters){
			return SceneManager.LoadScene(sceneBuildIndex,parameters);
		}


		
		public static AsyncOperation LoadSceneAsync(string sceneName,LoadSceneParameters parameters){
			sceneName=getBuildSettingsScenePath(sceneName);
			return SceneManager.LoadSceneAsync(sceneName,parameters);
		}
		
		public static AsyncOperation LoadSceneAsync(string sceneName){
			sceneName=getBuildSettingsScenePath(sceneName);
			return SceneManager.LoadSceneAsync(sceneName);
		}
		
		public static AsyncOperation LoadSceneAsync(string sceneName,LoadSceneMode mode){
			sceneName=getBuildSettingsScenePath(sceneName);
			return SceneManager.LoadSceneAsync(sceneName,mode);
		}
		
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex){
			return SceneManager.LoadSceneAsync(sceneBuildIndex);
		}
		
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex,LoadSceneParameters parameters){
			return SceneManager.LoadSceneAsync(sceneBuildIndex,parameters);
		}
		
		public static AsyncOperation LoadSceneAsync(int sceneBuildIndex,LoadSceneMode mode){
			return SceneManager.LoadSceneAsync(sceneBuildIndex,mode);
		}



		
		[Obsolete("Use SceneManager2.UnloadSceneAsync")]
		public static bool UnloadScene(Scene scene){
			return SceneManager.UnloadScene(scene);
		}
		
		[Obsolete("Use SceneManager2.UnloadSceneAsync")]
		public static bool UnloadScene(int sceneBuildIndex){
			return SceneManager.UnloadScene(sceneBuildIndex);
		}
		
		[Obsolete("Use SceneManager2.UnloadSceneAsync")]
		public static bool UnloadScene(string sceneName){
			sceneName=getBuildSettingsScenePath(sceneName);
			return SceneManager.UnloadScene(sceneName);
		}
		


		public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex){
			return SceneManager.UnloadSceneAsync(sceneBuildIndex);
		}
		
		public static AsyncOperation UnloadSceneAsync(string sceneName){
			sceneName=getBuildSettingsScenePath(sceneName);
			return SceneManager.UnloadSceneAsync(sceneName);
		}
		
		public static AsyncOperation UnloadSceneAsync(Scene scene){
			return SceneManager.UnloadSceneAsync(scene);
		}
		
		public static AsyncOperation UnloadSceneAsync(int sceneBuildIndex,UnloadSceneOptions options){
			return SceneManager.UnloadSceneAsync(sceneBuildIndex,options);
		}
		
		public static AsyncOperation UnloadSceneAsync(string sceneName,UnloadSceneOptions options){
			sceneName=getBuildSettingsScenePath(sceneName);
			return SceneManager.UnloadSceneAsync(sceneName,options);
		}
		
		public static AsyncOperation UnloadSceneAsync(Scene scene,UnloadSceneOptions options){
			return SceneManager.UnloadSceneAsync(scene,options);
		}

		/// <summary>
		/// 根据场景路径名称，返回在BuildSettings中的完整的路径名称
		/// </summary>
		/// <param name="sceneName">场景路径名称</param>
		/// <param name="isIncludeExtension">是否包含.unity扩展名</param>
		/// <returns></returns>
		public static string getBuildSettingsScenePath(string sceneName,bool isIncludeExtension=true){
			//是不是路径名
			bool isPathName=sceneName.LastIndexOf('/')>-1;
			var scenes=ProjectImporter.instance.buildSettingsData.scenes;
			int len=scenes.Length;
			for(int i=0;i<len;i++){
				string path=scenes[i].path;
				if(isPathName){
					if(path.LastIndexOf(sceneName,StringComparison.Ordinal)>-1){
						//匹配路径名称
						sceneName=path;
						break;
					}
				}else{
					//非路径名称时，只查找倒数'/'位置到最后
					string tempPath=path.Substring(path.LastIndexOf('/'));
					if(tempPath.LastIndexOf(sceneName,StringComparison.Ordinal)>-1){
						//匹配名称
						sceneName=path;
						break;
					}
				}
			}
			if(!isIncludeExtension){
				int dotIndex=sceneName.LastIndexOf('.');
				sceneName=sceneName.Substring(0,dotIndex);
			}
			return sceneName;
		}

	}
}
