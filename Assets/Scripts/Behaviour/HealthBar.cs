using UnityEngine;

using Shapes;

namespace SmtProject.Behaviour {
	public sealed class HealthBar : MonoBehaviour {
		public Dummy     Dummy;
		public Rectangle Background;
		public Rectangle Foreground;
		[Space]
		public Color FullColor;
		public Color EmptyColor;

		float _maxValue;
		int   _maxHp;

		float Progress {
			set => Foreground.Width = Mathf.Clamp01(value) * _maxValue;
		}

		void Start() {
			_maxValue = Background.Width;
			_maxHp    = Dummy.MaxHp;

			Dummy.OnCurHpChanged += OnHpChanged;
			UpdateProgress(Dummy.CurHp);
		}

		void OnDestroy() {
			if ( Dummy ) {
				Dummy.OnCurHpChanged -= OnHpChanged;
			}
		}

		void OnHpChanged(int hp) {
			UpdateProgress(hp);
		}

		void UpdateProgress(int hp) {
			var progress = (float) hp / _maxHp;
			Progress         = progress;
			Foreground.Color = Color.Lerp(EmptyColor, FullColor, progress);
		}
	}
}
