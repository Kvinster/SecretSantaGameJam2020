using SmtProject.Behaviour.Utils;

namespace SmtProject.Behaviour {
	public sealed class HealthBar : BaseProgressBar {
		public Dummy Dummy;

		int _maxHp;

		protected override void Start() {
			base.Start();
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
			UpdateProgress(progress);
		}
	}
}
