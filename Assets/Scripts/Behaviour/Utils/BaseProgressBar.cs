using UnityEngine;

using Shapes;

namespace SmtProject.Behaviour.Utils {
	public abstract class BaseProgressBar : MonoBehaviour {
		public Rectangle Background;
		public Rectangle Foreground;
		[Space]
		public Color FullColor;
		public Color EmptyColor;

		float _maxWidth;

		float Progress {
			set => Foreground.Width = Mathf.Clamp01(value) * _maxWidth;
		}

		protected virtual void Start() {
			_maxWidth = Background.Width;

			UpdateProgress(1f);
		}

		protected void UpdateProgress(float progress) {
			Progress         = progress;
			Foreground.Color = Color.Lerp(EmptyColor, FullColor, progress);
		}
	}
}
