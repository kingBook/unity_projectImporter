namespace UnityEngine{
	using System;
	using System.ComponentModel;

	public class QualitySettings2{
		public static int particleRaycastBudget { get; set; }
		
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

		public static void DecreaseLevel(){
			
		}
		
		public static void DecreaseLevel([DefaultValue("false")] bool applyExpensiveChanges){
			
		}
		
		public static int GetQualityLevel(){
			return 0;
		}
		public static void IncreaseLevel(){
			
		}
		
		public static void IncreaseLevel([DefaultValue("false")] bool applyExpensiveChanges){
			
		}
		
		public static void SetQualityLevel(int index,[DefaultValue("true")] bool applyExpensiveChanges){
			
		}
		public static void SetQualityLevel(int index){
			
		}
	}
}