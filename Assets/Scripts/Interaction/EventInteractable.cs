using UnityEngine;
using UnityEngine.Events;

namespace Interaction {
	public class EventInteractable : MonoBehaviour, IInteractable {
		[SerializeField] private UnityEvent onInteract;
		public Vector3 Position => transform.position;

		public void Interact() {
			onInteract.Invoke();
		}
	}
}