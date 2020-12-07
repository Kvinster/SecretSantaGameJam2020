using System;

using UnityEngine;

using DG.Tweening;

namespace SmtProject.Utils.ValueAnim {
	public abstract class BaseValueAnim {
		const float AnimDuration = 1f;

		float _prevValue;
		float _nextValue;

		float _curValue;

		protected Tween ProgressAnim;

		public float CurValue {
			get => _curValue;
			private set {
				if ( Mathf.Approximately(_curValue, value) ) {
					return;
				}
				_curValue = value;
				OnCurValueChanged?.Invoke(_curValue);
			}
		}

		public float CurProgress => Mathf.Approximately(0f, _nextValue - _prevValue)
			? 0f
			: Mathf.Clamp01((CurValue - _prevValue) / (_nextValue - _prevValue));

		public event Action<float> OnCurValueChanged;

		protected BaseValueAnim(float startValue) {
			_prevValue = _nextValue = CurValue = startValue;
		}

		public virtual void SetNextValue(float nextValue) {
			ProgressAnim?.Kill();

			_prevValue = CurValue;
			_nextValue = nextValue;
			ProgressAnim = DOTween.To(() => CurValue, value => {
				CurValue = value;
			}, _nextValue, AnimDuration).SetEase(Ease.OutSine);
		}
	}
}
