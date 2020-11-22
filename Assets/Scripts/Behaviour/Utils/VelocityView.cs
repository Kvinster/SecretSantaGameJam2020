using UnityEngine;

namespace SmtProject.Behaviour.Utils {
	public sealed class VelocityView : MonoBehaviour {
		public Rigidbody2D Rigidbody;
		public Shapes.Line Line;

		void Start() {
			Line.Start = Vector3.zero;
			Line.End   = Vector3.zero;
		}

		void Update() {
			var velocity = Rigidbody.velocity;
			Line.End = velocity;
		}
	}
}
