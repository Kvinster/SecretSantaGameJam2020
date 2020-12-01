using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

namespace SmtProject.Behaviour.Platformer {
	public sealed class FadeTransition : MonoBehaviour {
		enum State {
			Left,
			Middle,
			Right
		}

		public Transform Target;
		public Graphic   LeftGraphic;
		public Graphic   RightGraphic;
		public Transform LeftBorder;
		public Transform RightBorder;
		public float     FadeDuration;

		State _curState;

		bool _transitionPassed;

		float _leftBorderX;
		float _rightBorderX;

		Tween _fadeAnim;

		void Start() {
			_leftBorderX  = LeftBorder.position.x;
			_rightBorderX = RightBorder.position.x;

			UpdateCurState();

			if ( _curState == State.Right ) {
				_transitionPassed = true;
			}

			if ( _curState == State.Left ) {
				SetGraphicAlpha(LeftGraphic, 1f);
				SetGraphicAlpha(RightGraphic, 0f);
			} else {
				SetGraphicAlpha(LeftGraphic, 0f);
				SetGraphicAlpha(RightGraphic, 1f);
			}
		}

		void UpdateCurState() {
			var targetX = Target.position.x;
			if ( targetX < _leftBorderX ) {
				_curState = State.Left;
			} else if ( targetX < _rightBorderX ) {
				_curState = State.Middle;
			} else {
				_curState = State.Right;
			}
		}

		void Update() {
			var oldState = _curState;
			UpdateCurState();
			if ( oldState == _curState ) {
				return;
			}
			if ( _curState == State.Middle ) {
				return;
			}
			if ( !_transitionPassed && (_curState == State.Left) ) {
				return;
			}
			if ( _transitionPassed && (_curState == State.Right) ) {
				return;
			}
			var fadeOutGraphic = (_curState == State.Right) ? LeftGraphic  : RightGraphic;
			var fadeInGraphic  = (_curState == State.Right) ? RightGraphic : LeftGraphic;
			_fadeAnim?.Kill();
			_fadeAnim = DOTween.Sequence()
				.Insert(0f, fadeOutGraphic.DOFade(0f, FadeDuration))
				.Insert(0f, fadeInGraphic.DOFade(1f, FadeDuration));
			_transitionPassed = (_curState == State.Right);
		}

		static void SetGraphicAlpha(Graphic graphic, float alpha) {
			var oldColor = graphic.color;
			graphic.color = new Color(oldColor.r, oldColor.g, oldColor.b, alpha);
		}
	}
}
