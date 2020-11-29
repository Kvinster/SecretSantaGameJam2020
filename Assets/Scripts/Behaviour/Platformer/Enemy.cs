using UnityEngine;

using JetBrains.Annotations;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Enemy : MonoBehaviour {
		enum WalkDir {
			Up    = 0,
			Down  = 1,
			Left  = 2,
			Right = 3
		}

		static readonly int IsWalking   = Animator.StringToHash("IsWalking");
		static readonly int IsDying     = Animator.StringToHash("IsDying");
		static readonly int WalkDirHash = Animator.StringToHash("WalkDir");

		public Transform Target;
		public float     WalkSpeed;
		[Space]
		public Animator WalkAnimator;

		bool    _isWalking;
		bool    _isDying;
		WalkDir _curWalkDir;

		void Update() {
			if ( _isDying ) {
				return;
			}
			var dir = Target.position - transform.position;
			transform.Translate(dir.normalized * (Time.deltaTime * WalkSpeed));
			UpdateWalkParams(dir);
			UpdateAnimParams();
		}

		public void StartDying() {
			_isDying = true;
			UpdateAnimParams();
		}

		[UsedImplicitly]
		void Die() {
			Destroy(gameObject);
		}

		void UpdateWalkParams(Vector2 speed) {
			if ( speed == Vector2.zero ) {
				_isWalking = false;
			} else {
				_isWalking = true;
				var angle = Vector2.SignedAngle(Vector2.right, speed);
				if ( (angle <= 45) && (angle >= -45) ) {
					_curWalkDir = WalkDir.Right;
				} else if ( (angle > 45) && (angle < 135) ) {
					_curWalkDir = WalkDir.Up;
				} else if ( (angle < -45) && (angle > -135) ) {
					_curWalkDir = WalkDir.Down;
				} else {
					_curWalkDir = WalkDir.Left;
				}
			}
		}

		void UpdateAnimParams() {
			WalkAnimator.SetBool(IsWalking, _isWalking);
			WalkAnimator.SetBool(IsDying, _isDying);
			WalkAnimator.SetInteger(WalkDirHash, (int) _curWalkDir);
		}
	}
}
