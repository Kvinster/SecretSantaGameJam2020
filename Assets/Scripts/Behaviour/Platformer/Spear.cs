using UnityEngine;

using System;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Spear : MonoBehaviour {
		public int       Damage         = 1;
		public float     KnockbackForce = 5f;
		public Transform KnockbackOrigin;
		public float     RechargeTime;

		bool  _isRecharging;
		float _rechargeTimer;

		public event Action OnEnemyKilled;

		void Update() {
			if ( _isRecharging ) {
				_rechargeTimer -= Time.deltaTime;
				if ( _rechargeTimer <= 0f ) {
					_isRecharging  = false;
					_rechargeTimer = RechargeTime;
				}
			}
		}

		void OnTriggerEnter2D(Collider2D other) {
			if ( _isRecharging ) {
				return;
			}
			var enemy = other.gameObject.GetComponent<Enemy>();
			if ( enemy ) {
				if ( enemy.TakeDamage(Damage) ) {
					OnEnemyKilled?.Invoke();
				} else {
					Vector2 curPos = KnockbackOrigin.position;
					enemy.Knockback((other.ClosestPoint(curPos) - curPos).normalized, KnockbackForce);
				}
				_isRecharging  = true;
				_rechargeTimer = RechargeTime;
			}
		}
	}
}
