using UnityEngine;

using System.Collections.Generic;

namespace SmtProject.Behaviour.Platformer {
	[RequireComponent(typeof(AudioSource))]
	public sealed class RandomSoundPlayer : MonoBehaviour {
		public List<AudioClip> AudioClips = new List<AudioClip>();

		AudioSource _audioSource;

		bool IsInit => _audioSource;

		void Start() {
			TryInit();
		}

		public void Play() {
			TryInit();
			if ( AudioClips.Count == 0 ) {
				Debug.LogError("Can's play random sound — AudioClips is empty");
				return;
			}
			_audioSource.PlayOneShot(AudioClips[Random.Range(0, AudioClips.Count)]);
		}

		void TryInit() {
			if ( IsInit ) {
				return;
			}
			_audioSource = GetComponent<AudioSource>();
		}
	}
}
