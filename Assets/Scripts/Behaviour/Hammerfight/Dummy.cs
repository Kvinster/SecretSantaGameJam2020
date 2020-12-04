using UnityEngine;

using System;

namespace SmtProject.Behaviour.Hammerfight {
	public sealed class Dummy : MonoBehaviour {
		public int   MaxHp;
		public float DamageMult;
		[Space]
		public float ShakeDuration  = 0.5f;
		public float ShakeMagnitude = 1f;

		int _curHp;

		public int CurHp {
			get => _curHp;
			private set {
				if ( _curHp == value ) {
					return;
				}
				_curHp = value;
				while ( _curHp < 0 ) {
					_curHp += MaxHp;
				}
				OnCurHpChanged?.Invoke(_curHp);
			}
		}

		public event Action<int> OnCurHpChanged;

		void Start() {
			CurHp = MaxHp;
		}

		void OnCollisionEnter2D(Collision2D other) {
			var damage = other.relativeVelocity.magnitude * DamageMult;
			CurHp -= Mathf.FloorToInt(damage);
			ScreenShake.Instance.Shake(ShakeDuration, ShakeMagnitude);
		}
	}
}
