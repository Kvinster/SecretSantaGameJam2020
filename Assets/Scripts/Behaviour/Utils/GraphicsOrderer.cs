using UnityEngine;

using System.Collections.Generic;

namespace SmtProject.Behaviour.Utils {
	public sealed class GraphicsOrderer : MonoBehaviour {
		interface IOrderable {
			void Reorder(int index);
		}

		abstract class BaseOrderable<T> : IOrderable {
			protected readonly T Orderable;

			protected BaseOrderable(T orderable) {
				Orderable = orderable;
			}

			public abstract void Reorder(int index);
		}

		sealed class SpriteRendererOrderable : BaseOrderable<SpriteRenderer> {
			public SpriteRendererOrderable(SpriteRenderer orderable) : base(orderable) { }

			public override void Reorder(int index) {
				Orderable.sortingOrder = index;
			}
		}

		sealed class MeshRendererOrderable : BaseOrderable<MeshRenderer> {
			public MeshRendererOrderable(MeshRenderer orderable) : base(orderable) { }

			public override void Reorder(int index) {
				Orderable.sortingOrder = index;
			}
		}

		public float Offset;

		readonly List<IOrderable> _orderables = new List<IOrderable>();

		void Start() {
			CollectOrderables();
		}

		void CollectOrderables() {
			_orderables.Clear();
			CollectOrderables(transform);
		}

		void CollectOrderables(Transform tr) {
			var sr = tr.GetComponent<SpriteRenderer>();
			if ( sr ) {
				_orderables.Add(new SpriteRendererOrderable(sr));
			}

			var mr = tr.GetComponent<MeshRenderer>();
			if ( mr ) {
				_orderables.Add(new MeshRendererOrderable(mr));
			}

			for ( var i = 0; i < tr.childCount; ++i ) {
				CollectOrderables(tr.GetChild(i));
			}
		}

		void Update() {
			var startIndex = Mathf.CeilToInt(-(transform.position.y + Offset) * 1000f);
			foreach ( var orderable in _orderables ) {
				orderable.Reorder(startIndex++);
			}
		}
	}
}
