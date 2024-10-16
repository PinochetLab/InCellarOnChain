using System;
using InputSystem;
using UnityEngine;

namespace Inventory {
	public class InventoryController : MonoBehaviour {
		[SerializeField] private GameObject inventoryPanel;

		private bool _isInventoryOpen;

		private void Update() {
			if ( Input.GetButtonDown("Inventory") ) {
				_isInventoryOpen = !_isInventoryOpen;
				GameInputManager.SetMode(_isInventoryOpen ? Mode.UI : Mode.Game);
				inventoryPanel.SetActive( _isInventoryOpen );
			}
		}
	}
}