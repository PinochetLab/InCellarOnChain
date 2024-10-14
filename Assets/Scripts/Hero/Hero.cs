using Interaction;
using UnityEngine;

namespace Hero {
	public class Hero : MonoBehaviour, IActor {
		public Vector3 GetPosition() {
			return transform.position;
		}
	}
}