using UnityEngine;

using SmtProject.Behaviour.Platformer.StatBar;

namespace SmtProject.Behaviour.Platformer {
	[RequireComponent(typeof(Collider2D))]
	public sealed class FireBadge : MonoBehaviour {
		public GameObject   ToolTipRoot;
		public GameObject   ProgressBarRoot;
		public FloatStatBar ProgressBar;
		public float        ActivationDuration;
		public GameObject   DragonPrefab;

		Player _player;
		bool   _isActive;

		float _progress;

		void Start() {
			ProgressBar.Init(0, 0, 1);
			ToolTipRoot.SetActive(false);
			ProgressBarRoot.SetActive(false);
		}

		void Update() {
			if ( _isActive ) {
				_progress += Time.deltaTime;
				if ( _progress > ActivationDuration ) {
					var dragon = FindObjectOfType<Dragon>();
					if ( dragon ) {
						var pos = dragon.transform.position;
						Destroy(dragon.gameObject);
						var newDragonGo = Instantiate(DragonPrefab, pos, Quaternion.identity);
						var newDragon   = newDragonGo.GetComponent<Dragon>();
						newDragon.Init(_player);
					} else {
						Debug.LogError("Can't find the dragon");
					}
					Destroy(gameObject);
				} else {
					ProgressBar.UpdateView(_progress / ActivationDuration);
				}
			} else if ( _player && Input.GetKeyDown(KeyCode.E) ) {
				_isActive = true;
				_progress = 0f;
				ProgressBarRoot.SetActive(true);
				ToolTipRoot.SetActive(false);
			}
		}

		void OnTriggerEnter2D(Collider2D other) {
			if ( _isActive ) {
				return;
			}
			var player = other.gameObject.GetComponent<Player>();
			if ( player ) {
				_player = player;
				ToolTipRoot.SetActive(true);
			}
		}

		void OnTriggerExit2D(Collider2D other) {
			var player = other.gameObject.GetComponent<Player>();
			if ( player ) {
				if ( _isActive ) {
					_isActive = false;
					_progress = 0f;
				}
				_player = null;
				ProgressBarRoot.SetActive(false);
				ToolTipRoot.SetActive(false);
			}
		}
	}
}
