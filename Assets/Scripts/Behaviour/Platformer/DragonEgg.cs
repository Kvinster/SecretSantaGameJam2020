using UnityEngine;

using System.Collections.Generic;

using SmtProject.Behaviour.Platformer.StatBar;
using SmtProject.Behaviour.Utils;
using SmtProject.Utils.ValueAnim;

using DG.Tweening;

namespace SmtProject.Behaviour.Platformer {
	public sealed class DragonEgg : MonoBehaviour {
		public DragonSpawner    DragonSpawner;
		public Exploder         EggExploder;
		public float            DisappearDuration;
		public List<GameObject> Fragments;
		[Space]
		public int                StartHp;
		public FloatStatBar       HealthBar;
		public Collider2DNotifier CollisionNotifier;

		bool _used;

		Sequence _disappearAnim;

		int            _curHp;
		FloatValueAnim _hpAnim;

		void OnDestroy() {
			if ( CollisionNotifier ) {
				CollisionNotifier.OnCollisionEnter -= OnDetectCollisionStart;
			}
		}

		void Start() {
			_curHp                    =  StartHp;
			_hpAnim                   =  new FloatValueAnim(_curHp);
			_hpAnim.OnCurValueChanged += OnCurHpAnimValueChanged;
			HealthBar.Init(_curHp, 0, _curHp);
			CollisionNotifier.OnCollisionEnter += OnDetectCollisionStart;
		}

		void Update() {
			if ( _used ) {
				if ( _disappearAnim == null ) {
					var time = DisappearDuration * 0.9f;
					_disappearAnim = DOTween.Sequence()
						.AppendInterval(time);
					foreach ( var fragment in Fragments ) {
						_disappearAnim.InsertCallback(time, () => { Destroy(fragment); });
					}
					_disappearAnim.onComplete += () => Destroy(gameObject);
				}
			}
		}

		public void Hatch() {
			Spawn();
			_used                              =  true;
			CollisionNotifier.OnCollisionEnter -= OnDetectCollisionStart;
		}

		void OnCurHpAnimValueChanged(float curValue) {
			HealthBar.UpdateView(curValue);
		}

		void OnCurHpChanged() {
			if ( _curHp == 0 ) {
				ScreenTransitionController.Instance.Transition("Platformer", transform.position, () => {
					var player = FindObjectOfType<Player>();
					return player ? player.transform.position : Vector3.zero;
				});
			} else {
				_hpAnim.SetNextValue(_curHp);
			}
		}

		void Spawn() {
			EggExploder.Explode();
			DragonSpawner.Spawn();
		}

		void OnDetectCollisionStart(Collision2D other) {
			var enemy = other.gameObject.GetComponent<Enemy>();
			if ( enemy ) {
				if ( !enemy.TakeDamage(10) ) {
					enemy.Knockback((enemy.transform.position - transform.position).normalized, 10f);
				}
				_curHp = Mathf.Max(0, _curHp - 10);
				OnCurHpChanged();
			}
		}
	}
}
