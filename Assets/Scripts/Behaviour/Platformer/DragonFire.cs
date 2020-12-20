using UnityEngine;

using JetBrains.Annotations;

namespace SmtProject.Behaviour.Platformer {
	public sealed class DragonFire : MonoBehaviour {
		public int            Damage = 5;
		public SpriteRenderer SpriteRenderer;

		Dragon _owner;

		bool _isBig;

		public void Init(bool isFlipped, Dragon owner) {
			_owner = owner;

			_isBig = (owner.Type == DragonType.Adult);

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

			var demon = other.gameObject.GetComponent<Demon>();
			if ( demon ) {
				demon.TakeDamage(Damage);
			}

			if ( _isBig ) {
				var rocks = other.gameObject.GetComponent<Rocks>();
				if ( rocks ) {
					rocks.Destroy();
				}
			}
		}
	}
}
