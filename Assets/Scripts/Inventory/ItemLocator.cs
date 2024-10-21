using Inventory.Render;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory {
	public class ItemLocator : MonoBehaviour {
		
		[SerializeField] private GridInventoryRenderer playerRenderer;
		[SerializeField] private GridInventoryRenderer secondRenderer;
		
		[SerializeField] private GameObject locatorWindow;
		
		private readonly UnityEvent<ItemTransform?> _transformChoiceCallback = new ();

		private bool _locating;

		private bool _beginIsPlayer = true;
		private bool _isPlayer = true;
		private GridInventoryRenderer _currentRenderer;
		private GridInventoryRenderer _otherRenderer;

		private Item _selectionItem;
		private ItemTransform _selectionIt;
		private GridInventoryRenderer _selectionRenderer;

		private ItemTransform _currentIt;
		private GridInventoryRenderer _currentRend;
		
		private Vector2 _offset;

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
			
			_selectionItem = item;
			_selectionIt = startTransform;
			_currentIt = startTransform;
			
			_transformChoiceCallback.RemoveAllListeners();
			_transformChoiceCallback.AddListener(transformChoiceCallback);
			
			locatorWindow.SetActive(true);
			
			_beginIsPlayer = gridInventory == playerRenderer.GridInventory;
			SetLocatorDisplay(_beginIsPlayer);

			_currentRend = _currentRenderer;
			_selectionRenderer = _otherRenderer;
			
			playerRenderer.Show();
			secondRenderer.Show();
			
			_locating = true;

			CountOffset();
			Debug.Log(_offset);
		}

		private static bool IsPointInRect(RectTransform rt, Vector2 point, out Vector2 localPoint) {
			return RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, point, null, out localPoint) 
			       && rt.rect.Contains(localPoint);
		}

		private void CountOffset() {
			var mousePos = Input.mousePosition;
			if ( !IsPointInRect(_currentRenderer.LocatorRt, mousePos, out Vector2 localPoint) ) {
				return;
			}
			
			localPoint.y *= -1;
			var pos = localPoint + _currentRenderer.LocatorRt.rect.size / 2;
			var realPos = (Vector2)_currentIt.Pos * Constants.CellSize;
			_offset = realPos - pos;
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
				_currentIt.Rotate();
			}
			var mousePos = Input.mousePosition;
			
			if ( IsPointInRect(_otherRenderer.LocatorRt, mousePos, out _) ) {
				_currentRenderer.Deselect();
				SwitchLocatorDisplay();
			}

			if ( !IsPointInRect(_currentRenderer.LocatorRt, mousePos, out Vector2 localPoint) ) {
				return;
			}
			
			localPoint.y *= -1;
			var pos = localPoint + _currentRenderer.LocatorRt.rect.size / 2;
			pos += _offset;
			
			var posInt = new Vector2Int(
				Mathf.RoundToInt(pos.x / Constants.CellSize),
				Mathf.RoundToInt(pos.y / Constants.CellSize));
			
			//var itemTransform = new ItemTransform(posInt, );
			_currentIt.SetPosition(posInt);

			_currentIt = _currentRenderer.GridInventory.Correct(_selectionItem, _currentIt);
			
			var canPlace = _currentRenderer.GridInventory.CanPlaceItem(_selectionItem, _currentIt);
			
			_currentRenderer.Select(_selectionItem, _currentIt, canPlace);

			if ( !canPlace ) {
				return;
			}

			if ( Input.GetKeyDown(KeyCode.Space) ) {
				var empty = secondRenderer.GridInventory.IsEmpty() && _isPlayer;
				if ( empty ) {
					secondRenderer.Hide();
				}
				Close();
				if ( _isPlayer == _beginIsPlayer ) {
					_transformChoiceCallback?.Invoke(_selectionIt);
				}
				else {
					_transformChoiceCallback?.Invoke(ItemTransform.Moved);
					_currentRenderer.GridInventory.AddItem(_selectionItem, _selectionIt);
				}
			}
			//gridSlotDisplay.SetPosition(GetPosition(posInt));
			if ( !_currentIt.Equals(_selectionIt) || _currentRenderer != _selectionRenderer ) {
				_selectionIt = _currentIt;
				//_selectionRenderer.Deselect(_selectionItem);
				_selectionRenderer = _currentRenderer;
				//_currentRenderer.Select(_selectionItem, _selectionIt);
			}
			/*if ( !itemTransform.Equals(_oldTransform) ) {
				_oldTransform = itemTransform;
			}*/
		}
	}
}