using UnityEngine;

using DG.Tweening;

namespace SmtProject.Behaviour.Platformer {
	public sealed class LocalModeAnim : MonoBehaviour {
		public float Strength;
		public float Duration;

		Tween _anim;

		void OnEnable() {
			transform.localPosition = Vector3.up * Strength;
			_anim = DOTween.Sequence()
				.Append(transform.DOLocalMove(Vector3.down * Strength, Duration / 2f))//.SetEase(Ease.InOutSine)
				.Append(transform.DOLocalMove(Vector3.up * Strength, Duration / 2f))//.SetEase(Ease.InOutSine)
				.SetLoops(-1);
		}

		void OnDisable() {
			_anim?.Kill();
			transform.localPosition = Vector3.zero;
		}
	}
}
