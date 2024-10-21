using System;
using System.Collections;
using Interaction;
using UnityEngine;
using UnityEngine.Events;

namespace Door {
	public class Door : AbstractDoor {
		public override void Interact(Character character, UnityAction callback) {
			var needs = InteractionNeeds.Get(character);
			ChangeState(needs.CharacterPosition);
			callback?.Invoke();
		}
		
		public override void Interact(Character character, UnityAction callback, string interactionName) {
			if ( interactionName == "opened" ) {
				Interact(character, callback);
			}
			else {
				TryOpen();
				callback?.Invoke();
			}
		}
	}
}