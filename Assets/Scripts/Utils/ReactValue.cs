using System;

namespace SmtProject.Utils {
	public sealed class ReactValue<T> where T : struct {
		T _curValue;

		public T CurValue {
			get => _curValue;
			private set {
				if ( _curValue.Equals(value) ) {
					return;
				}
				_curValue = value;
				if ( DelayedReact.Instance ) {
					DelayedReact.Instance.Promise.Then(() => OnCurValueChanged?.Invoke(_curValue));
				} else {
					OnCurValueChanged?.Invoke(_curValue);
				}
			}
		}

		public event Action<T> OnCurValueChanged;

		public ReactValue() : this(default) { }

		public ReactValue(T startValue) {
			_curValue = startValue;
		}

		public void SetValue(T value) {
			CurValue = value;
		}

		public static implicit operator T(ReactValue<T> a) {
			return a.CurValue;
		}

		public static bool operator ==(ReactValue<T> a, ReactValue<T> b) {
			if ( (a == null) || (b == null) ) {
				return ReferenceEquals(a, b);
			}
			return a.CurValue.Equals(b.CurValue);
		}

		public static bool operator !=(ReactValue<T> a, ReactValue<T> b) {
			return !(a == b);
		}

		bool Equals(ReactValue<T> other) {
			return _curValue.Equals(other._curValue);
		}

		public override bool Equals(object obj) {
			return ReferenceEquals(this, obj) || obj is ReactValue<T> other && Equals(other);
		}

		public override int GetHashCode() {
			return _curValue.GetHashCode();
		}
	}
}
