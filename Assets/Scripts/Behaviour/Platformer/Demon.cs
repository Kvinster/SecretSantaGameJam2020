using System;

using UnityEngine;

using System.Collections.Generic;

using SmtProject.Behaviour.Utils;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Demon : MonoBehaviour {
		static readonly int AttackHash  = Animator.StringToHash("Attack");
		static readonly int HurtHash    = Animator.StringToHash("Hurt");
		static readonly int DeathHash   = Animator.StringToHash("Death");
		static readonly int DeadHash    = Animator.StringToHash("Dead");
		static readonly int IsInverted  = Animator.StringToHash("IsInverted");
		static readonly int IsWalking   = Animator.StringToHash("IsWalking");
		static readonly int IsTriggered = Animator.StringToHash("IsTriggered");

		public Transform          StartTarget;
		public Animator           Animator;
		public float              WalkSpeed;
		public float              AttackDistance;
		public int                StartHp;
		public Collider2DNotifier AttackRangeNotifier;
		public Collider2D         Collider;
		public Collider2DNotifier DetectRangeNotifier;
		public RandomSoundPlayer  DetectSoundPlayer;
		public RandomSoundPlayer  DeathSoundPlayer;

		bool _isAlive;
		bool _isLeft;
		bool _isWalking;
		bool _isTriggered;

		Transform _target;

		readonly HashSet<GameObject> _possibleTargets = new HashSet<GameObject>();

		int _curHp;

		public event Action OnDie;

		void Start() {
			_isLeft  = true;
			_target  = StartTarget;
			_curHp   = StartHp;
			_isAlive = true;

			UpdateParams();

			AttackRangeNotifier.OnTriggerEnter += OnAttackRangeEnter;
			AttackRangeNotifier.OnTriggerStay  += OnAttackRangeStay;
			AttackRangeNotifier.OnTriggerExit  += OnAttackRangeExit;
			if ( !_target ) {
				DetectRangeNotifier.OnTriggerEnter += OnDetectRangeEnter;
			}
		}

		void Update() {
			if ( _isTriggered || !_isAlive ) {
				return;
			}
			if ( _target ) {
				var yOffset = 0.3f;
				var dir = _target.position - transform.position - new Vector3(0, yOffset);
				if ( _target.position.x > transform.position.x ) {
					_isLeft = false;
				} else if ( _target.position.x < transform.position.x ) {
					_isLeft = true;
				}
				if ( dir.magnitude > AttackDistance ) {
					transform.Translate(dir.normalized * (Time.deltaTime * WalkSpeed));
					_isWalking = true;
				} else if ( Mathf.Abs(_target.position.y - yOffset - transform.position.y) > float.Epsilon ) {
					transform.Translate((_target.position.y - yOffset > transform.position.y ? Vector2.up : Vector2.down) *
					                    (Time.deltaTime * WalkSpeed));
				} else {
					_isWalking = false;
				}
			} else {
				_isWalking = false;
			}
			UpdateParams();
		}

		public void TakeDamage(int damage) {
			if ( _isTriggered || !_isAlive ) {
				return;
			}
			_curHp = Mathf.Max(0, _curHp - damage);
			if ( _curHp == 0 ) {
				Die();
			} else {
				Hurt();
			}
		}

		void TryAttack(Collider2D other) {
			if ( !_isAlive || _isTriggered ) {
				return;
			}
			var player = other.GetComponent<Player>();
			if ( player ) {
				Attack();
			}
			UpdateParams();
		}

		void Attack() {
			_isTriggered = true;
			Animator.SetTrigger(AttackHash);
			UpdateParams();
		}

		void TryDamage() {
			foreach ( var target in _possibleTargets ) {
				var player = target.GetComponent<Player>();
				if ( player ) {
					player.TakeDamage(10, gameObject, 2);
				}

				var dragon = target.GetComponent<Dragon>();
				if ( dragon ) {
					// TODO: damage dragon
				}
			}
		}

		void EndAttack() {
			_isTriggered = false;
			Animator.ResetTrigger(AttackHash);
			UpdateParams();
		}

		void Hurt() {
			_isTriggered = true;
			Animator.SetTrigger(HurtHash);
			UpdateParams();
		}

		void EndHurt() {
			_isTriggered = false;
			Animator.ResetTrigger(HurtHash);
			UpdateParams();
		}

		void Die() {
			_isAlive     = false;
			_isTriggered = true;
			Animator.SetTrigger(DeathHash);
			UpdateParams();
			DeathSoundPlayer.Play();
		}

		void EndDie() {
			Animator.ResetTrigger(DeathHash);
			Animator.SetTrigger(DeadHash);
			Collider.enabled = false;
			UpdateParams();
			OnDie?.Invoke();
		}

		void UpdateParams() {
			Collider.offset = new Vector2(_isLeft ? 0.3f : -0.3f, Collider.offset.y);
			Animator.SetBool(IsInverted, _isLeft);
			Animator.SetBool(IsWalking, _isWalking);
			Animator.SetBool(IsTriggered, _isTriggered);
		}

		void OnAttackRangeEnter(Collider2D other) {
			TryAttack(other);
			if ( other.GetComponent<Player>() || other.GetComponent<Dragon>() ) {
				_possibleTargets.Add(other.gameObject);
			}
		}

		void OnAttackRangeStay(Collider2D other) {
			TryAttack(other);
		}

		void OnAttackRangeExit(Collider2D other) {
			_possibleTargets.Remove(other.gameObject);
		}

		void OnDetectRangeEnter(Collider2D other) {
			if ( other.GetComponent<Player>() ) {
				_target = other.transform;
				DetectSoundPlayer.Play();
			}
		}
	}
}
