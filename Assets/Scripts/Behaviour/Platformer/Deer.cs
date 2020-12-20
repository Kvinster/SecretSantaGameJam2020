using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Deer : MonoBehaviour {
		static readonly int IsRight   = Animator.StringToHash("IsRight");
		static readonly int IsRunning = Animator.StringToHash("IsRunning");

		public Animator Animator;
		public float    RunSpeed;
		public bool     StartRight;
		public float    DeathTimer;

		bool _isRunning;
		bool _isRight;

		Vector2 _runDirection;

		float _deathTimer;

		void Start() {
			_isRight = StartRight;
			UpdateAnimParams();
		}

		void Update() {
			if ( _isRunning ) {
				transform.Translate(_runDirection.normalized * (RunSpeed * Time.deltaTime));
				_deathTimer += Time.deltaTime;
				if ( _deathTimer > DeathTimer ) {
					Destroy(gameObject);
				}
			}
		}

		void StartRun() {
			_isRunning = true;
			UpdateAnimParams();
		}

		void UpdateAnimParams() {
			Animator.SetBool(IsRunning, _isRunning);
			Animator.SetBool(IsRight, _isRight);
		}

		void OnTriggerEnter2D(Collider2D other) {
			if ( _isRunning ) {
				return;
			}
			if ( other.gameObject.GetComponent<Player>() ) {
				_isRight      = other.transform.position.x < transform.position.x;
				_runDirection = _isRight ? Vector2.right : Vector2.left;
				StartRun();
			}
		}
	}
}
