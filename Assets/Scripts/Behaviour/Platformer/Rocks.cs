using UnityEngine;

using JetBrains.Annotations;

namespace SmtProject.Behaviour.Platformer {
	[RequireComponent(typeof(Collider2D))]
	public sealed class Rocks : MonoBehaviour {
		static readonly int DestroyHash   = Animator.StringToHash("Destroy");
		static readonly int DestroyedHash = Animator.StringToHash("Destroyed");

		public Animator   Animator;
		public Collider2D Collider;
		public bool       Indestructible;

		public void Destroy() {
			if ( Indestructible ) {
				Debug.Log("Psych!");
				return;
			}
			Animator.SetTrigger(DestroyHash);
		}

		[UsedImplicitly]
		public void EndDestroy() {
			Animator.SetTrigger(DestroyedHash);
			Collider.enabled = false;
		}
	}
}
