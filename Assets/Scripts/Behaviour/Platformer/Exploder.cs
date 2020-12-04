using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Exploder : MonoBehaviour {
		public Explodable Explodable;
		public float      ExplosionForce;

		public void Explode() {
			var centerPos = Explodable.transform.position;
			Explodable.explode();
			foreach ( var frag in Explodable.fragments ) {
				frag.GetComponent<Rigidbody2D>()
					.AddForce((frag.transform.position - centerPos).normalized * ExplosionForce, ForceMode2D.Impulse);
			}
		}
	}
}
