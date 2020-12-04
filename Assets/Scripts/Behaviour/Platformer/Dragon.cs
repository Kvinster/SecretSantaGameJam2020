using UnityEngine;

using System.Collections.Generic;

using SmtProject.Behaviour.Utils;

using JetBrains.Annotations;

using Shapes;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Dragon : MonoBehaviour {
		const string FirePrefabPathPrefix = "Prefabs/Platformer/DragonFire_";

		static readonly int IsWalking    = Animator.StringToHash("IsWalking");
		static readonly int WalkInverted = Animator.StringToHash("WalkInverted");
		static readonly int Idle         = Animator.StringToHash("Idle");
		static readonly int Attack       = Animator.StringToHash("Attack");
		static readonly int Hurt         = Animator.StringToHash("Hurt");
		static readonly int Death        = Animator.StringToHash("Death");

		public DragonType Type;
		public float      MinDistance;
		public float      WalkSpeed;
		[Space]
		public SpriteRenderer SpriteRenderer;
		[Space]
		public Animator Animator;
		[Space]
		public Transform FirePos;
		[Space]
		public Collider2DNotifier DetectNotifier;
		public Collider2DNotifier AttackRangeNotifier;

		bool _canAct = true;

		bool _isFlipped;

		Player    _owner;
		Transform _target;

		GameObject _firePrefab;

		readonly HashSet<Enemy>                _detectedEnemies = new HashSet<Enemy>();
		readonly Dictionary<Collider2D, Enemy> _colliderToEnemy = new Dictionary<Collider2D, Enemy>();

		GameObject FirePrefab {
			get {
				if ( !_firePrefab ) {
					_firePrefab =
						Resources.Load<GameObject>(FirePrefabPathPrefix + Type);
				}
				return _firePrefab;
			}
		}

		Transform CurTarget {
			get {
				Transform target = null;
				if ( _detectedEnemies.Count > 0 ) {
					var minDistance = float.MaxValue;
					foreach ( var enemy in _detectedEnemies ) {
						var dist = Vector2.Distance(enemy.transform.position, transform.position);
						if ( dist < minDistance ) {
							minDistance = dist;
							target      = enemy.transform;
						}
					}
				} else {
					target = _target;
				}
				return target;
			}
		}

		void Start() {
			DetectNotifier.OnTriggerEnter      += OnDetectObjectEnter;
			DetectNotifier.OnTriggerExit       += OnDetectObjectExit;
			AttackRangeNotifier.OnTriggerEnter += OnAttackObjectEnter;

			if ( !_owner ) {
				var player = FindObjectOfType<Player>();
				if ( player ) {
					Init(player);
				}
			}
		}

		void Update() {
			if ( _canAct ) {
				if ( Input.GetKeyDown(KeyCode.Q) ) {
					StartAttack();
				} else {
					Transform target;
					Vector3   targetPos;
					float     minDistance;
					if ( _detectedEnemies.Count > 0 ) {
						target      = CurTarget;
						targetPos   = Vector2.Lerp(_target.position, target.position, 0.2f);
						minDistance = 0.4f;
					} else {
						target      = _target;
						targetPos   = _target.position;
						minDistance = MinDistance;
					}
					var curPos   = transform.position;
					var distance = Vector2.Distance(targetPos, curPos);
					if ( distance > minDistance ) {
						var dir   = targetPos - curPos;
						var shift = dir.normalized * (Time.deltaTime * WalkSpeed);

						if ( shift.magnitude > (distance - minDistance) ) {
							shift = shift.normalized * Mathf.Max(0f, distance - MinDistance);
						}
						if ( shift != Vector3.zero ) {
							transform.Translate(shift);
							Animator.SetBool(IsWalking, true);

							var closestTarget = target;
							if ( target != _target ) {
								var targetDistance = Vector2.Distance(targetPos, transform.position);
								closestTarget =
									(targetDistance < Vector2.Distance(_target.transform.position, curPos))
										? target
										: _target;
							}
							var closestTargetPos = closestTarget.position;
							var flip             = (target.position.x < curPos.x) ^ (closestTargetPos.x < curPos.x);
							Animator.SetBool(WalkInverted, flip);
						} else {
							Animator.SetBool(IsWalking, false);
						}
					} else {
						Animator.SetBool(IsWalking, false);
					}
					if ( _target == target ) {
						UpdateOrientation(_target);
					} else {
						var targetDistance = Vector2.Distance(target.position, transform.position);
						UpdateOrientation((targetDistance < Vector2.Distance(_target.transform.position, curPos))
							? target
							: _target);
					}
				}
			}
		}

		public void Init(Player player) {
			_owner  = player;
			_target = player.transform;
		}

		public void EndAttack() {
			Animator.ResetTrigger(Attack);
			_canAct = true;

			EndBlockingAction();
		}

		void UpdateOrientation(Transform target) {
			var targetPos = target.position;
			_isFlipped           = (targetPos.x < transform.position.x);
			SpriteRenderer.flipX = _isFlipped;
		}

		[UsedImplicitly]
		void EndHurt() {
			Animator.ResetTrigger(Hurt);
			_canAct = true;

			EndBlockingAction();
		}

		[UsedImplicitly]
		void EndDie() {
			Animator.ResetTrigger(Death);
			Destroy(gameObject);

			EndBlockingAction();
		}

		void StartAttack() {
			UpdateOrientation(CurTarget);
			Animator.SetTrigger(Attack);
			_canAct = false;

			CreateFire();

			StartBlockingAction();
		}

		void CreateFire() {
			var go          = Instantiate(FirePrefab, null);
			var firePos     = FirePos.localPosition;
			var instancePos = new Vector3(firePos.x * (_isFlipped ? -1 : 1), firePos.y, firePos.z);
			go.transform.position = transform.TransformPoint(instancePos);
			go.transform.SetParent(null);
			var fire = go.GetComponent<DragonFire>();
			fire.Init(_isFlipped, this);
		}

		void StartBlockingAction() {
			Animator.ResetTrigger(Idle);
		}

		void EndBlockingAction() {
			Animator.SetTrigger(Idle);
		}

		void OnDetectObjectEnter(Collider2D objectCollider) {
			var enemy = objectCollider.GetComponent<Enemy>();
			if ( enemy ) {
				_detectedEnemies.Add(enemy);
				_colliderToEnemy.Add(objectCollider, enemy);
			}
		}

		void OnDetectObjectExit(Collider2D objectCollider) {
			if ( _colliderToEnemy.TryGetValue(objectCollider, out var enemy) ) {
				_detectedEnemies.Remove(enemy);
				_colliderToEnemy.Remove(objectCollider);
			}
		}

		void OnAttackObjectEnter(Collider2D objectCollider) {
			if ( !_canAct ) {
				return;
			}
			var enemy = objectCollider.GetComponent<Enemy>();
			if ( enemy ) {
				StartAttack();
			}
		}
	}
}
