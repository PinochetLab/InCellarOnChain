namespace Interaction {
	public interface IInteractable {
		void Interact(Character character);

		void Interact(Character character, string interactionName) {
			Interact(character);
		}
	}
}