using UnityEngine;

using System;

namespace SmtProject.Behaviour.Utils {
	[RequireComponent(typeof(Collider2D))]
	public sealed class Collider2DNotifier : MonoBehaviour {
		public event Action<Collider2D>  OnTriggerEnter;
		public event Action<Collider2D>  OnTriggerStay;
		public event Action<Collider2D>  OnTriggerExit;
		public event Action<Collision2D> OnCollisionEnter;
		public event Action<Collision2D> OnCollisionStay;
		public event Action<Collision2D> OnCollisionExit;

		void OnTriggerEnter2D(Collider2D other) {
			OnTriggerEnter?.Invoke(other);
		}

		void OnTriggerStay2D(Collider2D other) {
			OnTriggerStay?.Invoke(other);
		}

		void OnTriggerExit2D(Collider2D other) {
			OnTriggerExit?.Invoke(other);
		}

		void OnCollisionEnter2D(Collision2D other) {
			OnCollisionEnter?.Invoke(other);
		}

		void OnCollisionStay2D(Collision2D other) {
			OnCollisionStay?.Invoke(other);
		}

		void OnCollisionExit2D(Collision2D other) {
			OnCollisionExit?.Invoke(other);
		}
	}
}
