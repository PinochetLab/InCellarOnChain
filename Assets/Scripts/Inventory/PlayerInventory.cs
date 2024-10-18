using System;
using Inventory.Display;
using UnityEngine;

namespace Inventory {
	public enum InventoryMode { Moving, Adding }
	public class PlayerInventory : AbstractInventory {
		private static PlayerInventory _instance;
		
		[SerializeField] private PlayerFieldInventory playerFieldInventory;
		
		[SerializeField] private FieldInventoryDisplay fieldDisplay;
		
		[SerializeField] private GameObject inventoryWindow;

		private bool _jobStarted;
		private bool _isOpened;
		private bool _isMoving;
		private InventoryMode _inventoryMode;

		private Configuration _configuration = null;

		private AddItemCallback _callback;

		private void Close() {
			SetState(false);
		}

		private void Open() {
			SetState(true);
		}

		private void SetState(bool opened) {
			_isOpened = opened;
			inventoryWindow.SetActive(opened);
		}

		private void Awake() {
			playerFieldInventory.OnStartMove.AddListener(OnStartMove);
			playerFieldInventory.OnFinishMove.AddListener(OnFinishMove);
			fieldDisplay.AddListeners(OnStartMove, OnFinishMove);
			SetState(_isOpened);
		}

		public override void TryAddItem(Item item, AddItemCallback callback) {
			_inventoryMode = InventoryMode.Adding;
			_callback = callback;
			fieldDisplay.Show();
			var result = fieldDisplay.Inventory.AddItem(item);
			if ( !result ) {
				_inventoryMode = InventoryMode.Moving;
				_callback?.Invoke(new AddItemResult(false));
			}
			Open();
		}

		private void OnStartMove() {
			_jobStarted = true;
			_isMoving = true;
			if ( _inventoryMode == InventoryMode.Moving ) {
				if ( _configuration == null ) {
					fieldDisplay.Hide();
				}
			}
			_configuration ??= playerFieldInventory.SaveConfiguration();
		}
		
		private void OnFinishMove() {
			_isMoving = false;
		}

		private void ProcessTab() {
			if ( Input.GetButtonDown("Inventory") ) {
				_isOpened = !_isOpened;
				SetState(_isOpened);
				if ( !_isOpened && _jobStarted ) {
					if ( _isMoving ) {
						ProcessCancel();
					}
					else {
						if ( fieldDisplay.Inventory.IsEmpty() ) {
							ProcessSubmit();
						}
						else {
							ProcessCancel();
						}
					}
				}
			}
		}

		private void Update() {
			ProcessTab();
			ProcessInput();
		}

		private void ProcessCancel() {
			switch (_inventoryMode)
			{
				case InventoryMode.Adding:
					Debug.Log("Adding item stop");
					_callback?.Invoke(new AddItemResult(false));
					_inventoryMode = InventoryMode.Moving;
					break;
				case InventoryMode.Moving:
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
			playerFieldInventory.LoadConfiguration(_configuration);
			fieldDisplay.Inventory.Clear();
			_configuration = null;
			_jobStarted = false;
			Close();
		}
		
		private void ProcessSubmit() {
			switch (_inventoryMode)
			{
				case InventoryMode.Adding:
					if ( fieldDisplay.Inventory.IsEmpty() ) {
						_callback?.Invoke(new AddItemResult(true));
						_inventoryMode = InventoryMode.Moving;
						_jobStarted = false;
						_configuration = null;
						Close();
					}
					break;
				case InventoryMode.Moving:
					if ( fieldDisplay.Inventory.IsEmpty() ) {
						_jobStarted = true;
						_configuration = null;
						Close();
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private void ProcessInput() {
			if ( _isMoving ) {
				return;
			}

			if ( Input.GetMouseButtonDown(1) ) {
				ProcessCancel();
			}
			else if ( Input.GetButtonDown("Interact") ) {
				ProcessSubmit();
			}
		}

		public override bool HasItem(Item item) {
			return playerFieldInventory.HasItem(item);
		}

		public override bool IsEmpty() {
			return playerFieldInventory.IsEmpty();
		}
	}
}