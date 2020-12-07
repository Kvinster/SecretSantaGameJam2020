using System;

using DG.Tweening;

using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public sealed class XpValueAnim {
		int   _curLevel;
		float _curXp;
		float _nextLevelXp;

		readonly float _animDuration;

		Tween _anim;

		public float CurXp {
			get => _curXp;
			private set {
				if ( Mathf.Approximately(_curXp, value) ) {
					return;
				}
				_curXp = value;
				OnCurValueChanged?.Invoke(_curXp);
			}
		}

		public int CurLevel {
			get => _curLevel;
			private set {
				if ( _curLevel == value ) {
					return;
				}
				_curLevel = value;
				OnCurLevelChanged?.Invoke(_curLevel);
			}
		}

		public event Action<float> OnCurValueChanged;
		public event Action<int>   OnCurLevelChanged;

		public XpValueAnim(float startXp, int startLevel, float animDuration = 1f) {
			CurXp    = startXp;
			CurLevel = startLevel;

			_nextLevelXp = startXp;

			_animDuration = animDuration;
		}

		public void SetNextValue(float nextValue) {
			_nextLevelXp = nextValue;
			_anim        = DOTween.To(() => CurXp, xp => { CurXp = xp; }, _nextLevelXp, _animDuration);
		}

		public void SetNextLevel(float nextValue, int nextLevel) {
			if ( _anim?.IsActive() ?? false ) {
				_anim.onComplete += () => {
					CurLevel = nextLevel;
					SetNextValue(nextValue);
					_anim = null;
				};
			} else {
				SetNextValue(nextValue);
			}
		}
	}
}
