using System;

using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Spear : MonoBehaviour {
		public float RechargeTime;

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
				enemy.StartDying();
				_isRecharging  = true;
				_rechargeTimer = RechargeTime;

				OnEnemyKilled?.Invoke();
			}
		}
	}
}
