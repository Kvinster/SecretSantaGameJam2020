using UnityEngine;
using UnityEngine.VFX;

namespace SmtProject.Behaviour.Platformer {
	public sealed class Campfire : MonoBehaviour {
		static readonly int LitHash   = Animator.StringToHash("Lit");
		static readonly int UnlitHash = Animator.StringToHash("Unlit");

		public Animator     BackgroundAnimator;
		public Animator     ForegroundAnimator;
		public VisualEffect Fx;

		public void Lit() {
			BackgroundAnimator.SetTrigger(LitHash);
			ForegroundAnimator.SetTrigger(LitHash);
			Fx.SendEvent("Lit");
		}

		public void Extinguish() {
			BackgroundAnimator.ResetTrigger(LitHash);
			ForegroundAnimator.ResetTrigger(LitHash);
			BackgroundAnimator.SetTrigger(UnlitHash);
			ForegroundAnimator.SetTrigger(UnlitHash);
			Fx.SendEvent("Extinguish");
		}
	}
}
