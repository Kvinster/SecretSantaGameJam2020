using UnityEngine;

using SmtProject.Behaviour.Platformer.StatBar;
using SmtProject.Behaviour.Utils;

namespace SmtProject.Behaviour.Platformer {
	public sealed class CampfireSceneController : MonoBehaviour {
		public Transform          Player;
		public GameObject         EggInactiveRoot;
		public GameObject         EggActiveRoot;
		public DragonEgg          EggActive;
		public float              HatchDuration;
		public Campfire           Campfire;
		public GameObject         ToolTipRoot;
		public Collider2DNotifier AreaNotifier;
		public GameObject         HatchTimerProgressRoot;
		public FloatStatBar       HatchTimerProgress;
		public Collider2D         Collider;
		public GameObject         TutorialRoot;
		[Space]
		public GameObject EnemySpawnRoot;
		public EnemySpawn EnemySpawn;
		public int        StartSpawnedEnemies;
		public int        EndSpawnedEnemies;

		bool _playerPresent;
		bool _isActive;
		bool _hatched;

		float _hatchTimer;

		void OnDisable() {
			AreaNotifier.OnTriggerEnter -= OnAreaEnter;
			AreaNotifier.OnTriggerExit  -= OnAreaExit;
		}

		void Start() {
			EggInactiveRoot.SetActive(true);
			EggActiveRoot.SetActive(false);
			ToolTipRoot.SetActive(false);

			HatchTimerProgressRoot.SetActive(false);
			HatchTimerProgress.Init(0f, 0f, 1f);

			TutorialRoot.SetActive(false);

			AreaNotifier.OnTriggerEnter += OnAreaEnter;
			AreaNotifier.OnTriggerExit  += OnAreaExit;
		}

		void Update() {
			if ( _isActive ) {
				if ( !_hatched ) {
					_hatchTimer += Time.deltaTime;
					if ( _hatchTimer > HatchDuration ) {
						EggActive.Hatch();
						Campfire.Extinguish();
						HatchTimerProgressRoot.SetActive(false);
						EnemySpawn.Target = Player;
						EnemySpawn.BurstSpawn(EndSpawnedEnemies);
						EnemySpawn.IsLocked = true;
						EnemySpawnRoot.SetActive(false);
						Collider.enabled = false;
						_hatched         = true;
						TutorialRoot.SetActive(true);
					} else {
						HatchTimerProgress.UpdateView(_hatchTimer / HatchDuration);
					}
				}
			} else if ( _playerPresent && Input.GetKeyDown(KeyCode.E) ) {
				EggInactiveRoot.SetActive(false);
				EggActiveRoot.SetActive(true);
				Campfire.Lit();
				ToolTipRoot.SetActive(false);

				AreaNotifier.OnTriggerEnter -= OnAreaEnter;
				AreaNotifier.OnTriggerExit  -= OnAreaExit;

				EnemySpawn.IsLocked = false;
				EnemySpawn.BurstSpawn(StartSpawnedEnemies);

				HatchTimerProgressRoot.SetActive(true);

				_isActive = true;
			}
		}

		void OnAreaEnter(Collider2D other) {
			if ( other.GetComponent<Player>() ) {
				_playerPresent = true;
				if ( !_isActive ) {
					ToolTipRoot.SetActive(true);
				}
			}
		}

		void OnAreaExit(Collider2D other) {
			if ( other.GetComponent<Player>() ) {
				_playerPresent = false;
				ToolTipRoot.SetActive(false);
			}
		}
	}
}
