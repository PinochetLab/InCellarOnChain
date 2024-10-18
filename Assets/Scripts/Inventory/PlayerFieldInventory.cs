using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Inventory {
	public sealed class PlayerFieldInventory : AbstractFieldInventory {
		[SerializeField] private FieldInventory secondInventory;

		[SerializeField] private Item testItem;

		public override void Init() {
			SetUp();
			AddItem(testItem, new ItemTransform(new Vector2Int(0, 0), false));
			AddItem(testItem, new ItemTransform(new Vector2Int(2, 0), true));
		}
		
		public override void TryAddItem(Item item, AddItemCallback callback) {
			//TODO
		}
	}
}