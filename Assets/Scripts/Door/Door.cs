using System;
using System.Collections;
using Interaction;
using UnityEngine;

namespace Door {
	public class Door : AbstractDoor {
		public override void PerformAction(IActor actor) {
			ChangeState(actor);
		}
	}
}