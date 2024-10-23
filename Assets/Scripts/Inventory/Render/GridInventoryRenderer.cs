using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inventory.Render {
	public class GridInventoryRenderer : MonoBehaviour {
		[SerializeField] private GameObject window;
		[SerializeField] private Transform slotParent;
		[SerializeField] private GameObject slotPrefab;

		[SerializeField] private RectTransform locatorRt;

		public RectTransform LocatorRt => locatorRt;
		
		public GridInventory GridInventory { get; private set; }

		private Transformations _oldTransformations = new ();
		
		private List<List<SlotRenderer>> _slots = new ();

		private Item _selectionItem;
		private ItemTransform _selectionIt;

		public void Show() {
			SetActive(true);
		}
		
		public void Hide() {
			SetActive(false);
		}

		private void SetActive(bool active) {
			window.SetActive(active);
		}

		public void SetInventory(GridInventory gridInventory) {
			GridInventory = gridInventory;
			
			GridInventory.OnBuild.RemoveAllListeners();
			GridInventory.OnUpdate.RemoveAllListeners();
			GridInventory.OnAdd.RemoveAllListeners();
			GridInventory.OnRemove.RemoveAllListeners();
			
			GridInventory.OnBuild.AddListener(Build);
			GridInventory.OnUpdate.AddListener(UpdateItems);
			GridInventory.OnAdd.AddListener(AddItem);
			GridInventory.OnRemove.AddListener(RemoveItem);
			
			GridInventory.Update();
		}
		
		private void Build(int width, int height) {
			while ( slotParent.childCount > 0 ) {
				DestroyImmediate(slotParent.GetChild(0).gameObject);
			}

			_slots = new List<List<SlotRenderer>>();

			for ( var y = 0; y < height; y++ ) {
				var row = new List<SlotRenderer>();
				for ( var x = 0; x < width; x++ ) {
					var slot = Instantiate(slotPrefab, slotParent).GetComponent<SlotRenderer>();
					slot.SetEmpty();
					row.Add(slot);
				}
				_slots.Add(row);
			}
		}

		private void AddItem(Item item, ItemTransform it) {
			var angle = it.GetAngle();
			_oldTransformations.Add(item, it);
			foreach ( var (cell, sprite, edgeInfo) in it.GetSprites(item) ) {
				_slots[cell.y][cell.x].SetItem(sprite, edgeInfo, angle, TryMove);
			}
			return;
			void TryMove() {
				GridInventory.TryMoveItem(item, it);
			}
		}
		
		private void RemoveItem(Item item, ItemTransform it) {
			_oldTransformations.Remove(item, it);
			foreach ( var v in it.GetCells(item) ) {
				_slots[v.y][v.x].SetEmpty();
			}
		}
		
		private void UpdateItems(Transformations transformations) {
			foreach ( var (item, it) in _oldTransformations ) {
				var cells = it.GetCells(item);
				foreach ( var v in cells ) {
					_slots[v.y][v.x].SetEmpty();
				}
			}

			_oldTransformations = new Transformations(transformations);
			
			foreach ( var (item, it) in transformations ) {
				var angle = it.GetAngle();
				foreach ( var (cell, sprite, edgeInfo) in it.GetSprites(item) ) {
					_slots[cell.y][cell.x].SetItem(sprite, edgeInfo, angle, TryMove);
				}
				continue;

				void TryMove() {
					GridInventory.TryMoveItem(item, it);
				}
			}
		}
	}
}