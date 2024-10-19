using System.Collections.Generic;
using Interaction;
using Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Interaсtive {
	public class Chest : AbstractInteractable {
		[SerializeField] private List<Item> items;
		
		private readonly GridInventory _chestGridInventory = new ();

		private void Awake() {
			foreach ( var item in items ) {
				_chestGridInventory.TryAutoAddItem(item);
			}
		}

		public override void Interact(Character character, UnityAction callback) {
			var keeper = InteractionNeeds.Get(character).Keeper;
			keeper.StartAddItems(_chestGridInventory, OnEnd);
			return;
			
			void OnEnd() {
				callback.Invoke();
			}
		}
	}
}