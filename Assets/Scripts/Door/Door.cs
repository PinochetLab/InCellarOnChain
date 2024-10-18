using System;
using System.Collections;
using Interaction;
using UnityEngine;

namespace Door {
	public class Door : AbstractDoor {
		public override void Interact(Character character) {
			var needs = InteractionNeeds.Get(character);
			ChangeState(needs.CharacterPosition);
		}
	}
}