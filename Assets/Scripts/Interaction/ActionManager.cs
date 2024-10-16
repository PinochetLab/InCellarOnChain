using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Interaction {
	public class ActionManager : MonoBehaviour {
		
		[FormerlySerializedAs("hero")] [SerializeField] private Hero.Player player;
		
		[SerializeField] private RectTransform hint;
		
		private static readonly List<ActionTrigger> ActionTriggers = new();
		private void OnTriggerEnter(Collider other) {
			var actionTrigger = other.GetComponent<ActionTrigger>();
			if ( actionTrigger ) {
				ActionTriggers.Add(actionTrigger);
			}
		}
		
		private void OnTriggerExit(Collider other) {
			var actionTrigger = other.GetComponent<ActionTrigger>();
			if ( actionTrigger ) {
				ActionTriggers.Remove(actionTrigger);
			}
		}

		private void Update() {
			UpdateHint();
			CheckAction();
		}

		private void UpdateHint() {
			if ( ActionTriggers.Count == 0 ) {
				hint.gameObject.SetActive(false);
				return;
			}
			hint.gameObject.SetActive(true);
			hint.anchoredPosition = UnityEngine.Camera.main.WorldToScreenPoint(ActionTriggers[0].Position);
		}

		private void CheckAction() {
			if ( !Input.GetButtonDown("Action") ) return;
			if ( ActionTriggers.Count == 0) return;
			ActionTriggers[0].Notify(player);
		}
	}
}