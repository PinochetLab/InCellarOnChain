using System;
using System.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Async;
using Inventory.Display;
using UnityEngine;

namespace Inventory {
	public class PlayerInventory : MonoBehaviour, IInventory {

		[SerializeField] private int maxCount = 30;
		
		[SerializeField] private int width = 4;
		
		[SerializeField] private PlayerInventoryDisplay display;
		
		[SerializeField] private ItemLocator itemLocator;
		
		private Dictionary<Item, List<ItemTransform>> _itemTransforms = new ();
		
		private List<List<InventorySlot>> _slots = new ();

		[SerializeField] private Item testItem;

		private void Awake() {
			var count = maxCount;
			while ( count > 0 ) {
				List<InventorySlot> slots;
				if ( count >= width ) {
					slots = Enumerable.Range(0, 4).Select(_ => InventorySlot.CreateEmpty()).ToList();
				}
				else {
					slots = Enumerable.Range(0, 4).Select(i => 
						i < count ? InventorySlot.CreateEmpty() : InventorySlot.CreateDenied()).ToList();
				}
				_slots.Add(slots);
				count -= width;
			}

			AddItem(testItem, Vector2Int.zero, false);
			AddItem(testItem, new Vector2Int(2, 0), true);
			
			display.SetUp(this, _slots);
			display.UpdateItems(_itemTransforms);
		}

		private void AddItem(Item item, Vector2Int position, bool b) {
			var itemTransform = new ItemTransform(position, b);
			if (!_itemTransforms.ContainsKey(item)) _itemTransforms.Add(item, new List<ItemTransform>());
			_itemTransforms[item].Add(itemTransform);
			
			itemTransform.GetSlots(item).ToList().ForEach(v => _slots[v.y][v.x].SetItem(item));
		}

		private bool IsPositionValid(Vector2Int position) {
			return position.x >= 0 && 
			       position.x < width && 
			       position.y >= 0 && 
			       position.y < _slots.Count;
		}

		private bool IsSlotEmpty(Vector2Int position) {
			return _slots[position.y][position.x].IsEmpty();
		}

		// ReSharper disable Unity.PerformanceAnalysis
		public void TryMoveItem(Item item, ItemTransform itemTransform) {
			if ( !_itemTransforms.ContainsKey(item) ) return;
			_itemTransforms[item].Remove(itemTransform);
			display.UpdateItems(_itemTransforms);
			
			itemTransform.GetSlots(item).ToList().ForEach(v => _slots[v.y][v.x].SetEmpty());

			itemLocator.TryLocateItem(item, itemTransform, this, TryMoveCallback);
			return;

			void TryMoveCallback(ItemTransform? optionalTransform) {
				var it = optionalTransform ?? itemTransform;
				_itemTransforms[item].Add(it);
				it.GetSlots(item).ToList().ForEach(v => _slots[v.y][v.x].SetItem(item));
				display.UpdateItems(_itemTransforms);
			}
		}

		public bool CanLocateItem(Item item, ItemTransform itemTransform) {
			return itemTransform.GetSlots(item).All(v => IsPositionValid(v) && IsSlotEmpty(v));
		}

		public IEnumerator<bool> TryAddItem(Item item) {
			throw new System.NotImplementedException();
		}

		public bool HasItem(Item item) {
			return _itemTransforms.ContainsKey(item) && _itemTransforms[item].Count > 0;
		}

		public void RemoveItem(Item item) {
			throw new System.NotImplementedException();
		}
	}
}