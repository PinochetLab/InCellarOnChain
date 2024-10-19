using Inventory.Render;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory {
	public class ItemLocator : MonoBehaviour {
		
		[SerializeField] private GridInventoryRenderer playerRenderer;
		[SerializeField] private GridInventoryRenderer secondRenderer;
		
		[SerializeField] private GameObject locatorWindow;
		
		[SerializeField] private ItemLocatorDisplay gridSlotDisplay;
		[SerializeField] private ItemLocatorDisplay mouseSlotDisplay;

		private Item _item;
		private ItemTransform _oldTransform;
		private readonly UnityEvent<ItemTransform?> _transformChoiceCallback = new ();

		private bool _locating;

		private bool _beginIsPlayer = true;
		private bool _isPlayer = true;
		private GridInventoryRenderer _currentRenderer;
		private GridInventoryRenderer _otherRenderer;

		private void Awake() {
			locatorWindow.SetActive(false);
			SetLocatorDisplay(_isPlayer);
		}

		private void SwitchLocatorDisplay() {
			SetLocatorDisplay(!_isPlayer);
		}

		private void SetLocatorDisplay(bool isPlayer) {
			_isPlayer = isPlayer;
			if ( isPlayer ) {
				_currentRenderer = playerRenderer;
				_otherRenderer = secondRenderer;
			}
			else {
				_currentRenderer = secondRenderer;
				_otherRenderer = playerRenderer;
			}
		}

		private void Close() {
			_locating = false;
			locatorWindow.SetActive(false);
		}

		private Vector2 GetPosition(Vector2Int cell) {
			var points = new Vector3[4];
			_currentRenderer.LocatorRt.GetWorldCorners(points);
			var p = (Vector2)points[1];
			p.y *= -1;
			var d = (Vector2)cell * Constants.CellSize;
			return p + d;
		}
		
		public void TryLocateItem(
			Item item,
			ItemTransform startTransform, 
			GridInventory gridInventory,
			UnityAction<ItemTransform?> transformChoiceCallback
			) {
			
			_item = item;
			_oldTransform = startTransform;
			
			_transformChoiceCallback.RemoveAllListeners();
			_transformChoiceCallback.AddListener(transformChoiceCallback);
			
			locatorWindow.SetActive(true);
			
			_beginIsPlayer = gridInventory == playerRenderer.GridInventory;
			SetLocatorDisplay(_beginIsPlayer);
			
			playerRenderer.Show();
			secondRenderer.Show();
			
			_locating = true;

			var position = GetPosition(startTransform.Position);
			
			gridSlotDisplay.SetPosition(position);
			gridSlotDisplay.SetCellSize(startTransform.RotatedSize(item));
			gridSlotDisplay.SetUp(item, startTransform.Rotated);

			mouseSlotDisplay.SetPosition(position);
			mouseSlotDisplay.SetCellSize(startTransform.RotatedSize(item));
			mouseSlotDisplay.SetUp(item, startTransform.Rotated);
		}

		private static bool IsPointInRect(RectTransform rt, Vector2 point, out Vector2 localPoint) {
			return RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, point, null, out localPoint) 
			       && rt.rect.Contains(localPoint);
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
			
			if ( IsPointInRect(_otherRenderer.LocatorRt, mousePos, out _) ) {
				SwitchLocatorDisplay();
			}

			if ( !IsPointInRect(_currentRenderer.LocatorRt, mousePos, out Vector2 localPoint) ) {
				return;
			}
			
			localPoint.y *= -1;
			var pos = localPoint + _currentRenderer.LocatorRt.rect.size / 2 - gridSlotDisplay.ActualSize / 2;
			
			var posInt = new Vector2Int(
				Mathf.RoundToInt(pos.x / Constants.CellSize),
				Mathf.RoundToInt(pos.y / Constants.CellSize));
			
			var itemTransform = new ItemTransform(posInt, gridSlotDisplay.Rotated);

			if ( !_currentRenderer.GridInventory.CanPlaceItem(_item, itemTransform) ) {
				return;
			}

			if ( Input.GetMouseButtonDown(0) ) {
				var empty = secondRenderer.GridInventory.IsEmpty() && _isPlayer;
				if ( empty ) {
					secondRenderer.Hide();
				}
				Close();
				if ( _isPlayer == _beginIsPlayer ) {
					_transformChoiceCallback?.Invoke(itemTransform);
				}
				else {
					_transformChoiceCallback?.Invoke(ItemTransform.Moved);
					_currentRenderer.GridInventory.AddItem(_item, itemTransform);
				}
			}
			gridSlotDisplay.SetPosition(GetPosition(posInt));
			if ( !itemTransform.Equals(_oldTransform) ) {
				_oldTransform = itemTransform;
			}
		}
	}
}