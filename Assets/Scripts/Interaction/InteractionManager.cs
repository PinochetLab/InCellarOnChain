using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interaction {
	public class InteractionManager : MonoBehaviour {
		[SerializeField] private RectTransform hint;
		
		private static List<IInteractable> _interactables = new();
		private void OnTriggerEnter(Collider other) {
			var interactable = other.GetComponent<IInteractable>();
			if ( interactable != null ) {
				_interactables.Add(interactable);
			}
		}
		
		private void OnTriggerExit(Collider other) {
			var interactable = other.GetComponent<IInteractable>();
			if ( interactable != null ) {
				_interactables.Remove(interactable);
			}
		}

		private void Update() {
			if ( _interactables.Count == 0 ) {
				hint.gameObject.SetActive(false);
				return;
			}
			hint.gameObject.SetActive(true);
			var pos = _interactables[0].Position;
			hint.anchoredPosition = UnityEngine.Camera.main.WorldToScreenPoint(pos);
			if ( Input.GetButtonDown("Interact") ) {
				_interactables[0].Interact();
			}
		}
	}
}