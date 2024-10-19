using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Action;
using Interaction.UI;
using Inventory;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interaction {
	public class PlayerInteractionManager : MonoBehaviour {
		[SerializeField] private InteractionScreen interactionScreen;
		
		private readonly List<AbstractInteractionTrigger> _triggers = new();

		private bool _available = true;

		private void OnTriggerEnter(Collider other) {
			var trigger = other.GetComponent<AbstractInteractionTrigger>();
			if ( trigger ) {
				_triggers.Add(trigger);
			}
		}
		
		private void OnTriggerExit(Collider other) {
			var trigger = other.GetComponent<AbstractInteractionTrigger>();
			if ( trigger ) {
				_triggers.Remove(trigger);
			}
		}

		private void Update() {
			if (!_available) return;
			_triggers.RemoveAll(AbstractInteractionTrigger.IsNull);
			ProcessHint();
			ProcessInteraction();
		}

		private void ProcessHint() {
			if ( _triggers.Count == 0 ) {
				interactionScreen.SetActive(false);
				return;
			}
			interactionScreen.SetActive(true);
			interactionScreen.SetHintPosition(_triggers[0].HintPoint);
		}

		private void ProcessInteraction() {
			if ( _triggers.Count == 0 ) return;
			if ( !Input.GetButtonDown("Interact") ) return;
			//interactionScreen.SetActive(false);
			_available = false;
			_triggers[0].Notify(Character.Player, interactionScreen, OnSelect);
		}

		private void OnSelect() {
			StartCoroutine(WaitForFrame());
		}
		
		private IEnumerator WaitForFrame() {
			yield return new WaitUntil(() => !Input.GetButtonDown("Action"));
			_available = true;
		}
	}
}