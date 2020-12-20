using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public sealed class EnemySpawn : MonoBehaviour {
		public GameObject EnemyPrefab;
		public Transform  SpawnPoint;
		public Transform  Target;
		public int        MaxEnemyCount = 10;
		public float      SpawnInterval = 5f;

		float _spawnTimer;

		bool CanSpawn => (Enemy.Instances.Count < MaxEnemyCount);

		void Update() {
			if ( CanSpawn ) {
				_spawnTimer += Time.deltaTime;
				if ( _spawnTimer > SpawnInterval ) {
					_spawnTimer -= SpawnInterval;
					Spawn();
				}
			}
		}

		void Spawn() {
			var enemyGo = Instantiate(EnemyPrefab, SpawnPoint.position, Quaternion.identity);
			var enemy   = enemyGo.GetComponent<Enemy>();
			enemy.Init(Target);
		}
	}
}
