using Interaction;
using Inventory;
using UnityEngine;

namespace Interaсtive {
	public class CollectableItem : AbstractInteractable {
		[SerializeField] private Item item;

		public override void Interact(Character character) {
			Debug.Log("CollectableItem");
			var inventory = InteractionNeeds.Get(character).Inventory;
			inventory.TryAddItem(item, ProcessResult);
		}

		private void ProcessResult(AddItemResult result) {
			Debug.Log($"Result " + result.Success);
			if ( result.Success ) {
				DestroyImmediate(gameObject);
			}
		}
	}
}