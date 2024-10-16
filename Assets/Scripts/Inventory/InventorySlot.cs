using UnityEngine;

namespace Inventory {
	public enum SlotStatus {Empty, Denied, Item}
	
	public class InventorySlot {
		private SlotStatus _status;
		private Item _item;

		private InventorySlot(SlotStatus status, Item item) {
			_status = status;
			_item = item;
		}

		public void AddItem(Item item) {
			_status = SlotStatus.Item;
			_item = item;
		}
		
		public static InventorySlot CreateEmpty() => new (SlotStatus.Empty, null);
		
		public static InventorySlot CreateDenied() {
			return new InventorySlot(SlotStatus.Denied, null);
		}

		public static InventorySlot CreateItem(Item item) {
			return new InventorySlot(SlotStatus.Item, item);
		}

		public void SetItem(Item item) {
			_status = SlotStatus.Item;
			_item = item;
		}

		public void SetEmpty() {
			_status = SlotStatus.Empty;
		}

		public bool IsEmpty() {
			return _status == SlotStatus.Empty;
		}
		
		public bool IsDenied() {
			return _status == SlotStatus.Denied;
		}
	}
}