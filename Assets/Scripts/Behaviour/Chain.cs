using UnityEngine;

using System.Collections.Generic;

namespace SmtProject.Behaviour {
	public sealed class Chain : MonoBehaviour {
		public List<ChainJoint> Joints = new List<ChainJoint>();

		public ChainJoint LastJoint => ((Joints != null) && (Joints.Count > 0)) ? Joints[Joints.Count - 1] : null;
	}
}
