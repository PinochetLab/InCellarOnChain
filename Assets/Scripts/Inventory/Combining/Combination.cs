using System;
using UnityEngine;

namespace Inventory.Combining {
	[CreateAssetMenu(fileName = "Combination", menuName = "Combination", order = 0)]
	public class Combination : ScriptableObject {
		[SerializeField] private Item result;
		[SerializeField] private Item first;
		[SerializeField] private ItemTransform firstTransform;
		[SerializeField] private Item second;
		[SerializeField] private ItemTransform secondTransform;
		
		private ItemTransform GetResultTransform(Item item, ItemTransform localIt, ItemTransform it) {
			var baseIt = new ItemTransform(Vector2Int.zero, it.RotateIndex);
			var v1 = baseIt.Transform(localIt.Position, result);
			v1 -= baseIt.Transform(Vector2Int.zero, item);
			var v2 = Vector2Int.zero;
			var p = it.Position + (v2 - v1);
			return new ItemTransform(p, it.RotateIndex);
		}

		private ItemTransform GetOtherTransform(Item item1, Item item2, ItemTransform it, ItemTransform localIt1, ItemTransform localIt2) {
			var baseIt = new ItemTransform(Vector2Int.zero, it.RotateIndex);
			var v1 = baseIt.Transform(localIt1.Position, result);
			v1 -= baseIt.Transform(Vector2Int.zero, item1);
			var v2 = baseIt.Transform(localIt2.Position, result);
			v2 -= baseIt.Transform(Vector2Int.zero, item2);
			var p = it.Position + (v2 - v1);
			return new ItemTransform(p, it.RotateIndex);
		}
		
		public Item First => first;
		public Item Second => second;

		public Item Result => result;

		public ItemTransform GetResultTransform(Item item, ItemTransform it) {
			if ( item == first ) {
				return GetResultTransform(first, firstTransform, it);
			}
			if (item == second) {
				return GetResultTransform(second, secondTransform, it);
			}
			throw new ArgumentException();
		}

		public ItemTransform GetOtherTransform(Item item, ItemTransform it) {
			if ( item == first ) {
				return GetSecondTransform(it);
			}
			if (item == second) {
				return GetFirstTransform(it);
			}
			throw new ArgumentException();
		}

		private ItemTransform GetSecondTransform(ItemTransform it) {
			return GetOtherTransform(first, second, it, firstTransform, secondTransform);
		}
		
		private ItemTransform GetFirstTransform(ItemTransform it) {
			return GetOtherTransform(second, first, it, secondTransform, firstTransform);
		}
	}
}