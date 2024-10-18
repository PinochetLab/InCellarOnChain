using System.Collections.Generic;

namespace Inventory {
	public class AddItemResult {
		public bool Success { get; set; }

		public AddItemResult(bool success) {
			Success = success;
		}
	}
	public delegate void AddItemCallback(AddItemResult result);
	public interface IInventory {
		void TryAddItem(Item item, AddItemCallback callback);
		bool HasItem(Item item);
		bool IsEmpty();
	}
}