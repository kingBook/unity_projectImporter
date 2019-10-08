namespace UnityProjectImporter{
	using UnityEngine;
	using System.Collections;

	public class PhysicsData:ScriptableObject{
		public Vector3 gravity;
		public PhysicMaterial defaultMaterial;
		public float bounceThreshold;
		public float sleepThreshold;
		public float defaultContactOffset;
		public int defaultSolverIterations;
		public int defaultSolverVelocityIterations;
		public bool queriesHitBackfaces;
		public bool queriesHitTriggers;
		public bool enableAdaptiveForce;

		public float clothInterCollisionDistance;
		public float clothInterCollisionStiffness;

		public int contactsGeneration;

		public int[] layerCollisionMatrix;
		public bool autoSimulation;
		public bool autoSyncTransforms;
		public bool reuseCollisionCallbacks;

		public bool clothInterCollisionSettingsToggle;

		public Vector3 clothGravity;
		public int contactPairsMode;
		public int broadphaseType;
		public Bounds worldBounds;
		public int worldSubdivisions;
		public int frictionType;
		public bool enableEnhancedDeterminism;
		public bool enableUnifiedHeightmaps;
		public float defaultMaxAngularSpeed;
		
	}
}
