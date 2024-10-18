using Interaction.UI;

namespace Interaction {
	public delegate void SelectCallback();
	public interface IInteractionTrigger {
		void Notify(Character character, InteractionScreen interactionScreen, SelectCallback callback);
	}
}