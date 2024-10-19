using UnityEngine.Events;

namespace Interaction {
	public interface IInteractable {
		void Interact(Character character, UnityAction callback);

		void Interact(Character character, UnityAction callback, string interactionName) {
			Interact(character, callback);
		}
	}
}