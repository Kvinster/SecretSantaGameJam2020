using UnityEngine;

using System.Collections.Generic;

using SmtProject.Behaviour.Platformer.StatBar;
using SmtProject.Behaviour.Utils;

using DG.Tweening;
using JetBrains.Annotations;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Enemy : MonoBehaviour {
		public static readonly HashSet<Enemy> Instances = new HashSet<Enemy>();

		enum WalkDir {
			Up    = 0,
			Down  = 1,
			Left  = 2,
			Right = 3
		}

		static readonly int IsWalking   = Animator.StringToHash("IsWalking");
		static readonly int IsDying     = Animator.StringToHash("IsDying");
		static readonly int WalkDirHash = Animator.StringToHash("WalkDir");

		public int         StartHp = 10;
		public float       WalkSpeed;
		public Collider2D  Collider;
		public Rigidbody2D Rigidbody;
		[Space]
		public GameObject   HealthBarRoot;
		public FloatStatBar HealthBar;
		[Space]
		public Animator WalkAnimator;
		[Space]
		public Collider2DNotifier InnerDetectNotifier;
		public Collider2DNotifier OuterDetectNotifier;

		bool    _isWalking;
		bool    _isDying;
		WalkDir _curWalkDir;

		Transform _target;

		Tween _knockbackAnim;

		int _maxHp;
		int _curHp;
		int CurHp {
			get => _curHp;
			set {
				_curHp = value;
				HealthBar.UpdateView(_curHp);
			}
		}

		void Start() {
			InnerDetectNotifier.OnTriggerEnter += OnInnerDetectObjectEnter;
			OuterDetectNotifier.OnTriggerExit  += OnOuterDetectObjectExit;

			_maxHp = StartHp;
			CurHp  = StartHp;

			HealthBar.Init(CurHp, 0, _maxHp);

			Instances.Add(this);
		}

		void OnDestroy() {
			Instances.Remove(this);
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

		public void Init(Transform target) {
			_target = target;
		}

		public bool TakeDamage(int damage) {
			if ( _isDying ) {
				return false;
			}
			CurHp = Mathf.Max(0, CurHp - damage);
			if ( CurHp == 0 ) {
				StartDying();
				return true;
			}
			return false;
		}

		public void Knockback(Vector2 direction, float knockbackForce) {
			if ( _isDying ) {
				return;
			}
			_knockbackAnim?.Kill(true);
			_knockbackAnim = DOTween.Sequence()
				.AppendInterval(0.3f)
				.AppendCallback(() => { Collider.enabled = true; });

			Collider.enabled = false;
			Rigidbody.AddForce(direction * knockbackForce, ForceMode2D.Impulse);
		}

		void StartDying() {
			_isDying = true;
			_knockbackAnim?.Kill(true);
			Collider.enabled = false;
			HealthBarRoot.SetActive(false);
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
