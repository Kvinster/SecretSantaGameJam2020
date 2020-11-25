using UnityEngine;

using Shapes;

namespace SmtProject.Behaviour {
	public sealed class ChainJoint : MonoBehaviour {
		public Line            Line;
		public Rigidbody2D     Rigidbody;
		public DistanceJoint2D DistanceJoint;
		public HingeJoint2D    HingeJoint;

		bool _isHead;

		void Update() {
			var cb = HingeJoint.connectedBody;
			if ( cb ) {
				Line.Start =
					transform.InverseTransformPoint(cb.transform.TransformPoint(new Vector3(_isHead ? 0f : 1f, 0)));
			}
		}

		public void Init(Rigidbody2D connectedBody, bool isHead = false) {
			_isHead = isHead;

			transform.localPosition     = connectedBody.transform.localPosition + new Vector3(1, 0);
			DistanceJoint.connectedBody = connectedBody;
			DistanceJoint.distance      = isHead ? 0f : 1f;
			HingeJoint.connectedBody    = connectedBody;
		}
	}
}
