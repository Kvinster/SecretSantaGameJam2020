using UnityEngine;

namespace SmtProject.Behaviour.Hammerfight {
	public sealed class Weight : MonoBehaviour {
		public DistanceJoint2D DistanceJoint;
		public HingeJoint2D    HingeJoint;
		public SpringJoint2D   SpringJoint;
		[Space]
		public float MaxFrequency = 2f;
		public float MinFrequency = 0.5f;

		float _sjDist;

		void Reset() {
			DistanceJoint = GetComponent<DistanceJoint2D>();
			HingeJoint    = GetComponent<HingeJoint2D>();
			SpringJoint   = GetComponent<SpringJoint2D>();
		}

		void Start() {
			_sjDist = SpringJoint.distance;
		}

		void Update() {
			var distance = Vector2.Distance(SpringJoint.connectedBody.position, transform.position);
			if ( distance <= SpringJoint.distance ) {
				SpringJoint.frequency = MinFrequency;
			} else {
				var diff = DistanceJoint.distance - _sjDist;
				SpringJoint.frequency =
					Mathf.Clamp( (MaxFrequency - MinFrequency) * (distance - _sjDist) / diff, MinFrequency,
						MaxFrequency);
			}
			if ( Mathf.Approximately(SpringJoint.frequency, 0f) ) {
				SpringJoint.frequency = 0.005f;
			}
		}
	}
}
