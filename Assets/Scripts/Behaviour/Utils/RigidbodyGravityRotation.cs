using UnityEngine;

namespace SmtProject.Behaviour.Utils {
	[RequireComponent(typeof(Rigidbody2D))]
	public sealed class RigidbodyGravityRotation : MonoBehaviour {
		Rigidbody2D _rigidbody;

		void Start() {
			_rigidbody = GetComponent<Rigidbody2D>();
		}

		void Update() {
			var velocity = _rigidbody.velocity;
			var angle    = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg + 90;
			_rigidbody.rotation = Mathf.Lerp(_rigidbody.rotation, angle, Time.deltaTime);
		}
	}
}
