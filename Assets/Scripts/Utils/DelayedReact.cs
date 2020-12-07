using UnityEngine;

using System;

using RSG;

namespace SmtProject.Utils {
	public sealed class DelayedReact : IDisposable {
		public static DelayedReact Instance { get; private set; }

		readonly Promise _promise;

		public IPromise Promise => _promise;

		public DelayedReact() {
			if ( Instance == null ) {
				Instance = this;
			} else {
				Debug.LogError("Another instance of DelayerReact exists");
			}

			_promise = new Promise();
		}

		public void Dispose() {
			if ( Instance == this ) {
				Instance = null;
			}
			_promise.Resolve();
		}

		public static implicit operator bool(DelayedReact delayedReact) {
			return (delayedReact != null);
		}
	}
}
