using UnityEngine;

using System.Collections.Generic;

using Shapes;

namespace SmtProject.Behaviour.Utils {
	public sealed class GraphicsOrderer : MonoBehaviour {
		interface IOrderable {
			bool IsValid { get; }
			void Reorder(int index);
		}

		abstract class BaseOrderable<T> : IOrderable where T : Object {
			protected readonly T Orderable;

			public bool IsValid => Orderable;

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

		sealed class ShapeRendererOrderable : BaseOrderable<ShapeRenderer> {
			public ShapeRendererOrderable(ShapeRenderer orderable) : base(orderable) { }

			public override void Reorder(int index) {
				Orderable.SortingOrder = index;
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

			var shapeRenderer = tr.GetComponent<ShapeRenderer>();
			if ( shapeRenderer ) {
				_orderables.Add(new ShapeRendererOrderable(shapeRenderer));
			}

			for ( var i = 0; i < tr.childCount; ++i ) {
				CollectOrderables(tr.GetChild(i));
			}
		}

		void Update() {
			var startIndex = Mathf.CeilToInt(-(transform.position.y + Offset) * 1000f);
			var needUpdate = false;
			foreach ( var orderable in _orderables ) {
				if ( !orderable.IsValid ) {
					needUpdate = true;
					continue;
				}
				orderable.Reorder(startIndex++);
			}
			if ( needUpdate ) {
				_orderables.RemoveAll(x => !x.IsValid);
			}
		}
	}
}
