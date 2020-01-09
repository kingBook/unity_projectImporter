using UnityEngine;

namespace UnityTools{
	public class TimeData:ScriptableObject{
		public float fixedTimestep;
		public float maximumAllowedTimestep;
		public float timeScale;
		public float maximumParticleTimestep;
	}
}
