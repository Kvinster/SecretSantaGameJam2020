using UnityEngine;

using System.Collections;

namespace SmtProject.Behaviour.Platformer {
	public sealed class DemonSceneController : MonoBehaviour {
		public Demon Demon;

		void Start() {
			Demon.OnDie += OnDemonDie;
		}

		void OnDemonDie() {
			Demon.OnDie -= OnDemonDie;
			StartCoroutine(EndGameCoro());
		}

		IEnumerator EndGameCoro() {
			yield return new WaitForSeconds(2f);
			ScreenTransitionController.Instance.Transition("Victory", FindObjectOfType<Player>().transform.position);
		}
	}
}
