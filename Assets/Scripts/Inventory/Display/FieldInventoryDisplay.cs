using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Display {
	public class FieldInventoryDisplay : AbstractFieldInventoryDisplay {

		[SerializeField] private FieldInventory defaultInventory;

		private FieldInventory _inventory;

		private static FieldInventoryDisplay _instance;

		private ItemLocator _itemLocator;

		private UnityAction _onStartMove, _onFinishMove;

		public void AddListeners(UnityAction onStartMove, UnityAction onFinishMove) {
			_onStartMove = onStartMove;
			_onFinishMove = onFinishMove;
			if ( _inventory ) {
				SetListeners();
			}
		}

		private void SetListeners() {
			_inventory.OnStartMove.AddListener(_onStartMove);
			_inventory.OnFinishMove.AddListener(_onFinishMove);
		}

		public void SetLocator(ItemLocator locator) {
			_itemLocator = locator;
			if ( _inventory ) {
				_inventory.SetLocator( locator );
			}
		}

		public static void SetInventory(FieldInventory inventory) {
			_instance.defaultInventory.OnGenerate.RemoveAllListeners();
			_instance.defaultInventory.OnUpdated.RemoveAllListeners();
			_instance.UpdateInventory(inventory);
		}

		public static void ResetInventory() {
			SetInventory(_instance.defaultInventory);
		}

		private void UpdateInventory(FieldInventory inventory) {
			Debug.Log("Updating inventory");
			_inventory = inventory;
			_inventory.OnGenerate.AddListener(GenerateField);
			_inventory.OnUpdated.AddListener(UpdateItems);
			if ( _itemLocator ) {
				_inventory.SetLocator( _itemLocator );
			}
			if ( _onStartMove != null ) {
				SetListeners();
			}
			_inventory.Init();
		}

		private void Awake() {
			_instance = this;
		}

		public override AbstractFieldInventory Inventory {
			get {
				if ( !_inventory ) {
					UpdateInventory(defaultInventory);
				}
				return _inventory;
			}
		}
	}
}