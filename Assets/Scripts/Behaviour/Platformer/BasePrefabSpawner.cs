using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public abstract class BasePrefabSpawner : MonoBehaviour {
		public GameObject Prefab;

		float _timer;

		public void Spawn() {
			var go = Instantiate(Prefab, null);
			InitInstance(go);
		}

		protected abstract void InitInstance(GameObject instanceGameObject);
	}
}
