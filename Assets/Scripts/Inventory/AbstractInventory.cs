using System.Collections.Generic;
using UnityEngine;

namespace Inventory {
	public abstract class AbstractInventory : MonoBehaviour, IInventory {
		public abstract void TryAddItem(Item item, AddItemCallback callback);

		public abstract bool HasItem(Item item);

		public abstract bool IsEmpty();
	}
}