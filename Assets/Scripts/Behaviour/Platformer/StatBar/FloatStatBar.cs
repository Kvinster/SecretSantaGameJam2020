namespace SmtProject.Behaviour.Platformer.StatBar {
	public sealed class FloatStatBar : BaseStatBar<float> {
		protected override void UpdateViewInternal(float value, float minValue, float maxValue) {
			UpdateProgress((value - minValue) / (maxValue - minValue));
		}
	}
}
