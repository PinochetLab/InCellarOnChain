using Interaction;
using UnityEngine;

namespace Hero {
	public class Player : MonoBehaviour, IActor {
		public Vector3 GetPosition() {
			return transform.position;
		}
	}
}