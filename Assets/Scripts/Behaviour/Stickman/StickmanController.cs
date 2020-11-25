using UnityEngine;

namespace SmtProject.Behaviour {
	public sealed class StickmanController : MonoBehaviour {
		public Rigidbody2D Rigidbody;
		public float       Speed;

		Vector2 _input;

		void Update() {
			_input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
		}

		void FixedUpdate() {
			if ( _input == Vector2.zero ) {
				return;
			}
			Rigidbody.AddForce(_input * Speed, ForceMode2D.Impulse);

			_input = Vector2.zero;
		}
	}
}
