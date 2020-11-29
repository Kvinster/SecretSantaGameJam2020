using UnityEngine;

using JetBrains.Annotations;

namespace SmtProject.Behaviour.Platformer {
	public sealed class DragonFire : MonoBehaviour {
		public SpriteRenderer SpriteRenderer;

		public void Init(bool isFlipped) {
			SpriteRenderer.flipX = isFlipped;
		}

		[UsedImplicitly]
		void EndPlay() {
			Destroy(gameObject);
		}

		void OnTriggerEnter2D(Collider2D other) {
			var enemy = other.gameObject.GetComponent<Enemy>();
			if ( enemy ) {
				enemy.StartDying();
			}
		}
	}
}
