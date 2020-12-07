﻿using UnityEngine;

using SmtProject.Behaviour.Platformer.StatBar;
using SmtProject.Core.Platformer;
using SmtProject.Utils.ValueAnim;

using DG.Tweening;
using JetBrains.Annotations;
using TMPro;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Player : MonoBehaviour {
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
		public float    WalkSpeed;
		public Animator WalkAnimator;
		[Space]
		public FloatStatBar HealthBar;
		public FloatStatBar XpBar;
		public TMP_Text     CurLevelText;
		[Space]
		public Spear Spear;

		PlayerController _playerController;

		bool _canAttack = true;

		bool    _isHurt;
		bool    _isWalking;
		bool    _isHitting;
		WalkDir _curWalkDir;

		Tween _knockbackAnim;

		FloatValueAnim _hpAnim;
		XpValueAnim    _xpAnim;

		int CurHp    => _playerController.CurHp;
		int CurXp    => _playerController.CurXp;
		int CurLevel => _playerController.CurLevel;

		void Start() {
			_playerController = PlayerController.Instance;

			HealthBar.Init(CurHp, 0, _playerController.MaxHp);
			XpBar.Init(CurXp, 0, _playerController.NextLevelXp);

			_playerController.CurHp.OnCurValueChanged += OnCurHpChanged;
			_playerController.MaxHp.OnCurValueChanged += OnMaxHpChanged;
			_hpAnim                                   =  new FloatValueAnim(CurHp);
			_hpAnim.OnCurValueChanged                 += OnCurHpAnimValueChanged;
			OnMaxHpChanged(_playerController.MaxHp);

			_playerController.CurXp.OnCurValueChanged += OnCurXpChanged;
			_xpAnim                   =  new XpValueAnim(CurXp, CurLevel, 0.2f);
			_xpAnim.OnCurValueChanged += OnCurXpAnimValueChanged;
			_xpAnim.OnCurLevelChanged += OnCurXpAnimLevelChanged;
			_xpAnim.SetNextValue(CurXp);
			OnCurXpAnimLevelChanged(0);

			Spear.OnEnemyKilled += OnEnemyKilled;
		}

		void OnCurHpChanged(int curHp) {
			_hpAnim.SetNextValue(curHp);
		}

		void OnCurHpAnimValueChanged(float curValue) {
			HealthBar.UpdateView(curValue);
		}

		void OnMaxHpChanged(int maxHp) {
			HealthBar.UpdateView(_hpAnim.CurValue, 0, maxHp);
		}

		void OnCurXpChanged(int curXp) {
			if ( _xpAnim.CurLevel < CurLevel ) {
				_xpAnim.SetNextValue(_playerController.GetNextLevelXp(CurLevel - 1));
				_xpAnim.SetNextLevel(curXp, CurLevel);
			} else {
				_xpAnim.SetNextValue(curXp);
			}
		}

		void OnCurXpAnimValueChanged(float curValue) {
			XpBar.UpdateView(curValue);
		}

		void OnCurXpAnimLevelChanged(int level) {
			XpBar.UpdateView(_xpAnim.CurXp, _playerController.GetNextLevelXp(level - 1),  _playerController.GetNextLevelXp(level));
			CurLevelText.text = level.ToString();
		}

		void OnEnemyKilled() {
			_playerController.AddXp(1);
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

					input *= (Time.deltaTime * WalkSpeed);
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
			_isHurt = true;

			_knockbackAnim = transform
				.DOJump(
					transform.position + (transform.position - other.transform.position).normalized * KnockbackForce,
					KnockbackHeight, 1, KnockbackDuration)
				.SetEase(Ease.OutSine);
			_knockbackAnim.onComplete += () => { _isHurt = false; };

			_playerController.TakeDamage(10);
		}
	}
}
