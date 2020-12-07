namespace SmtProject.Behaviour.Platformer.StatBar {
	public sealed class IntStatBar : BaseStatBar<int> {
		protected override void UpdateViewInternal(int value, int minValue, int maxValue) {
			UpdateProgress((float) (value - minValue) / (maxValue - minValue));
		}
	}
}
