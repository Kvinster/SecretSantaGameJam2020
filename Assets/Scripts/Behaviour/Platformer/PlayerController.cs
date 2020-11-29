using System;

using DG.Tweening;

using UnityEngine;

using JetBrains.Annotations;

namespace SmtProject.Behaviour.Platformer {
	public sealed class PlayerController : MonoBehaviour {
		enum WalkDir {
			Up    = 0,
			Down  = 1,
			Left  = 2,
			Right = 3
		}

		static readonly int IsWalking   = Animator.StringToHash("IsWalking");
		static readonly int IsHitting   = Animator.StringToHash("IsHitting");
		static readonly int WalkDirHash = Animator.StringToHash("WalkDir");

		public float KnockbackForce;
		public float KnockbackHeight;
		public float KnockbackDuration;
		[Space]
		public int      MaxHp;
		[Space]
		public float    WalkSpeed;
		public Animator WalkAnimator;

		bool _canAttack = true;

		bool    _isHurt;
		bool    _isWalking;
		bool    _isHitting;
		WalkDir _curWalkDir;

		Tween _knockbackAnim;

		int _curHp;
		public int CurHp {
			get => _curHp;
			private set {
				if ( _curHp == value ) {
					return;
				}
				_curHp = Mathf.Clamp(value, 0, MaxHp);
				OnCurHpChanged?.Invoke(_curHp);
			}
		}

		public event Action<int> OnCurHpChanged;

		void Start() {
			CurHp = MaxHp;
		}

		void Update() {
			if ( _isHurt ) {
				return;
			}
			if ( _canAttack && Input.GetKeyDown(KeyCode.Space) ) {
				Hit();
			} else if ( !_isHitting ) {
				var input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
				if ( input != Vector2.zero ) {
					input = input.normalized * (Time.deltaTime * WalkSpeed);
					transform.Translate(input);

					UpdateWalkParams(input);
				} else {
					UpdateWalkParams(Vector2.zero);
				}
			}
			UpdateAnimParams();
		}

		void Hit() {
			_isHitting = true;
			_canAttack = false;
		}

		[UsedImplicitly]
		void StopHit() {
			_isHitting = false;
			_canAttack = true;
			UpdateAnimParams();
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
			WalkAnimator.SetBool(IsHitting, _isHitting);
			WalkAnimator.SetInteger(WalkDirHash, (int) _curWalkDir);
		}

		void OnCollisionEnter2D(Collision2D other) {
			if ( !other.gameObject.GetComponent<Enemy>() ) {
				return;
			}
			if ( _isHurt ) {
				return;
			}
			Destroy(other.gameObject);
			_isHurt = true;

			_knockbackAnim = transform
				.DOJump(
					transform.position + (transform.position - other.transform.position).normalized * KnockbackForce,
					KnockbackHeight, 1, KnockbackDuration)
				.SetEase(Ease.OutSine);
			_knockbackAnim.onComplete += () => { _isHurt = false; };

			CurHp -= 10;
		}
	}
}
