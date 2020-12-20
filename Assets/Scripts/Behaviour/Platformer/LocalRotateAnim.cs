using UnityEngine;

using DG.Tweening;

namespace SmtProject.Behaviour.Platformer {
	public sealed class LocalRotateAnim : MonoBehaviour {
		public Transform Target;
		public bool      PlayOnEnable;
		public float     Duration;
		public float     Interval;
		public float     Strength;

		Tween _anim;

		void OnDestroy() {
			_anim?.Kill();
		}

		void OnEnable() {
			if ( PlayOnEnable ) {
				Play();
			}
		}

		void Play() {
			_anim?.Kill();
			_anim = DOTween.Sequence()
				.AppendInterval(Interval)
				.Append(Target.DOLocalRotate(new Vector3(0, 0, Strength), Duration / 4f, RotateMode.LocalAxisAdd))
				.Append(Target.DOLocalRotate(new Vector3(0, 0, -2 * Strength), Duration / 2f, RotateMode.LocalAxisAdd))
				.Append(Target.DOLocalRotate(new Vector3(0, 0, Strength), Duration / 4f, RotateMode.LocalAxisAdd))
				.SetLoops(-1);
		}
	}
}
