using UnityEngine;

namespace SmtProject.Behaviour.Hammerfight {
	public sealed class ChainCreator : MonoBehaviour {
		public Rigidbody2D Origin;
		public int         ChainLength = 5;

		void Update() {
			if ( Input.GetKeyDown(KeyCode.Space) ) {
				var weightPrefab = Resources.Load<GameObject>("Prefabs/Weight");
				var weightGo     = Instantiate(weightPrefab, null, false);
				weightGo.transform.position = Origin.transform.position;
				var weight = weightGo.GetComponent<Weight>();
				weight.DistanceJoint.connectedBody = Origin;
				var chain = ChainHelper.CreateChain(ChainLength, Origin);
				if ( !chain ) {
					return;
				}
				var weightDistanceJoint = weight.DistanceJoint;
				var weightHingeJoint    = weight.HingeJoint;
				var weightSpringJoint   = weight.SpringJoint;
				weightDistanceJoint.connectedBody = Origin;
				weightDistanceJoint.enabled       = true;
				weightSpringJoint.connectedBody   = Origin;
				weightSpringJoint.enabled         = true;
				var lastJointRigidbody = chain.LastJoint.Rigidbody;
				weightHingeJoint.connectedBody = lastJointRigidbody;
				weightHingeJoint.enabled       = true;
			}
		}
	}
}
