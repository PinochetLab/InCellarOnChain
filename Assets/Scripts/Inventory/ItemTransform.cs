using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory {
	public readonly struct ItemTransform : IEquatable<ItemTransform> {
		public Vector2Int Position { get; }
		public bool Rotated { get; }
		
		public static ItemTransform Moved => new ItemTransform(-Vector2Int.one, false);
		
		public bool IsMoved => Position == -Vector2Int.one && !Rotated;

		public ItemTransform(Vector2Int position, bool rotated) {
			Position = position;
			Rotated = rotated;
		}
		
		public ItemTransform(ItemTransform other) {
			Position = other.Position;
			Rotated = other.Rotated;
		}

		public Vector2Int RotatedSize(Item item) {
			var size = item.Size;
			if ( Rotated ) {
				size = new Vector2Int(size.y, size.x);
			}
			return size;
		}

		public IEnumerable<Vector2Int> GetSlots(Item item) {
			var size = RotatedSize(item);
			for ( var i = 0; i < size.x; ++i )
			for ( var j = 0; j < size.y; ++j ) {
				yield return Position + new Vector2Int(i, j);
			}
		} 

		public bool Equals(ItemTransform other)
		{
			return Position.Equals(other.Position) && Rotated.Equals(other.Rotated);
		}
	}
}