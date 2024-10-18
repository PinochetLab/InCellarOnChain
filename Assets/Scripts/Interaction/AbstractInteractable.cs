using UnityEngine;

namespace Interaction {
	public abstract class AbstractInteractable : MonoBehaviour, IInteractable {
		public abstract void Interact(Character character);
	}
}