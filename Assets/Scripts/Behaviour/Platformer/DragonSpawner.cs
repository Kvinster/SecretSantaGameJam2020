using UnityEngine;

using DG.Tweening;

namespace SmtProject.Behaviour.Platformer {
	public sealed class DragonSpawner : BasePrefabSpawner {
		public Player    Player;
		public Transform SpawnPos;
		public float     ScaleAnimDuration = 0.5f;

		protected override void InitInstance(GameObject instanceGameObject) {
			var dragon = instanceGameObject.GetComponent<Dragon>();
			if ( !dragon ) {
				Debug.LogError("No Dragon on instance game object");
				return;
			}
			instanceGameObject.transform.position = SpawnPos.position;
			dragon.Init(Player);
			dragon.transform.localScale = Vector3.zero;
			dragon.transform.DOScale(Vector3.one, ScaleAnimDuration);
		}
	}
}
