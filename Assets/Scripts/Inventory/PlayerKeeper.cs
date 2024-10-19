using Inventory.Render;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory {
	public class PlayerKeeper : AbstractKeeper {
		private class TaskInfo {
			public bool NeedToClear { get; }
			public TaskInfo(bool needToClear) {
				NeedToClear = needToClear;
			}
		}
		
		[SerializeField] private ItemLocator itemLocator;

		[SerializeField] private GameObject inventoryWindow;

		[SerializeField] private GridInventoryRenderer playerRenderer;
		[SerializeField] private GridInventoryRenderer secondRenderer;
		
		private readonly GridInventory _playerInventory = new ();
		
		private readonly GridInventory _extraInventory = new ();
		
		private readonly GridInventory _itemInventory = new ();

		private GridInventory _inventory;
		
		private bool _isOpened;
		
		private bool _isMoving;
		private TaskInfo _currentTaskInfo;
		
		private Configuration _playerConfiguration;
		private Configuration _secondConfiguration;

		private readonly UnityEvent _endCallback = new ();
		private readonly UnityEvent<bool> _successCallback = new ();

		private void ResetCallbacks() {
			_endCallback.RemoveAllListeners();
			_successCallback.RemoveAllListeners();
		}

		private void SetState(bool opened) {
			_isOpened = opened;
			inventoryWindow.SetActive(opened);
		}
		
		private void Show() => SetState(true);
		private void Hide() => SetState(false);

		private void Awake() {
			_playerInventory.OnBeginMove.AddListener(BeginMove);
			_playerInventory.OnEndMove.AddListener(EndMove);
			playerRenderer.SetInventory(_playerInventory);
			_playerInventory.SetLocator(itemLocator);
			SetState(_isOpened);
		}

		private void SetInventory(GridInventory inventory) {
			if ( _inventory != null ) {
				_inventory.OnBeginMove.RemoveAllListeners();
				_inventory.OnEndMove.RemoveAllListeners();
			}
			_inventory = inventory;
			inventory.OnBeginMove.AddListener(BeginMove);
			inventory.OnEndMove.AddListener(EndMove);
			secondRenderer.SetInventory(inventory);
			inventory.SetLocator(itemLocator);
		}

		private void MoveItem() {
			ResetCallbacks();
			secondRenderer.Hide();
			_extraInventory.Reset();
			SetInventory(_extraInventory);
			_currentTaskInfo = new TaskInfo(true);
			StartTask();
		}
		
		public override void TryAddItem(Item item, UnityAction<bool> callback) {
			ResetCallbacks();
			secondRenderer.Show();
			_successCallback?.AddListener(callback);
			_itemInventory.Reset();
			SetInventory(_itemInventory);
			_itemInventory.TryAutoAddItem(item);
			_currentTaskInfo = new TaskInfo(true);
			StartTask();
		}

		public override void StartAddItems(GridInventory gridInventory, UnityAction callback) {
			ResetCallbacks();
			secondRenderer.Show();
			_endCallback?.AddListener(callback);
			SetInventory(gridInventory);
			_currentTaskInfo = new TaskInfo(false);
			StartTask();
		}

		private void StartTask() {
			_playerConfiguration = _playerInventory.Save();
			_secondConfiguration = _inventory.Save();
			Show();
		}

		private bool CanFinishTask() {
			return !_currentTaskInfo.NeedToClear || _inventory.IsEmpty();
		}

		private void CancelTask() {
			_playerInventory.Load(_playerConfiguration);
			_inventory.Load(_secondConfiguration);
			Hide();
			_endCallback.Invoke();
			_successCallback.Invoke(false);
		}
		
		private void FinishTask() {
			Hide();
			_endCallback.Invoke();
			_successCallback.Invoke(true);
		}

		private void BeginMove() {
			_isMoving = true;
			secondRenderer.Hide();
		}

		private void EndMove() {
			_isMoving = false;
		}

		private void Update() {
			ProcessTab();
			ProcessInput();
		}

		private void ProcessTab() {
			if ( !Input.GetButtonDown("Inventory") ) {
				return;
			}

			if ( _isOpened ) {
				if ( CanFinishTask() ) {
					FinishTask();
				}
				else {
					CancelTask();
				}
			}
			else {
				MoveItem();
			}
		}

		private void ProcessInput() {
			if (!_isOpened || _isMoving ) {
				return;
			}

			if ( Input.GetMouseButtonDown(1) ) {
				CancelTask();
			}

			if ( Input.GetButtonDown("Interact") ) {
				if ( CanFinishTask() ) {
					FinishTask();
				}
			}
		}
	}
}