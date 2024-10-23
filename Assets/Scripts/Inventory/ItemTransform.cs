using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Render;
using UnityEngine;

namespace Inventory {
	[Serializable]
	public struct ItemTransform : IEquatable<ItemTransform> {
		[SerializeField] private Vector2Int position;

		[SerializeField] private int rotateIndex;

		public Vector2Int Position => position;
		
		public int RotateIndex => rotateIndex;
		
		public static ItemTransform Moved => new (-Vector2Int.one, 0);

		public Vector2Int RotatedSize(Item item) {
			var size = item.BoundingSize;
			if ( rotateIndex % 2 == 1 ) {
				size = new Vector2Int(size.y, size.x);
			}
			return size;
		}
		
		public bool IsMoved => position == -Vector2Int.one && rotateIndex == 0;

		public ItemTransform(Vector2Int position, int rotateIndex) {
			this.position = position;
			this.rotateIndex = rotateIndex;
		}
		
		public ItemTransform(ItemTransform other) {
			position = other.position;
			rotateIndex = other.rotateIndex;
		}

		public Vector2Int Transform(Vector2Int cell, Item item) {
			var boundingSize = item.BoundingSize;
			var x = cell.x;
			var y = cell.y;
			return position + rotateIndex switch {
				0 => new Vector2Int(x, y),
				1 => new Vector2Int(y, boundingSize.x - 1 - x),
				2 => new Vector2Int(boundingSize.x - 1 - x, boundingSize.y - 1 - y),
				3 => new Vector2Int(boundingSize.y - 1 - y, x),
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		public IEnumerable<Vector2Int> GetCells(Item item) {
			var transform = this;
			return item.Cells.Select(cell => transform.Transform(cell, item));
		}

		public IEnumerable<Tuple<Vector2Int, Sprite, EdgeInfo>> GetSprites(Item item) {
			var cells = GetCells(item).ToList();
			foreach ( var cell in item.Cells ) {
				var v = Transform(cell, item);
				var s = item.GetSprite(cell);
				
				var left = !cells.Contains(v + Vector2Int.left);
				var right = !cells.Contains(v + Vector2Int.right);
				var top = !cells.Contains(v + Vector2Int.down);
				var bottom = !cells.Contains(v + Vector2Int.up);
				
				var edgeInfo = new EdgeInfo(left, right, top, bottom);
				
				yield return new Tuple<Vector2Int, Sprite, EdgeInfo>(v, s, edgeInfo);
			}
		}

		public float GetAngle() {
			return 90f * rotateIndex;
		}

		public bool Equals(ItemTransform other)
		{
			return position.Equals(other.position) && rotateIndex.Equals(other.rotateIndex);
		}

		public void Rotate() {
			rotateIndex = (rotateIndex + 1) % 4;
		}

		public void SetPosition(Vector2Int pos) {
			position = pos;
		}

		public override string ToString() {
			return $"({position.x}, {position.y}: {rotateIndex})";
		}
	}
}