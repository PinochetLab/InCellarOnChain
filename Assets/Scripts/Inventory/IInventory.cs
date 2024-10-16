using System.Collections.Generic;

namespace Inventory {
	public interface IInventory {
		IEnumerator<bool> TryAddItem(Item item);
		bool HasItem(Item item);
		void RemoveItem(Item item);
	}
}