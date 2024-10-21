using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory {
	[Serializable]
	public struct ItemTransform : IEquatable<ItemTransform> {
		private Vector2Int _position;

		private int _rotateIndex;

		public Vector2Int Pos => _position;
		
		public static ItemTransform Moved => new (-Vector2Int.one, 0);

		public Vector2Int RotatedSize(Item item) {
			var size = item.BoundingSize;
			if ( _rotateIndex % 2 == 1 ) {
				size = new Vector2Int(size.y, size.x);
			}
			return size;
		}
		
		public bool IsMoved => _position == -Vector2Int.one && _rotateIndex == 0;

		public ItemTransform(Vector2Int position, int rotateIndex) {
			_position = position;
			_rotateIndex = rotateIndex;
		}
		
		public ItemTransform(ItemTransform other) {
			_position = other._position;
			_rotateIndex = other._rotateIndex;
		}

		private Vector2Int Transform(Vector2Int cell, Item item) {
			var boundingSize = item.BoundingSize;
			var x = cell.x;
			var y = cell.y;
			return _position + _rotateIndex switch {
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

		public IEnumerable<Tuple<Vector2Int, Sprite, float>> GetSprites(Item item) {
			foreach ( var cell in item.Cells ) {
				var v = Transform(cell, item);
				var s = item.GetSprite(cell);
				var a = 90f * _rotateIndex;
				yield return new Tuple<Vector2Int, Sprite, float>(v, s, a);
			}
		}

		public bool Equals(ItemTransform other)
		{
			return _position.Equals(other._position) && _rotateIndex.Equals(other._rotateIndex);
		}

		public void Rotate() {
			_rotateIndex = (_rotateIndex + 1) % 4;
		}

		public void SetPosition(Vector2Int position) {
			_position = position;
		}

		public override string ToString() {
			return $"({_position.x}, {_position.y}: {_rotateIndex})";
		}
	}
}