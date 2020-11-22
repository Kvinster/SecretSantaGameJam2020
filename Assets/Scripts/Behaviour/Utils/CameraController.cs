using UnityEngine;

namespace SmtProject.Behaviour.Utils {
	public sealed class CameraController : MonoBehaviour {
		static readonly Vector3 Offset = new Vector3(0, 0, -10f);

		public Transform Target;

		void Update() {
			if ( !Target ) {
				return;
			}
			transform.position = Target.position + Offset;
		}
	}
}
