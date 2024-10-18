using Inventory.Display;
using UnityEngine;

namespace Inventory {
	public delegate void TransformChoiceCallback(ItemTransform? optionalTransform);
	public class ItemLocator : MonoBehaviour {
		
		[SerializeField] private PlayerFieldInventoryDisplay playerFieldInventoryDisplay;
		[SerializeField] private FieldInventoryDisplay fieldInventoryDisplay;
		
		[SerializeField] private GameObject locatorWindow;
		
		[SerializeField] private ItemLocatorDisplay gridSlotDisplay;
		[SerializeField] private ItemLocatorDisplay mouseSlotDisplay;

		private Item _item;
		private ItemTransform _oldTransform;
		private TransformChoiceCallback _transformChoiceCallback;

		private bool _locating;

		private bool _beginIsPlayer = true;
		private bool _isPlayer = true;
		private AbstractFieldInventoryDisplay _currentDisplay;
		private AbstractFieldInventoryDisplay _otherDisplay;

		private void Awake() {
			locatorWindow.SetActive(false);
			SetLocatorDisplay(_isPlayer);
			playerFieldInventoryDisplay.Inventory.SetLocator(this);
			fieldInventoryDisplay.SetLocator(this);
		}

		private void SwitchLocatorDisplay() {
			SetLocatorDisplay(!_isPlayer);
		}

		private void SetLocatorDisplay(bool isPlayer) {
			_isPlayer = isPlayer;
			if ( isPlayer ) {
				_currentDisplay = playerFieldInventoryDisplay;
				_otherDisplay = fieldInventoryDisplay;
			}
			else {
				_currentDisplay = fieldInventoryDisplay;
				_otherDisplay = playerFieldInventoryDisplay;
			}
		}

		public void Close() {
			_locating = false;
			locatorWindow.SetActive(false);
		}

		private Vector2 GetPosition(Vector2Int cell) {
			var points = new Vector3[4];
			_currentDisplay.LocatorPanel.GetWorldCorners(points);
			var p = (Vector2)points[1];
			p.y *= -1;
			var d = (Vector2)cell * Constants.CellSize;
			return p + d;
		}
		
		public void TryLocateItem(
			Item item,
			ItemTransform startTransform, 
			AbstractFieldInventory inventory,
			TransformChoiceCallback transformChoiceCallback
			) {
			_item = item;
			_oldTransform = startTransform;
			_transformChoiceCallback = transformChoiceCallback;
			locatorWindow.SetActive(true);
			
			_beginIsPlayer = inventory == playerFieldInventoryDisplay.Inventory;
			SetLocatorDisplay(_beginIsPlayer);
			
			playerFieldInventoryDisplay.Show();
			fieldInventoryDisplay.Show();
			
			_locating = true;

			var position = GetPosition(startTransform.Position);
			
			gridSlotDisplay.SetPosition(position);
			gridSlotDisplay.SetCellSize(startTransform.RotatedSize(item));
			gridSlotDisplay.SetUp(item, startTransform.Rotated);

			mouseSlotDisplay.SetPosition(position);;
			mouseSlotDisplay.SetCellSize(startTransform.RotatedSize(item));
			mouseSlotDisplay.SetUp(item, startTransform.Rotated);
		}

		private static bool IsPointInRect(RectTransform rt, Vector2 point, out Vector2 localPoint) {
			if ( RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, point, null, out localPoint) ) {
				return rt.rect.Contains(localPoint);
			}
			return false;
		}

		private void Update() {
			
			if ( !_locating ) {
				return;
			}
			if ( Input.GetMouseButtonDown(1) ) {
				Close();
				_transformChoiceCallback?.Invoke(null);
				return;
			}
			if ( Input.GetButtonDown("Interact") ) {
				gridSlotDisplay.Rotate();
				mouseSlotDisplay.Rotate();
			}
			var mousePos = Input.mousePosition;

			var half = gridSlotDisplay.ActualSize / 2;
			
			mouseSlotDisplay.SetPosition(new Vector2(mousePos.x, -mousePos.y) - half);
			
			if ( IsPointInRect(_otherDisplay.LocatorPanel, mousePos, out Vector2 lp) ) {
				SwitchLocatorDisplay();
			}

			if ( !IsPointInRect(_currentDisplay.LocatorPanel, mousePos, out Vector2 localPoint) ) {
				return;
			}
			
			localPoint.y *= -1;
			var pos = localPoint + _currentDisplay.LocatorPanel.rect.size / 2 - gridSlotDisplay.ActualSize / 2;
			
			var posInt = new Vector2Int(
				Mathf.RoundToInt(pos.x / Constants.CellSize),
				Mathf.RoundToInt(pos.y / Constants.CellSize));
			
			var itemTransform = new ItemTransform(posInt, gridSlotDisplay.Rotated);

			if ( !_currentDisplay.Inventory.CanLocateItem(_item, itemTransform) ) {
				return;
			}

			if ( Input.GetMouseButtonDown(0) ) {
				var empty = fieldInventoryDisplay.Inventory.IsEmpty() && _isPlayer;
				if ( empty ) {
					fieldInventoryDisplay.Hide();
				}
				Close();
				if ( _isPlayer == _beginIsPlayer ) {
					_transformChoiceCallback?.Invoke(itemTransform);
				}
				else {
					_transformChoiceCallback?.Invoke(ItemTransform.Moved);
					_currentDisplay.Inventory.AddItem(_item, itemTransform);
				}
			}
			gridSlotDisplay.SetPosition(GetPosition(posInt));
			if ( !itemTransform.Equals(_oldTransform) ) {
				_oldTransform = itemTransform;
			}
		}
	}
}