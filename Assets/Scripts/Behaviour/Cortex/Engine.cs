using UnityEngine;

using DG.Tweening;

namespace SmtProject.Behaviour.Cortex {
	public sealed class Engine : MonoBehaviour {
		static readonly int FireEnable  = Animator.StringToHash("FireEnable");
		static readonly int FireDisable = Animator.StringToHash("FireDisable");

		public Rigidbody2D Rigidbody;
		public Vector3     ForceOffset;
		public float       EngineForce = 1f;
		public Animator    FireAnimator;
		public Transform   FireAnimTransform;

		bool _isFiring;

		Tween _fireAnim;

		void Update() {
			if ( _isFiring ) {
				_isFiring = false;
				_fireAnim?.Kill();
				_fireAnim = null;
			} else {
				FireAnimator.ResetTrigger(FireEnable);
				FireAnimator.SetTrigger(FireDisable);

				if ( _fireAnim == null ) {
					_fireAnim = FireAnimTransform.DOScale(new Vector3(0f, 0f, 1f), 0.5f);
				}
			}
		}

		public void Fire() {
			var force = transform.up * EngineForce;
			Rigidbody.AddForceAtPosition(force, transform.TransformPoint(ForceOffset));
			FireAnimator.ResetTrigger(FireDisable);
			FireAnimator.SetTrigger(FireEnable);

			if ( !_isFiring ) {
				_fireAnim?.Kill();
				_fireAnim = FireAnimTransform.DOScale(new Vector3(1f, 1f, 1f), 0.5f);
			}

			_isFiring = true;
		}
	}
}
