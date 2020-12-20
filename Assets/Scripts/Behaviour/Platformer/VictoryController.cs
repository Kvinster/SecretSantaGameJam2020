using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public sealed class VictoryController : MonoBehaviour {
		public float LockDuration = 2f;

		float _timer;

		void Update() {
			_timer += Time.deltaTime;

			if ( Input.anyKey && (_timer > LockDuration) ) {
#if UNITY_WEBGL
				ScreenTransitionController.Instance.Transition("Platformer", Vector3.zero);
#else
				Application.Quit();
#endif
			}
		}
	}
}
