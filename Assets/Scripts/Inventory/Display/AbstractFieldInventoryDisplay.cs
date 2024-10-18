using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Display {
	using ItemTransformations = Dictionary<Item, List<ItemTransform>>;
	using ItemSlots = List<List<InventorySlot>>;
	
	public abstract class AbstractFieldInventoryDisplay : MonoBehaviour {
		[SerializeField] private GameObject window;
		[SerializeField] private GameObject emptySlotPrefab;
		[SerializeField] private GameObject itemSlotPrefab;
		[SerializeField] private Transform emptySlotParent;
		[SerializeField] private Transform itemSlotParent;
		[SerializeField] private RectTransform locatorPanel;
		
		public RectTransform LocatorPanel => locatorPanel;
		
		public abstract AbstractFieldInventory Inventory { get; }

		public void Show() {
			window.SetActive(true);
		}

		public void Hide() {
			window.SetActive(false);
		}

		protected void GenerateField(ItemSlots slots) {
			while ( itemSlotParent.childCount > 0 ) {
				DestroyImmediate(itemSlotParent.GetChild(0).gameObject);
			}

			for ( var y = 0; y < slots.Count; y++ )
			for ( var x = 0; x < slots.Count; x++ ) {
				var position = new Vector2Int(x, y);
				var emptySlotGo = Instantiate(emptySlotPrefab, emptySlotParent);
				var emptySlotDisplay = emptySlotGo.GetComponent<EmptySlotDisplay>();
				
				emptySlotDisplay.SetCellPosition(position);
				emptySlotDisplay.SetCellSize(Vector2Int.one);
			}
		}
		
		protected void UpdateItems(ItemTransformations itemTransformations) {
			while ( itemSlotParent.childCount > 0 ) {
				DestroyImmediate(itemSlotParent.GetChild(0).gameObject);
			}

			foreach ( var (item, transforms) in itemTransformations ) {
				foreach ( var itemTransform in transforms ) {
					var itemSlotGo = Instantiate(itemSlotPrefab, itemSlotParent);
					var itemSlotDisplay = itemSlotGo.GetComponent<ItemSlotDisplay>();

					itemSlotDisplay.SetUp(item, itemTransform, TryMove);
				
					itemSlotDisplay.SetCellPosition(itemTransform.Position);
					itemSlotDisplay.SetCellSize(itemTransform.RotatedSize(item));
					continue;

					void TryMove() {
						Inventory.TryMoveItem(item, itemTransform);
					}
				}
			}
		}
	}
}