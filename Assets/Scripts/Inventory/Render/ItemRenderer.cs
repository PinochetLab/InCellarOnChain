using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Render {
	public class ItemRenderer : MonoBehaviour {
		[SerializeField] private Transform cellParent;
		[SerializeField] private RectTransform rectTransform;
		[SerializeField] private GameObject itemCellRendererPrefab;

		private Item _currentItem;
		private readonly List<ItemCellRenderer> _cellRenderers = new ();
		
		public void SetItem(Item item) {
			_cellRenderers.Clear();
			_currentItem = item;
			ClearChildren();
			var it = new ItemTransform(Vector2Int.zero, 0);
			foreach ( var (cell, sprite, edgeInfo) in it.GetSprites(item) ) {
				var itemCellRenderer = Instantiate(itemCellRendererPrefab, cellParent).GetComponent<ItemCellRenderer>();
				itemCellRenderer.SetUp(cell, sprite, edgeInfo);
				_cellRenderers.Add(itemCellRenderer);
			}
		}

		public void SetColor(Color color) {
			_cellRenderers.ForEach(x => x.SetColor(color));
		}

		public void SetTransform(Vector2 position, int rotateIndex) {
			var angle = rotateIndex * 90f;
			transform.localEulerAngles = Vector3.forward * angle;
			var size = (Vector2)_currentItem.BoundingSize * Constants.CellSize;
			rectTransform.anchoredPosition = rotateIndex switch {
				0 => position,
				1 => position - size.x * Vector2.up,
				2 => position + new Vector2(size.x, -size.y),
				3 => position + size.y * Vector2.right,
				_ => throw new ArgumentOutOfRangeException(nameof(rotateIndex), rotateIndex, null)
			};
		}

		private void ClearChildren() {
			while ( cellParent.childCount > 0 ) {
				DestroyImmediate(cellParent.GetChild(0).gameObject);
			}
		}
	}
}