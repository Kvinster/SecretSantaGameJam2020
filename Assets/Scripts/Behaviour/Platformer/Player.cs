using System.Collections.Generic;

using UnityEngine;

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
			Left  = 1,
			Down  = 2,
			Right = 3
		}

		static readonly int IsAliveHash   = Animator.StringToHash("IsAlive");
		static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
		static readonly int IsHittingHash = Animator.StringToHash("IsHitting");
		static readonly int WalkDirHash   = Animator.StringToHash("WalkDir");
		static readonly int BowHash       = Animator.StringToHash("Bow");
		static readonly int SlashHash     = Animator.StringToHash("Slash");
		static readonly int ThrustHash    = Animator.StringToHash("Thrust");
		static readonly int SpellHash     = Animator.StringToHash("Spell");

		public Rigidbody2D Rigidbody;
		public float       KnockbackForce;
		public float       KnockbackDuration;
		[Space]
		public float    WalkSpeed;
		public Animator WalkAnimator;
		[Space]
		public FloatStatBar HealthBar;
		public FloatStatBar XpBar;
		public TMP_Text     CurLevelText;
		[Space]
		public Spear Spear;
		[Space]
		public List<SimplePlayerAnimationController> Equipment = new List<SimplePlayerAnimationController>();

		PlayerController _playerController;

		bool _canAttack = true;

		bool    _isHurt;
		bool    _isWalking;
		bool    _isHitting;
		WalkDir _curWalkDir;

		int _curWeaponHash = ThrustHash;

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

			WalkAnimator.SetTrigger(_curWeaponHash);

			foreach ( var equip in Equipment ) {
				equip.SetTrigger(_curWeaponHash);
			}
		}

		[UsedImplicitly]
		void StopHit() {
			_isHitting = false;
			_canAttack = true;

			WalkAnimator.ResetTrigger(_curWeaponHash);

			UpdateAnimParams();

			foreach ( var equip in Equipment ) {
				equip.ResetTrigger(_curWeaponHash);
			}
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
			WalkAnimator.SetBool(IsAliveHash, true);
			WalkAnimator.SetBool(IsWalkingHash, _isWalking);
			WalkAnimator.SetBool(IsHittingHash, _isHitting);
			WalkAnimator.SetInteger(WalkDirHash, (int) _curWalkDir);

			foreach ( var equip in Equipment ) {
				equip.UpdateAnimParams(true, _isWalking, _isHitting, (int) _curWalkDir);
			}
		}

		void OnCollisionEnter2D(Collision2D other) {
			if ( !other.gameObject.GetComponent<Enemy>() ) {
				return;
			}
			if ( _isHurt ) {
				return;
			}
			_isHurt = true;

			var curPos = transform.position;
			Rigidbody.AddForce((curPos - other.transform.position).normalized * KnockbackForce, ForceMode2D.Impulse);
			_knockbackAnim = DOTween.Sequence().AppendInterval(KnockbackDuration);
			_knockbackAnim.onComplete += () => { _isHurt = false; };

			_playerController.TakeDamage(10);
		}
	}
}
