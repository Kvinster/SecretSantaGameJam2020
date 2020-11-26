using UnityEngine;

using DG.Tweening;
using Shapes;

namespace SmtProject.Behaviour.Cortex {
	public sealed class TransporterController : MonoBehaviour {
		public Rigidbody2D BodyRigidbody;
		public float       RotationSpeed;
		[Space]
		public Disc  BodyDisc;
		public float OpenStartAngle;
		public float OpenEndAngle;
		public float ClosedStartAngle;
		public float ClosedEndAngle;
		public float DoorsAnimDuration = 0.5f;
		[Space]
		public Engine LeftEngine;
		public Engine RightEngine;
		[Space]
		public float      SpawnDelay = 2f;
		public Transform  BombSpawnPos;
		public GameObject BombPrefab;

		bool _isOpen;

		Sequence _doorsAnim;

		float _spawnTimer;

		void Update() {
			if ( Input.GetKey(KeyCode.W) ) {
				LeftEngine.Fire();
				RightEngine.Fire();
			}
			if ( Input.GetKey(KeyCode.Q) ) {
				AddTorque();
			} else if ( Input.GetKey(KeyCode.E) ) {
				AddTorque(true);
			}
			if ( Input.GetKeyDown(KeyCode.Space) ) {
				ToggleOpen();
			}
			if ( _isOpen ) {
				if ( _spawnTimer <= 0 ) {
					SpawnBomb();
					_spawnTimer += SpawnDelay;
				}
				_spawnTimer -= Time.deltaTime;
			}
		}

		void AddTorque(bool negate = false) {
			BodyRigidbody.AddTorque(RotationSpeed * (negate ? -1f : 1f), ForceMode2D.Force);
		}

		void ToggleOpen() {
			_doorsAnim?.Kill();
			_doorsAnim = DOTween.Sequence();
			var totalAngle = Mathf.Abs(OpenStartAngle - ClosedStartAngle);
			var duration = _isOpen
				? (DoorsAnimDuration * (BodyDisc.AngRadiansStart * Mathf.Rad2Deg - ClosedStartAngle) / totalAngle)
				: (DoorsAnimDuration * (BodyDisc.AngRadiansStart * Mathf.Rad2Deg - OpenStartAngle) / totalAngle);
			duration = Mathf.Abs(duration);
			_doorsAnim
				.Insert(0f,
					DOTween.To(() => BodyDisc.AngRadiansStart, angle => {
						BodyDisc.AngRadiansStart = angle;
					}, (_isOpen ? ClosedStartAngle : OpenStartAngle) * Mathf.Deg2Rad, duration))
				.Insert(0f,
					DOTween.To(() => BodyDisc.AngRadiansEnd, angle => {
						BodyDisc.AngRadiansEnd = angle;
					}, (_isOpen ? ClosedEndAngle : OpenEndAngle) * Mathf.Deg2Rad, duration));
			_isOpen = !_isOpen;
			if ( _isOpen ) {
				_spawnTimer = duration + 0.1f;
			}
		}

		void SpawnBomb() {
			var bombGo = Instantiate(BombPrefab, null);
			bombGo.transform.position = BombSpawnPos.position;
			bombGo.transform.rotation = transform.rotation;
			var bombRb = bombGo.GetComponent<Rigidbody2D>();
			bombRb.velocity = BodyRigidbody.velocity + (Vector2) transform.TransformDirection(Vector3.down) * 5f;
		}
	}
}
