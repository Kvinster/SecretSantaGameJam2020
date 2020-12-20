using UnityEngine;

using System.Collections.Generic;

namespace SmtProject.Behaviour.Platformer {
	public sealed class EnemySpawn : MonoBehaviour {
		public bool            StartLocked;
		public GameObject      EnemyPrefab;
		public Transform       Target;
		public int             MaxEnemyCount = 10;
		public float           SpawnInterval = 5f;
		public List<Transform> SpawnPoints;

		public bool IsLocked { get; set; }

		float _spawnTimer;

		bool CanSpawn => (Enemy.Instances.Count < MaxEnemyCount);

		void Start() {
			IsLocked = StartLocked;
		}

		void Update() {
			if ( IsLocked ) {
				return;
			}
			if ( CanSpawn ) {
				_spawnTimer += Time.deltaTime;
				if ( _spawnTimer > SpawnInterval ) {
					_spawnTimer -= SpawnInterval;
					Spawn();
				}
			}
		}

		public void BurstSpawn(int amount) {
			for ( var i = 0; i < amount; ++i ) {
				Spawn();
			}
		}

		void Spawn() {
			var spawnPoint = GetRandomSpawnPoint();
			if ( !spawnPoint ) {
				return;
			}
			var enemyGo = Instantiate(EnemyPrefab, spawnPoint.position, Quaternion.identity);
			var enemy   = enemyGo.GetComponent<Enemy>();
			enemy.Init(Target);
			_spawnTimer = 0f;
		}

		Transform GetRandomSpawnPoint() {
			if ( SpawnPoints.Count == 0 ) {
				Debug.LogError("SpawnPoints is null");
				return null;
			}
			return SpawnPoints[Random.Range(0, SpawnPoints.Count)];
		}
	}
}
