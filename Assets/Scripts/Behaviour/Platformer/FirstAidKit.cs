using SmtProject.Behaviour.Utils;
using SmtProject.Core.Platformer;

using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public sealed class FirstAidKit : MonoBehaviour {
		public Collider2DNotifier DetectRange;
		public GameObject         ToolTipRoot;

		bool _isActive;

		void Start() {
			DetectRange.OnTriggerEnter += OnDetectRangeEnter;
			DetectRange.OnTriggerExit  += OnDetectRangeExit;
			ToolTipRoot.SetActive(false);
		}

		void Update() {
			if ( _isActive && Input.GetKeyDown(KeyCode.E) ) {
				PlayerController.Instance.RestoreHp();
				DetectRange.OnTriggerEnter -= OnDetectRangeEnter;
				DetectRange.OnTriggerExit  -= OnDetectRangeExit;
				Destroy(gameObject);
			}
		}

		void OnDetectRangeEnter(Collider2D other) {
			if ( other.gameObject.GetComponent<Player>() ) {
				_isActive = true;
				ToolTipRoot.SetActive(true);
			}
		}

		void OnDetectRangeExit(Collider2D other) {
			if ( other.gameObject.GetComponent<Player>() ) {
				_isActive = false;
				ToolTipRoot.SetActive(false);
			}
		}
	}
}
