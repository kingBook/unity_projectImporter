using System;
using UnityEngine;

namespace UnityTools{
	[Serializable]
	public struct QualitySettings{
		public string name;
		public int pixelLightCount;
		public int shadows;
		public int shadowResolution;
		public int shadowProjection;
		public int shadowCascades;
		public float shadowDistance;
		public float shadowNearPlaneOffset;
		public float shadowCascade2Split;
		public Vector3 shadowCascade4Split;
		public int shadowmaskMode;
		public int skinWeights;
		public int textureQuality;
		public int anisotropicTextures;
		public int antiAliasing;
		public bool softParticles;
		public bool softVegetation;
		public bool realtimeReflectionProbes;
		public bool billboardsFaceCameraPosition;
		public int vSyncCount;
		public float lodBias;
		public int maximumLODLevel;
		public bool streamingMipmapsActive;
		public bool streamingMipmapsAddAllCameras;
		public float streamingMipmapsMemoryBudget;
		public int streamingMipmapsRenderersPerFrame;
		public int streamingMipmapsMaxLevelReduction;
		public int streamingMipmapsMaxFileIORequests;
		public int particleRaycastBudget;
		public int asyncUploadTimeSlice;
		public int asyncUploadBufferSize;
		public bool asyncUploadPersistentBuffer;
		public float resolutionScalingFixedDPIFactor;
		public string[] excludedTargetPlatforms;
	}

	[Serializable]
	public struct PlatformDefaultQuality{
		public string platform;
		public int qualityLevel;
	}

	public class QualityData:ScriptableObject{
		public int currentQuality;
		public QualitySettings[] qualitySettings;
		public PlatformDefaultQuality[] perPlatformDefaultQuality;
		
	}
}
