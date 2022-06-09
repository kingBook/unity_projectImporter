using System;
using System.Collections.Generic;
using UnityEditor;

namespace UnityTools {

    public class ProjectImporterEditor : Editor {

        public static readonly string CurrentProjectPath = Environment.CurrentDirectory.Replace('\\', '/');
        public static readonly string CurrentProjectTempPath = CurrentProjectPath + "/Temp";
        public static readonly string ProjectImporterTempPath = "Assets/UnityTools/temp";
        public static readonly string ResourcePath = "Assets/UnityTools/Resources";

        [MenuItem("Tools/TestImport")]
        public static void TestImport() {
            ImportCurrentProjectSettings();
            //importProject("D:/kingBook/projects/unity_parkinggame");
            //deleteProject("unity_parkinggame");


        }

        /// <summary>
        /// 将当前的项目设置导入到"ProjectImporter/Resources"保存
        /// </summary>
        public static void ImportCurrentProjectSettings() {
            ImportProject(CurrentProjectPath, isImportAssets: false, isImportBuildSettings: false, projectName: "default", isDeleteBuildSettingsScenes: false, isDeleteAssets: false);
        }

        /// <summary>
        /// 导入一个项目的Assets文件夹和ProjectSettings
        /// </summary>
        /// <param name="path">项目的路径位置</param>
        /// <param name="isImportAssets">是否导入Assets文件夹</param>
        /// <param name="isImportBuildSettings">是否导入BuildSettings</param>
        /// <param name="projectName">导入进来的文件夹名；项目中的所有设置文件的名称前缀,null时将从path的最后截取</param>
        /// <param name="isDeleteBuildSettingsScenes">导入前是否清除由projectName指定的项目在上一次导入时在BuildSettings窗口中的场景</param>
        /// <param name="isDeleteAssets">导入前是否清除由projectName指定的项目在上一次导入时的资源文件夹</param>
        public static void ImportProject(string path, bool isImportAssets = true, bool isImportBuildSettings = true,
        string projectName = null, bool isDeleteBuildSettingsScenes = true, bool isDeleteAssets = true) {
            if (projectName == null) {
                projectName = path.Substring(path.LastIndexOf('/') + 1);
            }

            //删除指定项目的所有资源和设置,用于重复导入时清空上一次导入的资源和设置
            DeleteProject(projectName, isDeleteBuildSettingsScenes, isDeleteAssets);

            //创建临时文件夹,如果文件夹存在则先删除
            FileUtil2.CreateDirectory(ProjectImporterTempPath, true);

            //导入tags和Layers
            var tagsAndLayersImporter = new TagsAndLayersImporter();
            tagsAndLayersImporter.Import(path, CurrentProjectTempPath, projectName);

            if (isImportAssets) {
                //导入Assets文件夹,并修改.cs文件解决冲突,必须在导入tags和Layers之后执行
                var assetsImporter = new AssetsImporter();
                assetsImporter.Import(path, CurrentProjectTempPath, projectName);
            }

            //导入Time
            var timeImporter = new TimeImporter();
            timeImporter.Import(path, CurrentProjectTempPath, projectName);

            //导入Physics
            var physicsImporter = new PhysicsImporter();
            physicsImporter.Import(path, CurrentProjectTempPath, projectName);

            //导入Physics2D
            var physics2DImporter = new Physics2DImporter();
            physics2DImporter.Import(path, CurrentProjectTempPath, projectName);

            //导入Quality
            var qualityImporter = new QualityImporter();
            qualityImporter.Import(path, CurrentProjectTempPath, projectName);

            if (isImportBuildSettings) {
                //导入BuildSettings
                var buildSettingsImporter = new BuildSettingsImporter();
                buildSettingsImporter.Import(path, CurrentProjectTempPath, projectName);
            }

            //所有事情完成，删除"ProjectImporter/temp"临时文件夹
            AssetDatabase.DeleteAsset(ProjectImporterTempPath);
        }

        /// <summary>
        /// 用于重复导入指定的项目时，删除指定项目的所有资源和设置。
        /// <br>项目的设置文件一定会被清除，BuildSettings窗口中的场景和导入进来的项目资源文件夹，</br>
        /// <br>则由参数决定是否删除</br>
        /// </summary>
        /// <param name="projectName">项目名称</param>
        /// <param name="isDeleteBuildSettingsScenes">是否删除项目在BuildSettings窗口中的场景列表</param>
        /// <param name="isDeleteAssets">删除导入进来的项目资源文件夹</param>
        public static void DeleteProject(string projectName, bool isDeleteBuildSettingsScenes = true, bool isDeleteAssets = true) {
            if (isDeleteBuildSettingsScenes) {
                //删除项目在BuildSettings窗口中的场景
                DeleteBuildSettingsScenes(projectName);
            }
            //删除项目设置
            DeleteProjectSettings(projectName);
            if (isDeleteAssets) {
                //删除项目文件夹
                AssetDatabase.DeleteAsset("Assets/" + projectName);
            }
            //刷新AssetDataBase
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 删除指定项目在BuildSettings窗口中的的场景
        /// </summary>
        /// <param name="projectName">项目名称</param>
        private static void DeleteBuildSettingsScenes(string projectName) {
            var scenes = new List<EditorBuildSettingsScene>();
            scenes.AddRange(EditorBuildSettings.scenes);
            int i = scenes.Count;
            while (--i >= 0) {
                bool isProjectScene = scenes[i].path.IndexOf(projectName + '/', StringComparison.Ordinal) > -1;
                if (isProjectScene) {
                    scenes.RemoveAt(i);
                }
            }
            EditorBuildSettings.scenes = scenes.ToArray();
        }

        /// <summary>
        /// 删除指定项目在"Assets/ProjectImporter/Resources"中的设置
        /// </summary>
        /// <param name="projectName"></param>
        private static void DeleteProjectSettings(string projectName) {
            string[] settingsAssetNames = new string[]{
                "_buildSettingsData.asset",
                "_layersData.asset",
                "_physics2dData.asset",
                "_physicsData.asset",
                "_qualityData.asset",
                "_sortingLayersData.asset",
                "_timeData.asset"
            };
            EditorUtility.DisplayProgressBar("Hold on...", "deleting settings for " + projectName, 0f);
            int len = settingsAssetNames.Length;
            for (int i = 0; i < len; i++) {
                string namePath = projectName + settingsAssetNames[i];
                EditorUtility.DisplayProgressBar("Hold on...", "Delete " + namePath, (i + 1f) / (float)len);
                AssetDatabase.DeleteAsset(ResourcePath + "/" + namePath);
            }
            EditorUtility.ClearProgressBar();
        }


        /// <summary>
        /// 重命名项目
        /// </summary>
        /// <param name="oldName">旧的名称</param>
        /// <param name="newName">新的名称</param>
        public void RenameProject(string oldName, string newName) {

        }

    }
}
