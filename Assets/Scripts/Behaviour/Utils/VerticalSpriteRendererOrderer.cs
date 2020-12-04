using UnityEngine;

namespace SmtProject.Behaviour.Utils {
	[RequireComponent(typeof(SpriteRenderer))]
	public sealed class VerticalSpriteRendererOrderer : MonoBehaviour {
		public float Offset;

		SpriteRenderer _spriteRenderer;

		void Start() {
			_spriteRenderer = GetComponent<SpriteRenderer>();
		}

		[ContextMenu("Update")]
		void Update() {
			_spriteRenderer.sortingOrder = Mathf.CeilToInt(-(transform.position.y + Offset) * 1000f);
		}

		void OnDrawGizmosSelected() {
			if ( Application.isPlaying ) {
				return;
			}
			var oldColor = Gizmos.color;
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position + Vector3.up * Offset, 0.05f);
			Gizmos.color = oldColor;
		}
	}
}
