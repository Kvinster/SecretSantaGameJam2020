namespace SmtProject.Behaviour.Platformer {
	public interface IPlayerAnimationController {
		void UpdateAnimParams(bool isAlive, bool isWalking, bool isHitting, int walkDir);
		void SetTrigger(int hash);
		void ResetTrigger(int hash);
	}
}
