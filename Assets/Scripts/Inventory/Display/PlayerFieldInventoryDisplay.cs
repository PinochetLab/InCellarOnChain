using UnityEngine;

namespace Inventory.Display {
	public class PlayerFieldInventoryDisplay : AbstractFieldInventoryDisplay {
		[SerializeField] private PlayerFieldInventory inventory;
		public override AbstractFieldInventory Inventory => inventory;

		private void Awake() {
			inventory.OnUpdated.AddListener(UpdateItems);
			inventory.OnGenerate.AddListener(GenerateField);
			inventory.Init();
		}
	}
}