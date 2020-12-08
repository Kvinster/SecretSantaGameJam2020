using UnityEngine;

using JetBrains.Annotations;

namespace SmtProject.Behaviour.Platformer {
	public sealed class DragonFire : MonoBehaviour {
		public int            Damage = 5;
		public SpriteRenderer SpriteRenderer;

		Dragon _owner;

		public void Init(bool isFlipped, Dragon owner) {
			_owner = owner;

			SpriteRenderer.flipX = isFlipped;
		}

		[UsedImplicitly]
		void EndPlay() {
			_owner.EndAttack();
			Destroy(gameObject);
		}

		void OnTriggerEnter2D(Collider2D other) {
			var enemy = other.gameObject.GetComponent<Enemy>();
			if ( enemy ) {
				enemy.TakeDamage(Damage);
			}
		}
	}
}
