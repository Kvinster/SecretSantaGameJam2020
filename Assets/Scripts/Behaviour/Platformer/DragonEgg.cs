using System.Collections.Generic;

using UnityEngine;

using SmtProject.Behaviour.Utils;

using DG.Tweening;

namespace SmtProject.Behaviour.Platformer {
	public sealed class DragonEgg : MonoBehaviour {
		public DragonSpawner      DragonSpawner;
		public Exploder           EggExploder;
		public Collider2DNotifier RangeNotifier;
		public GameObject         HintRoot;
		public float              DisappearDuration;
		public List<GameObject>   Fragments;

		bool _detected;
		bool _used;

		Sequence _disappearAnim;

		void Start() {
			RangeNotifier.OnTriggerEnter += OnRangeEnter;
			RangeNotifier.OnTriggerExit  += OnRangeExit;
			HintRoot.SetActive(false);
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
				return;
			}
			if ( !_detected ) {
				return;
			}
			if ( Input.GetKeyDown(KeyCode.E) ) {
				Spawn();
				_used = true;
				HintRoot.SetActive(false);
			}
		}

		void Spawn() {
			EggExploder.Explode();
			DragonSpawner.Spawn();
		}

		void OnRangeEnter(Collider2D other) {
			if ( _used ) {
				return;
			}
			if ( other.GetComponent<Player>() ) {
				_detected = true;
				if ( !_used ) {
					HintRoot.SetActive(true);
				}
			}
		}

		void OnRangeExit(Collider2D other) {
			if ( _used ) {
				return;
			}
			if ( other.GetComponent<Player>() ) {
				_detected = false;
				HintRoot.SetActive(false);
			}
		}
	}
}
