using System;
using System.Threading;
using System.Threading.Tasks;
using Async;
using Inventory.Display;
using UnityEngine;

namespace Inventory {
	public delegate void LocateDelegate(ItemTransform? optionalTransform);
	public class ItemLocator : MonoBehaviour {
		[SerializeField] private GameObject graphics;
		[SerializeField] private RectTransform fieldRectTransform;
		
		[SerializeField] private ItemLocatorDisplay gridSlotDisplay;
		[SerializeField] private ItemLocatorDisplay mouseSlotDisplay;

		private Item _item;
		private PlayerInventory _inventory;
		private ItemTransform _oldTransform;
		private LocateDelegate _locateDelegate;

		private bool _locating;
		
		public void TryLocateItem(
			Item item, 
			ItemTransform startTransform, 
			PlayerInventory inventory,
			LocateDelegate locateDelegate
			) {
			_item = item;
			_inventory = inventory;
			_oldTransform = startTransform;
			_locateDelegate = locateDelegate;
			
			graphics.SetActive(true);
			_locating = true;
			
			gridSlotDisplay.SetCellPosition(startTransform.Position);
			gridSlotDisplay.SetCellSize(startTransform.RotatedSize(item));
			gridSlotDisplay.SetUp(item, startTransform.Rotated);
			
			mouseSlotDisplay.SetCellPosition(startTransform.Position);
			mouseSlotDisplay.SetCellSize(startTransform.RotatedSize(item));
			mouseSlotDisplay.SetUp(item, startTransform.Rotated);
		}

		private void Update() {
			if ( !_locating ) return;
			if ( Input.GetMouseButtonDown(1) ) {
				_locating = false;
				graphics.SetActive(false);
				_locateDelegate.Invoke(null);
			}
			if ( Input.GetButtonDown("Action") ) {
				gridSlotDisplay.Rotate();
				mouseSlotDisplay.Rotate();
			}
			var position = Input.mousePosition;
			if ( !RectTransformUtility.ScreenPointToLocalPointInRectangle(fieldRectTransform, position, null,
				    out var localPoint) ) {
				return;
			}

			var halfSize = fieldRectTransform.rect.size / 2;
			localPoint += new Vector2(halfSize.x, -halfSize.y);
			var halfItemSize = gridSlotDisplay.ActualSize / 2;
			var pos = localPoint + new Vector2(-halfItemSize.x, halfItemSize.y);
			pos.y *= -1;
			var posInt = new Vector2Int(
				Mathf.RoundToInt(pos.x / Constants.CellSize),
				Mathf.RoundToInt(pos.y / Constants.CellSize));

			var itemTransform = new ItemTransform(posInt, gridSlotDisplay.Rotated);
			if ( _inventory.CanLocateItem(_item, itemTransform) ) {
				if ( Input.GetMouseButtonDown(0) ) {
					_locating = false;
					graphics.SetActive(false);
					_locateDelegate.Invoke(itemTransform);
				}
				gridSlotDisplay.SetCellPosition(posInt);
				if ( !itemTransform.Equals(_oldTransform) ) {
					_oldTransform = itemTransform;
				}
			}
			
			mouseSlotDisplay.SetPosition(pos);
		}
	}
}