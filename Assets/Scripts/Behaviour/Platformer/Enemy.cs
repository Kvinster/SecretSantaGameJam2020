using UnityEngine;

using SmtProject.Behaviour.Utils;

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

		public float WalkSpeed;
		[Space]
		public Animator WalkAnimator;
		[Space]
		public Collider2DNotifier InnerDetectNotifier;
		public Collider2DNotifier OuterDetectNotifier;

		bool    _isWalking;
		bool    _isDying;
		WalkDir _curWalkDir;

		Transform _target;

		void Start() {
			InnerDetectNotifier.OnTriggerEnter += OnInnerDetectObjectEnter;
			OuterDetectNotifier.OnTriggerExit  += OnOuterDetectObjectExit;
		}

		void Update() {
			if ( _isDying ) {
				return;
			}
			if ( _target ) {
				var dir = _target.position - transform.position;
				transform.Translate(dir.normalized * (Time.deltaTime * WalkSpeed));
				UpdateWalkParams(dir);
			} else {
				_isWalking = false;
			}
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

		void OnInnerDetectObjectEnter(Collider2D other) {
			if ( other.GetComponent<Player>() ) {
				_target = other.transform;
			}
		}

		void OnOuterDetectObjectExit(Collider2D other) {
			if ( other.transform == _target ) {
				_target = null;
			}
		}
	}
}
