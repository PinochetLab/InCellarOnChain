using UnityEngine;

namespace Inventory {
	public class FieldInventory : AbstractFieldInventory {

		[SerializeField] private bool main;
		
		private bool _initialized;
		
		public static FieldInventory Main { get; private set; }

		private void Awake() {
			if ( main ) {
				Main = this;
			}
		}

		public override void Init() {
			SetUp();
		}

		public override void TryAddItem(Item item, AddItemCallback callback) {
			var success = AddItem(item);
			callback(new AddItemResult(success));
		}
	}
}