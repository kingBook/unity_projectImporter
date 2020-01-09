using System;
using UnityEngine;

namespace UnityTools{
	[Serializable]
	public struct JobOptions{
		public bool useMultithreading;
		public bool useConsistencySorting;
		public int interpolationPosesPerJob;
		public int newContactsPerJob;
		public int collideContactsPerJob;
		public int clearFlagsPerJob;
		public int clearBodyForcesPerJob;
		public int syncDiscreteFixturesPerJob;
		public int syncContinuousFixturesPerJob;
		public int findNearestContactsPerJob;
		public int updateTriggerContactsPerJob;
		public int islandSolverCostThreshold;
		public int islandSolverBodyCostScale;
		public int islandSolverContactCostScale;
		public int islandSolverJointCostScale;
		public int islandSolverBodiesPerJob;
		public int islandSolverContactsPerJob;
	}
	
	public class Physics2dData:ScriptableObject{
		public Vector2 gravity;
		public PhysicsMaterial2D defaultMaterial;
		public int velocityIterations;
		public int positionIterations;
		public float velocityThreshold;
		public float maxLinearCorrection;
		public float maxAngularCorrection;
		public float maxTranslationSpeed;
		public float maxRotationSpeed;
		public float baumgarteScale;
		public float baumgarteTimeOfImpactScale;
		public float timeToSleep;
		public float linearSleepTolerance;
		public float angularSleepTolerance;
		public float defaultContactOffset;
		public JobOptions jobOptions;
		public bool autoSimulation;
		public bool queriesHitTriggers;
		public bool queriesStartInColliders;
		public bool callbacksOnDisable;
		public bool reuseCollisionCallbacks;
		public bool autoSyncTransforms;
		public bool alwaysShowColliders;
		public bool showColliderSleep;
		public bool showColliderContacts;
		public bool showColliderAABB;
		public float contactArrowScale;
		public Color colliderAwakeColor;
		public Color colliderAsleepColor;
		public Color colliderContactColor;
		public Color colliderAABBColor;
		public int[] layerCollisionMatrix;
		
		
	}
}
