using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Render {
	public delegate void ClickCallback();
	public class ItemSlotDisplay : AbstractSlotDisplay {
		[SerializeField] private Image itemImage;
		private ClickCallback _clickCallback;

		public void SetUp(Item item, ItemTransform itemTransform, ClickCallback clickCallback) {
			itemImage.sprite = item.Icon;
			if ( itemTransform.Rotated ) {
				var r = itemImage.rectTransform;
				var size = item.Size;
				var a1 = (float)size.x / size.y;
				var a2 = 1 / a1;
				r.rotation = Quaternion.Euler(0f, 0f, 90f);
				r.localScale = new Vector3(a1, a2, 1);
			}
			_clickCallback = clickCallback;
		}

		public void OnClick() {
			_clickCallback?.Invoke();
		}
	}
}