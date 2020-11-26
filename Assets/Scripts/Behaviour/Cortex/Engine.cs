using UnityEngine;
using UnityEngine.VFX;

namespace SmtProject.Behaviour.Cortex {
	public sealed class Engine : MonoBehaviour {
		public Rigidbody2D  Rigidbody;
		public Vector3      ForceOffset;
		public float        EngineForce = 1f;
		public VisualEffect FireEffect;

		bool _ifFiring;

		void Update() {
			if ( _ifFiring ) {
				_ifFiring = false;
			} else {
				FireEffect.Stop();
			}
		}

		public void Fire() {
			var force = transform.up * EngineForce;
			Rigidbody.AddForceAtPosition(force, transform.TransformPoint(ForceOffset));
			FireEffect.Play();
		}
	}
}
