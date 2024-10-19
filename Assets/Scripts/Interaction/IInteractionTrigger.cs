using Interaction.UI;
using UnityEngine.Events;

namespace Interaction {
	public delegate void SelectCallback();
	public interface IInteractionTrigger {
		void Notify(Character character, InteractionScreen interactionScreen, UnityAction callback);
	}
}