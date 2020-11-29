using UnityEngine;

using System.Collections.Generic;

using SmtProject.Behaviour.Utils;

using JetBrains.Annotations;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Dragon : MonoBehaviour {
		const string FirePrefabPath = "Prefabs/Platformer/DragonFire";

		static readonly int IsWalking = Animator.StringToHash("IsWalking");
		static readonly int Idle      = Animator.StringToHash("Idle");
		static readonly int Attack    = Animator.StringToHash("Attack");
		static readonly int Hurt      = Animator.StringToHash("Hurt");
		static readonly int Death     = Animator.StringToHash("Death");

		public Transform Target;
		public float     MinDistance;
		public float     WalkSpeed;
		[Space]
		public SpriteRenderer SpriteRenderer;
		[Space]
		public Animator Animator;
		[Space]
		public Transform FirePos;
		[Space]
		public Collider2DNotifier DetectNotifier;

		bool _canMove = true;

		bool _isFlipped;

		GameObject _firePrefab;

		readonly HashSet<Enemy>                _detectedEnemies = new HashSet<Enemy>();
		readonly Dictionary<Collider2D, Enemy> _colliderToEnemy = new Dictionary<Collider2D, Enemy>();

		GameObject FirePrefab {
			get {
				if ( !_firePrefab ) {
					_firePrefab = Resources.Load<GameObject>(FirePrefabPath);
				}
				return _firePrefab;
			}
		}

		void Start() {
			DetectNotifier.OnTriggerEnter += OnObjectEnter;
			DetectNotifier.OnTriggerExit  += OnObjectExit;
		}

		void Update() {
			if ( _canMove ) {
				if ( Input.GetKeyDown(KeyCode.Q) ) {
					StartAttack();
				} else {
					var targetPos = Target.position;
					var curPos    = transform.position;
					var distance  = Vector2.Distance(targetPos, curPos);
					if ( distance > MinDistance ) {
						var dir   = targetPos - curPos;
						var shift = dir.normalized * (Time.deltaTime * WalkSpeed);
						if ( shift.magnitude > (distance - MinDistance) ) {
							shift = shift.normalized * (distance - MinDistance);
						}
						transform.Translate(shift);
						Animator.SetBool(IsWalking, true);
					} else {
						Animator.SetBool(IsWalking, false);
					}
					UpdateOrientation();
				}
			}
		}

		void UpdateOrientation() {
			var targetPos = Vector3.zero;
			if ( _detectedEnemies.Count > 0 ) {
				var minDistance = float.MaxValue;
				foreach ( var enemy in _detectedEnemies ) {
					var dist = Vector2.Distance(enemy.transform.position, transform.position);
					if ( dist < minDistance ) {
						minDistance = dist;
						targetPos   = enemy.transform.position;
					}
				}
			} else {
				targetPos = Target.position;
			}
			_isFlipped           = (targetPos.x < transform.position.x);
			SpriteRenderer.flipX = _isFlipped;
		}

		[UsedImplicitly]
		void EndHurt() {
			Animator.ResetTrigger(Hurt);
			_canMove = true;

			EndBlockingAction();
		}

		[UsedImplicitly]
		void EndDie() {
			Animator.ResetTrigger(Death);
			Destroy(gameObject);

			EndBlockingAction();
		}

		void StartAttack() {
			Animator.SetTrigger(Attack);
			_canMove = false;

			CreateFire();

			StartBlockingAction();
		}

		[UsedImplicitly]
		void EndAttack() {
			Animator.ResetTrigger(Attack);
			_canMove = true;

			EndBlockingAction();
		}

		void CreateFire() {
			var go          = Instantiate(FirePrefab, null);
			var firePos     = FirePos.localPosition;
			var instancePos = new Vector3(firePos.x * (_isFlipped ? -1 : 1), firePos.y, firePos.z);
			go.transform.position = transform.TransformPoint(instancePos);
			go.transform.SetParent(null);
			var fire = go.GetComponent<DragonFire>();
			fire.Init(_isFlipped);
		}

		void StartBlockingAction() {
			Animator.ResetTrigger(Idle);
		}

		void EndBlockingAction() {
			Animator.SetTrigger(Idle);
		}

		void OnObjectEnter(Collider2D objectCollider) {
			var enemy = objectCollider.GetComponent<Enemy>();
			if ( enemy ) {
				_detectedEnemies.Add(enemy);
				_colliderToEnemy.Add(objectCollider, enemy);
			}
		}

		void OnObjectExit(Collider2D objectCollider) {
			if ( _colliderToEnemy.TryGetValue(objectCollider, out var enemy) ) {
				_detectedEnemies.Remove(enemy);
				_colliderToEnemy.Remove(objectCollider);
			}
		}
	}
}
