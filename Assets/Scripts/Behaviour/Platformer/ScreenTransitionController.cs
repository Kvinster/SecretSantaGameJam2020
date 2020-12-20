using UnityEngine;
using UnityEngine.SceneManagement;

using System;

using DG.Tweening;

namespace SmtProject.Behaviour.Platformer {
	public sealed class ScreenTransitionController : MonoBehaviour {
		static readonly int Progress = Shader.PropertyToID("_Progress");
		static readonly int Center   = Shader.PropertyToID("_Center");

		public static ScreenTransitionController Instance { get; private set; }

		public SpriteRenderer SpriteRenderer;
		public float          FadeInDuration;
		public float          FadeOutDuration;

		Sequence _anim;

		MaterialPropertyBlock _mpb;
		MaterialPropertyBlock MaterialPropertyBlock {
			get {
				if ( _mpb == null ) {
					_mpb = new MaterialPropertyBlock();
					SpriteRenderer.GetPropertyBlock(_mpb);
				}
				return _mpb;
			}
		}

		void Awake() {
			if ( Instance ) {
				Destroy(gameObject);
			} else {
				DontDestroyOnLoad(gameObject);
				Instance = this;
			}
		}

		void Update() {
			var cameraPos = Camera.main.transform.position;
			transform.position = new Vector3(cameraPos.x, cameraPos.y, 0);
		}

		public void Transition(string sceneName, Vector3 fadeInWorldCenter) {
			Transition(sceneName, fadeInWorldCenter, () => Camera.main.transform.TransformPoint(Vector3.zero));
		}

		public void Transition(string sceneName, Vector3 fadeInWorldCenter, Func<Vector3> fadeOutWorldCenterGetter) {
			if ( _anim != null ) {
				Debug.LogError("Scene transition already playing");
				return;
			}
			var fadeInScreenCenter = Camera.main.WorldToScreenPoint(fadeInWorldCenter) -
			                         new Vector3(Screen.width / 2f, Screen.height / 2f);
			MaterialPropertyBlock.SetVector(Center, fadeInScreenCenter);
			SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
			var progress = 0f;
			_anim = DOTween.Sequence()
				.Append(DOTween.To(() => progress, x => {
					progress = x;
					MaterialPropertyBlock.SetFloat(Progress, progress);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				}, 1f, FadeInDuration))
				.AppendCallback(() => {
					SceneManager.LoadScene(sceneName);
				})
				.AppendInterval(0.1f)
				.AppendCallback(() => {
					var fadeOutScreenCenter = Camera.main.WorldToScreenPoint(fadeOutWorldCenterGetter()) -
					                          new Vector3(Screen.width / 2f, Screen.height / 2f);
					MaterialPropertyBlock.SetVector(Center, fadeOutScreenCenter);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				})
				.Append(DOTween.To(() => progress, x => {
					progress = x;
					MaterialPropertyBlock.SetFloat(Progress, progress);
					SpriteRenderer.SetPropertyBlock(MaterialPropertyBlock);
				}, 0f, FadeOutDuration))
				.AppendCallback(() => _anim = null);
		}
	}
}
