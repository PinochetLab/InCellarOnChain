using System;
using System.Collections.Generic;
using Inventory.Combining;
using Inventory.Render;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory {
	public class ItemLocator : MonoBehaviour {
		
		[SerializeField] private GridInventoryRenderer playerRenderer;
		[SerializeField] private GridInventoryRenderer secondRenderer;
		
		[SerializeField] private GameObject locatorWindow;
		[SerializeField] private ItemRenderer itemRenderer;

		[SerializeField] private Color greenColor;
		[SerializeField] private Color redColor;
		
		[SerializeField] private CombinationManager combinationManager;
		[SerializeField] private Transform hintParent;
		[SerializeField] private GameObject hintItemRendererPrefab;
		[SerializeField] private Color cyanColor;
		
		private readonly UnityEvent<ItemTransform?> _transformChoiceCallback = new ();

		private bool _locating;

		private bool _beginIsPlayer = true;
		private bool _isPlayer = true;

		private Item _selectionItem;
		private ItemTransform _selectionIt;
		private bool _selectionIsPlayer;

		private ItemTransform _currentIt;
		
		private Vector2 _offset;
		
		private readonly Dictionary<ItemTransform, CombinationAnswer> _playerCombinations = new ();
		private readonly Dictionary<ItemTransform, CombinationAnswer> _secondCombinations = new ();

		private void Awake() {
			locatorWindow.SetActive(false);
			SetLocatorDisplay(_beginIsPlayer);
		}

		private void SwitchIsPlayer() {
			SetLocatorDisplay(!_isPlayer);
		}
		
		private RectTransform GetRect(bool isPlayer) {
			return GetRenderer(isPlayer).LocatorRt;
		}
		
		private GridInventory GetInventory(bool isPlayer) {
			return GetRenderer(isPlayer).GridInventory;
		}
		
		private GridInventoryRenderer GetRenderer(bool isPlayer) {
			return isPlayer ? playerRenderer : secondRenderer;
		}

		private Dictionary<ItemTransform, CombinationAnswer> GetCombinations(bool isPlayer) {
			return isPlayer ? _playerCombinations : _secondCombinations;
		}

		private void SetLocatorDisplay(bool isPlayer) {
			_isPlayer = isPlayer;
		}

		private void ProcessCombinations(Item item) {
			foreach ( var isPlayer in new List<bool> { true, false } ) {
				GetCombinations(isPlayer).Clear();
				var transformations = GetInventory(isPlayer).Save().Transformations;
				foreach ( var answer in combinationManager.GetAnswers(item, transformations) ) {
					var targetIt = answer.TargetIt;
					var ir = Instantiate(hintItemRendererPrefab, hintParent).GetComponent<ItemRenderer>();
					ir.SetItem(item);
					ir.SetColor(cyanColor);
					ir.SetTransform(GetScreenPosition(targetIt.Position, isPlayer), targetIt.RotateIndex);
					GetCombinations(isPlayer).Add(targetIt, answer);
				}
			}
		}

		private void Close() {
			while ( hintParent.childCount > 0 ) {
				DestroyImmediate(hintParent.GetChild(0).gameObject);
			}
			_locating = false;
			locatorWindow.SetActive(false);
		}

		private Vector2 GetScreenPosition(Vector2Int cell, bool isPlayer) {
			var points = new Vector3[4];
			GetRenderer(isPlayer).LocatorRt.GetWorldCorners(points);
			var p = (Vector2)points[1];
			cell.y *= -1;
			var d = (Vector2)cell * Constants.CellSize;
			return p + d;
		}
		
		public void TryLocateItem(
			Item item,
			ItemTransform startTransform, 
			GridInventory gridInventory,
			UnityAction<ItemTransform?> transformChoiceCallback
			) {
			ProcessCombinations(item);
			itemRenderer.SetItem(item);
			
			_selectionItem = item;
			_selectionIt = startTransform;
			_currentIt = startTransform;
			
			_transformChoiceCallback.RemoveAllListeners();
			_transformChoiceCallback.AddListener(transformChoiceCallback);
			
			locatorWindow.SetActive(true);
			
			_beginIsPlayer = gridInventory == playerRenderer.GridInventory;
			SetLocatorDisplay(_beginIsPlayer);

			_selectionIsPlayer = false;
			
			playerRenderer.Show();
			secondRenderer.Show();
			
			_locating = true;

			CountOffset();
		}

		private static bool IsPointInRect(RectTransform rt, Vector2 point, out Vector2 localPoint) {
			return RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, point, null, out localPoint) 
			       && rt.rect.Contains(localPoint);
		}

		private void CountOffset() {
			var mousePos = Input.mousePosition;
			if ( !IsPointInRect(GetRect(_isPlayer), mousePos, out Vector2 localPoint) ) {
				return;
			}
			
			localPoint.y *= -1;
			var pos = localPoint + GetRect(_isPlayer).rect.size / 2;
			var realPos = (Vector2)_currentIt.Position * Constants.CellSize;
			_offset = realPos - pos;
		}

		private bool TryCancel() {
			if ( Input.GetMouseButtonDown(1) ) {
				Close();
				_transformChoiceCallback?.Invoke(null);
				return true;
			}
			return false;
		}

		private void RotateLogic() {
			if ( Input.GetButtonDown("Interact") ) {
				_currentIt.Rotate();
			}
		}

		private bool TrySwitchRenderer(Vector2 mousePosition) {
			if ( IsPointInRect(GetRect(!_isPlayer), mousePosition, out _) ) {
				SwitchIsPlayer();
				return true;
			}
			return false;
		}

		private bool IsPointInCurrentRect(Vector2 mousePosition, out Vector2 localPoint) {
			return IsPointInRect(GetRect(_isPlayer), mousePosition, out localPoint);
		}

		private void Update() {
			if ( !_locating ) {
				return;
			}

			if ( TryCancel() ) {
				return;
			}
			
			RotateLogic();
			
			var mousePosition = Input.mousePosition;
			
			if ( TrySwitchRenderer(mousePosition) ) {
				return;
			}

			if ( !IsPointInCurrentRect(mousePosition, out Vector2 localPoint) ) {
				return;
			}
			
			localPoint.y *= -1;
			var pos = localPoint + GetRect(_isPlayer).rect.size / 2;
			pos += _offset;
			
			var posInt = new Vector2Int(
				Mathf.RoundToInt(pos.x / Constants.CellSize),
				Mathf.RoundToInt(pos.y / Constants.CellSize));
			
			_currentIt.SetPosition(posInt);

			_currentIt = GridInventory.Correct(_selectionItem, _currentIt);
			
			var canPlace = GetInventory(_isPlayer).CanPlaceItem(_selectionItem, _currentIt);
			
			var position = GetScreenPosition(_currentIt.Position, _isPlayer);
			var rotateIndex = _currentIt.RotateIndex;
			
			itemRenderer.SetTransform(position, rotateIndex);
			itemRenderer.SetColor(canPlace ? greenColor : redColor);

			if ( Input.GetKeyDown(KeyCode.Space) ) {
				if ( GetCombinations(_isPlayer).TryGetValue(_currentIt, out var answer) ) {
					var inventory = GetInventory(_isPlayer);
					if ( inventory.CanPlaceItem(_selectionItem, _currentIt, answer.OtherItem, answer.OtherIt) ) {
						var empty = secondRenderer.GridInventory.IsEmpty() && _isPlayer;
						if ( empty ) {
							secondRenderer.Hide();
						}
						Close();
						inventory.RemoveItem(answer.OtherItem, answer.OtherIt);
						inventory.AddItem(answer.ResultItem, answer.ResultIt);
					}
				}
				
				
			}

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
					GetInventory(_isPlayer).AddItem(_selectionItem, _selectionIt);
				}
			}
			
			if ( !_currentIt.Equals(_selectionIt) || _selectionIsPlayer != _isPlayer ) {
				_selectionIt = _currentIt;
				_selectionIsPlayer = _isPlayer;
			}
		}
	}
}