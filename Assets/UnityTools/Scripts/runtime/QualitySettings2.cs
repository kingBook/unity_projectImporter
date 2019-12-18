namespace UnityEngine{
	using System;
	using System.ComponentModel;
    using UnityTools;

    public class QualitySettings2{
		/*public static int particleRaycastBudget { get; set; }
		
		public static bool softVegetation { get; set; }
		
		public static int vSyncCount { get; set; }
		
		public static int antiAliasing { get; set; }
		
		public static int asyncUploadTimeSlice { get; set; }
		
		public static int asyncUploadBufferSize { get; set; }
		
		public static bool asyncUploadPersistentBuffer { get; set; }
		
		public static bool realtimeReflectionProbes { get; set; }
		
		public static bool billboardsFaceCameraPosition { get; set; }
		
		public static float resolutionScalingFixedDPIFactor { get; set; }
		
		[Obsolete("blendWeights is obsolete. Use skinWeights instead (UnityUpgradable) -> skinWeights",true)]
		public static BlendWeights blendWeights { get; set; }
		
		public static SkinWeights skinWeights { get; set; }
		
		public static bool streamingMipmapsActive { get; set; }
		
		public static float streamingMipmapsMemoryBudget { get; set; }
		
		public static int streamingMipmapsRenderersPerFrame { get; set; }
		
		public static int streamingMipmapsMaxLevelReduction { get; set; }
		
		public static bool streamingMipmapsAddAllCameras { get; set; }
		
		public static int streamingMipmapsMaxFileIORequests { get; set; }
		
		public static int maxQueuedFrames { get; set; }
		
		public static string[] names { get; }
		
		public static bool softParticles { get; set; }
		
		public static ColorSpace desiredColorSpace { get; }
		
		public static ColorSpace activeColorSpace { get; }
		
		public static int masterTextureLimit { get; set; }

		[Obsolete("Use GetQualityLevel and SetQualityLevel",false)]
		public static QualityLevel currentLevel { get; set; }
		
		public static int pixelLightCount { get; set; }
		
		public static ShadowQuality shadows { get; set; }
		
		public static int maximumLODLevel { get; set; }
		
		public static int shadowCascades { get; set; }
		
		public static float shadowDistance { get; set; }
		
		public static ShadowProjection shadowProjection { get; set; }
		
		public static ShadowmaskMode shadowmaskMode { get; set; }
		
		public static float shadowNearPlaneOffset { get; set; }
		
		public static float shadowCascade2Split { get; set; }
		
		public static Vector3 shadowCascade4Split { get; set; }
		
		public static float lodBias { get; set; }
		
		public static AnisotropicFiltering anisotropicFiltering { get; set; }
		
		public static ShadowResolution shadowResolution { get; set; }
		*/
		
		#region custom
		private static int m_qualityLevel=-1;
		
		/// <summary>设置_qualityLevel的值。</summary>
		public static void setQualityLevelValue(int value){
			m_qualityLevel=value;
		}
		#endregion
		
		/// <summary>降低当前的品质级别。</summary>
		public static void DecreaseLevel(){
			SetQualityLevel(--m_qualityLevel);
		}
		
		/// <summary>降低当前的品质级别。</summary>
		public static void DecreaseLevel(bool applyExpensiveChanges=false){
			//这里无法实现applyExpensiveChanges参数
			DecreaseLevel();
		}
		
		public static int GetQualityLevel(){
			return m_qualityLevel;
		}
		
		/// <summary>提高当前的品质级别。</summary>
		public static void IncreaseLevel(){
			SetQualityLevel(++m_qualityLevel);
		}
		
		/// <summary>提高当前的品质级别。</summary>
		public static void IncreaseLevel(bool applyExpensiveChanges=false){
			//这里无法实现applyExpensiveChanges参数
			IncreaseLevel();
		}
		
		public static void SetQualityLevel(int index,bool applyExpensiveChanges=true){
			//这里无法实现applyExpensiveChanges参数
			SetQualityLevel(index);
		}
		
		public static void SetQualityLevel(int index){
			var settingsList=ProjectImporter.instance.qualityData.qualitySettings;
			index=Mathf.Clamp(index,0,settingsList.Length-1);
			ProjectImporter.instance.SetQualityWithSettings(settingsList[index]);
			m_qualityLevel=index;
		}
	}
}