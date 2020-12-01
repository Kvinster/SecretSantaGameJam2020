using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	[RequireComponent(typeof(Camera))]
	public sealed class CameraController : MonoBehaviour {
		public Transform Target;
		public float     LeftBorder  = 0.2f;
		public float     RightBorder = 0.2f;

		Camera _camera;

		float _size;

		void Start() {
			_camera = GetComponent<Camera>();

			_size = _camera.orthographicSize * 2f * _camera.aspect;
		}

		void Update() {
			var diff        = 0f;
			var curX        = transform.position.x;
			var targetX     = Target.position.x;
			var leftBorder  = (curX - (_size * (0.5f - LeftBorder)));
			var rightBorder = (curX + (_size * (0.5f - RightBorder)));
			if ( targetX < leftBorder ) {
				diff = targetX - leftBorder;
			} else if ( targetX > rightBorder) {
				diff = targetX - rightBorder;
			}
			if ( !Mathf.Approximately(diff, 0f) ) {
				transform.Translate(new Vector3(diff, 0f));
			}
		}
	}
}
