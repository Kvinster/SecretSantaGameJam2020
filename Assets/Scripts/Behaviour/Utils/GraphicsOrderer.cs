using UnityEngine;

using System.Collections.Generic;

namespace SmtProject.Behaviour.Utils {
	public sealed class GraphicsOrderer : MonoBehaviour {
		public float Offset;

		readonly List<SpriteRenderer> _spriteRenderers = new List<SpriteRenderer>();

		void Start() {
			CollectSpriteRenderers();
		}

		void CollectSpriteRenderers() {
			_spriteRenderers.Clear();
			CollectSpriteRenderers(transform);
		}

		void CollectSpriteRenderers(Transform tr) {
			var sr = tr.GetComponent<SpriteRenderer>();
			if ( sr ) {
				_spriteRenderers.Add(sr);
			}
			for ( var i = 0; i < tr.childCount; ++i ) {
				CollectSpriteRenderers(tr.GetChild(i));
			}
		}

		void Update() {
			var startIndex = Mathf.CeilToInt(-(transform.position.y + Offset) * 1000f);
			foreach ( var sr in _spriteRenderers ) {
				sr.sortingOrder = startIndex++;
			}
		}

		[ContextMenu("Update")]
		void EditorUpdate() {
			var startIndex = Mathf.CeilToInt(-(transform.position.y + Offset) * 1000f);
			foreach ( var sr in GetComponentsInChildren<SpriteRenderer>() ) {
				sr.sortingOrder = startIndex++;
			}
		}
	}
}
