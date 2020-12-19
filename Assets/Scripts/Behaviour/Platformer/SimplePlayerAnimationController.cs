using UnityEngine;

namespace SmtProject.Behaviour.Platformer {
	public sealed class SimplePlayerAnimationController : MonoBehaviour, IPlayerAnimationController {
		static readonly int IsAliveHash   = Animator.StringToHash("IsAlive");
		static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");
		static readonly int IsHittingHash = Animator.StringToHash("IsHitting");
		static readonly int WalkDirHash   = Animator.StringToHash("WalkDir");

		public Animator Animator;

		bool IsActive => gameObject.activeInHierarchy;

		void Reset() {
			Animator = GetComponentInChildren<Animator>();
		}

		public void UpdateAnimParams(bool isAlive, bool isWalking, bool isHitting, int walkDir) {
			if ( !IsActive ) {
				return;
			}
			Animator.SetBool(IsAliveHash, isAlive);
			Animator.SetBool(IsWalkingHash, isWalking);
			Animator.SetBool(IsHittingHash, isHitting);
			Animator.SetInteger(WalkDirHash, walkDir);
		}

		public void SetTrigger(int hash) {
			if ( !IsActive ) {
				return;
			}
			Animator.SetTrigger(hash);
		}

		public void ResetTrigger(int hash) {
			if ( !IsActive ) {
				return;
			}
			Animator.ResetTrigger(hash);
		}
	}
}
