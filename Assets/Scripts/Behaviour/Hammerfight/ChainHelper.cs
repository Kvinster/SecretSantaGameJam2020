using UnityEngine;

using System.Collections.Generic;

namespace SmtProject.Behaviour.Hammerfight {
	public static class ChainHelper {
		const string HeadJointPrefabPath = "Prefabs/HeadJoint";
		const string JointPrefabPath     = "Prefabs/Joint";

		static GameObject _headJointPrefab;
		static GameObject _jointPrefab;

		public static void DestroyChain(Chain chain) {
			if ( !chain ) {
				Debug.LogWarning("Chain is null");
				return;
			}
			Object.Destroy(chain.gameObject);
		}

		public static Chain CreateChain(int length, Rigidbody2D origin) {
			if ( length <= 0 ) {
				Debug.LogErrorFormat("Invalid chain length '{0}'", length);
				return null;
			}
			if ( !origin ) {
				Debug.LogError("Origin is null");
				return null;
			}

			var chainParentGo = new GameObject("Chain");
			var chainParent   = chainParentGo.transform;
			chainParent.SetParent(origin.transform);
			chainParent.localPosition = Vector3.zero;

			var joints    = new List<ChainJoint>(length);
			var headJoint = CreateHeadJoint(chainParent);
			headJoint.Init(origin, true);
			joints.Add(headJoint);

			var prevJoint = headJoint.Rigidbody;
			for ( var i = 0; i < length - 1; ++i ) {
				var joint = CreateJoint(chainParent);
				joint.Init(prevJoint);
				prevJoint = joint.Rigidbody;
				joints.Add(joint);
			}

			var chain = chainParentGo.AddComponent<Chain>();
			chain.Joints = joints;
			return chain;
		}

		static ChainJoint CreateHeadJoint(Transform parent) {
			_headJointPrefab = TryCreatePrefab(_headJointPrefab, HeadJointPrefabPath);
			return CreateInstance<ChainJoint>(_headJointPrefab, parent);
		}

		static ChainJoint CreateJoint(Transform parent) {
			_jointPrefab = TryCreatePrefab(_jointPrefab, JointPrefabPath);
			return CreateInstance<ChainJoint>(_jointPrefab, parent);
		}

		static GameObject TryCreatePrefab(GameObject prefabValue, string prefabPath) {
			if ( prefabValue ) {
				return prefabValue;
			}
			var prefab = Resources.Load<GameObject>(prefabPath);
			if ( !prefab ) {
				Debug.LogErrorFormat("Can't load prefab for path '{0}'", prefabPath);
			}
			return prefab;
		}

		static T CreateInstance<T>(GameObject prefab, Transform parent) where T : Object {
			if ( !prefab ) {
				Debug.LogError("Prefab is null");
				return null;
			}
			var go = Object.Instantiate(prefab, parent);
			go.transform.localPosition = Vector3.zero;
			go.transform.rotation      = Quaternion.identity;
			if ( go ) {
				return go.GetComponent<T>();
			}
			Debug.LogError("Can't instantiate prefab");
			return null;
		}
	}
}
