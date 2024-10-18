using Interaction.UI;
using UnityEngine;

namespace Interaction {
	public abstract class AbstractInteractionTrigger : MonoBehaviour, IInteractionTrigger {
		public abstract void Notify(Character character, InteractionScreen interactionScreen, SelectCallback callback);
		
		public Vector3 HintPoint => transform.position;

		public static bool IsNull(AbstractInteractionTrigger trigger) {
			if ( trigger is null ) {
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