namespace unity_framework{
	using UnityEngine;
	using System.Collections;
	
	public interface IUpdate{
		void FixedUpdate();
		void Update();
		void LateUpdate();
		void OnGUI();
		void OnRenderObject();
		
	}

}