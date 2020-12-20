using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public sealed class ExitController : MonoBehaviour {
		void Update() {
#if !UNITY_WEBGL
			if ( Input.GetKeyDown(KeyCode.Escape) ) {
				Application.Quit();
			}
#endif
		}
	}
}
