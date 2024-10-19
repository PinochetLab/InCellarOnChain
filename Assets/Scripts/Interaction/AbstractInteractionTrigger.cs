using Interaction.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Interaction {
	public abstract class AbstractInteractionTrigger : MonoBehaviour, IInteractionTrigger {
		public abstract void Notify(Character character, InteractionScreen interactionScreen, UnityAction callback);
		
		public Vector3 HintPoint => transform.position;

		public static bool IsNull(AbstractInteractionTrigger trigger) {
			if ( !trigger ) {
				return true;
			}

			var itr = trigger as InteractionTriggerReplicator;
			if ( itr && itr.Trigger is null ) {
				return true;
			}
			return false;
		}
	}
}