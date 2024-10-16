using System;
using System.Threading.Tasks;
using Async;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.Display {
	public class ItemSlotDisplay : AbstractSlotDisplay {
		[SerializeField] private Image itemImage;
		
		private Item _item;
		private ItemTransform _itemTransform;
		private PlayerInventory _playerInventory;

		public void SetUp(Item item, ItemTransform itemTransform, PlayerInventory inventory) {
			_item = item;
			itemImage.sprite = item.Icon;
			if ( itemTransform.Rotated ) {
				var r = itemImage.rectTransform;
				var size = item.Size;
				var a1 = (float)size.x / size.y;
				var a2 = 1 / a1;
				r.rotation = Quaternion.Euler(0f, 0f, 90f);
				r.localScale = new Vector3(a1, a2, 1);
			}
			_itemTransform = itemTransform;
			_playerInventory = inventory;
		}

		public void OnClick() {
			_playerInventory.TryMoveItem(_item, _itemTransform);
		}
	}
}