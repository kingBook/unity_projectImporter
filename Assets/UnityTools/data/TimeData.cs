namespace UnityTools{
	using UnityEngine;
	using System.Collections;

	public class TimeData:ScriptableObject{
		public float fixedTimestep;
		public float maximumAllowedTimestep;
		public float timeScale;
		public float maximumParticleTimestep;
	}
}
