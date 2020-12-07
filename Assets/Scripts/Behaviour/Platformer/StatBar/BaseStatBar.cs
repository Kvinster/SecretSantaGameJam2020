using SmtProject.Behaviour.Utils;

namespace SmtProject.Behaviour.Platformer.StatBar {
	public abstract class BaseStatBar<T> : BaseProgressBar {
		T _minValue;
		T _maxValue;

		public void Init(T startValue, T minValue, T maxValue) {
			_minValue = minValue;
			_maxValue = maxValue;

			UpdateView(startValue);
		}

		public void UpdateView(T value) {
			UpdateView(value, _minValue, _maxValue);
		}

		public void UpdateView(T value, T minValue, T maxValue) {
			_minValue = minValue;
			_maxValue = maxValue;
			UpdateViewInternal(value, _minValue, _maxValue);
		}

		protected abstract void UpdateViewInternal(T value, T minValue, T maxValue);
	}
}
