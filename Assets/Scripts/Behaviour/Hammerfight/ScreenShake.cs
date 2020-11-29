using System.Collections;

using UnityEngine;

namespace SmtProject.Behaviour {
	public sealed class ScreenShake : MonoBehaviour {
		static ScreenShake _instance;

		public static ScreenShake Instance {
			get {
				if ( !_instance ) {
					var go = new GameObject("[ScreenShake]");
					_instance = go.AddComponent<ScreenShake>();
					_instance.Init();
				}
				return _instance;
			}
		}

		Transform _cameraTransform;
		bool      _isShaking;

		public void Shake(float duration, float magnitude) {
			if ( _isShaking ) {
				return;
			}
			StartCoroutine(ShakeCoro(duration, magnitude));
		}

		void Init() {
			_cameraTransform = Camera.main.transform;
		}

		IEnumerator ShakeCoro(float duration, float magnitude) {
			_isShaking = true;

			var oldPos = _cameraTransform.localPosition;

			var timer = 0f;
			while ( timer < duration ) {
				var pos    = _cameraTransform.localPosition;
				var offset = Random.insideUnitCircle * magnitude;
				_cameraTransform.localPosition =  pos + (Vector3) offset;
				timer                          += Time.deltaTime;
				yield return null;
			}

			_cameraTransform.localPosition = oldPos;

			_isShaking = false;
		}
	}
}
