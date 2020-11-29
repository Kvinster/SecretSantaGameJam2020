using SmtProject.Behaviour.Utils;

namespace SmtProject.Behaviour.Platformer {
	public sealed class PlayerHealthBar : BaseProgressBar {
		public PlayerController Player;

		int _maxHp;

		protected override void Start() {
			base.Start();
			_maxHp = Player.MaxHp;

			Player.OnCurHpChanged += OnPlayerHpChanged;
			UpdateProgress(Player.CurHp);
		}

		void OnDestroy() {
			if ( Player ) {
				Player.OnCurHpChanged -= OnPlayerHpChanged;
			}
		}

		void OnPlayerHpChanged(int hp) {
			UpdateProgress(hp);
		}

		void UpdateProgress(int hp) {
			UpdateProgress((float) hp / _maxHp);
		}
	}
}
