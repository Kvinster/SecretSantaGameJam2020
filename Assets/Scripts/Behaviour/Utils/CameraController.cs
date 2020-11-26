using UnityEngine;

namespace SmtProject.Behaviour.Utils {
	public sealed class CameraController : MonoBehaviour {
		static readonly Vector3 Offset = new Vector3(0, 0, -10f);

		public Transform Target;
		public bool      Vertical   = true;
		public bool      Horizontal = true;

		void Update() {
			if ( !Target ) {
				return;
			}
			var oldPos    = transform.position;
			var targetPos = Target.position;
			transform.position = new Vector3(Horizontal ? targetPos.x : oldPos.x, Vertical ? targetPos.y : oldPos.y) +
			                     Offset;
		}
	}
}
