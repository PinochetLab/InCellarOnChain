using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Display {
	public class PlayerInventoryDisplay : MonoBehaviour {

		[SerializeField] private GameObject emptySlotPrefab;
		
		[SerializeField] private GameObject itemSlotPrefab;

		[SerializeField] private Transform emptySlotParent;
		
		[SerializeField] private Transform itemSlotParent;
		
		private PlayerInventory _inventory;

		public void SetUp(PlayerInventory inventory, List<List<InventorySlot>> slots) {
			_inventory = inventory;
			
			while ( emptySlotParent.childCount > 0 ) {
				DestroyImmediate(emptySlotParent.GetChild(0).gameObject);
			}
			
			for ( var y = 0; y < slots.Count; y++ ) {
				var row = slots[y];
				for ( var x = 0; x < row.Count; x++ ) {
					var slot = row[x];
					if ( slot.IsDenied() ) {
						continue;
					}
					var emptySlotGo = Instantiate(emptySlotPrefab, emptySlotParent);
					var emptySlotDisplay = emptySlotGo.GetComponent<EmptySlotDisplay>();
					
					emptySlotDisplay.SetCellPosition(new Vector2Int(x, y));
					emptySlotDisplay.SetCellSize(Vector2Int.one);
				}
			}
		}

		public void UpdateItems(Dictionary<Item, List<ItemTransform>> itemTransforms) {

			while ( itemSlotParent.childCount > 0 ) {
				DestroyImmediate(itemSlotParent.GetChild(0).gameObject);
			}

			foreach ( var (item, transforms) in itemTransforms ) {
				foreach ( var itemTransform in transforms ) {
					var itemSlotGo = Instantiate(itemSlotPrefab, itemSlotParent);
					var itemSlotDisplay = itemSlotGo.GetComponent<ItemSlotDisplay>();
					
					itemSlotDisplay.SetUp(item, itemTransform, _inventory);
				
					itemSlotDisplay.SetCellPosition(itemTransform.Position);
					itemSlotDisplay.SetCellSize(itemTransform.RotatedSize(item));
				}
			}
		}
	}
}