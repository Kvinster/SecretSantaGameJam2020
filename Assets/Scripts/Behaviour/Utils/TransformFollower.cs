using UnityEngine;

namespace SmtProject.Behaviour.Utils {
	public sealed class TransformFollower : MonoBehaviour {
		public Vector3   Offset = new Vector3(0, 0, -10);
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

		[ContextMenu("Set offset")]
		void SetOffset() {
			if ( Target ) {
				Offset = transform.position - Target.position;
#if UNITY_EDITOR
				UnityEditor.EditorUtility.SetDirty(this);
#endif
			}
		}
	}
}
