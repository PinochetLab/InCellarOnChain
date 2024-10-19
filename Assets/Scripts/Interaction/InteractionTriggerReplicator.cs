using Interaction.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Interaction {
	public class InteractionTriggerReplicator : AbstractInteractionTrigger {
		[SerializeField] private AbstractInteractionTrigger trigger;
		
		public override void Notify(Character character, InteractionScreen interactionScreen, UnityAction callback) {
			trigger.Notify(character, interactionScreen, callback);
		}

		public AbstractInteractionTrigger Trigger => trigger;
	}
}