using UnityEngine;
using UnityEngine.VFX;

namespace SmtProject.Behaviour.Cortex {
	public sealed class Bomb : MonoBehaviour {
		const float ExplosionDuration = 0.2f;

		public GameObject   BombRoot;
		public VisualEffect Explosion;

		bool  _exploded;
		float _timer;

		void Update() {
			if ( _exploded ) {
				if ( _timer > ExplosionDuration ) {
					Destroy(gameObject);
				} else {
					_timer += Time.deltaTime;
				}
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			if ( _exploded ) {
				return;
			}
			BombRoot.SetActive(false);
			Explosion.SendEvent("Explode");
			_exploded = true;
		}
	}
}
