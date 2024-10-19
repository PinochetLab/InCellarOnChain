using UnityEngine;
using UnityEngine.Events;

namespace Inventory {
	public abstract class AbstractKeeper : MonoBehaviour {
		public abstract void TryAddItem(Item item, UnityAction<bool> callback);
		public abstract void StartAddItems(GridInventory gridInventory, UnityAction callback);
	}
}