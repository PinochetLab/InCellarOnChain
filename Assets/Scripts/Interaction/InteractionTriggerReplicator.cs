using Interaction.UI;
using UnityEngine;

namespace Interaction {
	public class InteractionTriggerReplicator : AbstractInteractionTrigger {
		[SerializeField] private AbstractInteractionTrigger trigger;
		
		public override void Notify(Character character, InteractionScreen interactionScreen, SelectCallback callback) {
			trigger.Notify(character, interactionScreen, callback);
		}

		public AbstractInteractionTrigger Trigger => trigger;
	}
}