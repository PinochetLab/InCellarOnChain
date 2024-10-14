using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Interaction {
	public class ActionTrigger : MonoBehaviour {
		[SerializeField] private List<GameObject> actionablesGameObjects;
		
		private List<IActionable> _actionables;

		private void Awake() {
			_actionables = actionablesGameObjects
				.Select(go => go.GetComponent<IActionable>())
				.Where(a => a != null)
				.ToList();
		}
		
		public Vector3 Position => transform.position;

		public void Notify(IActor actor) {
			_actionables.ForEach(a => a.PerformAction(actor));
		}
	}
}