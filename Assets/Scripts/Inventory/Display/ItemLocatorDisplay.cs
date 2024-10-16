using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Display {
	public class ItemLocatorDisplay : AbstractSlotDisplay {
		[SerializeField] private Image itemImage;
		
		private Item _item;
		private bool _rotated;

		public void SetUp(Item item, bool rotated) {
			_item = item;
			itemImage.sprite = item.Icon;
			_rotated = rotated;
			UpdateRotate();
		}
		
		public bool Rotated => _rotated;

		public void Rotate() {
			_rotated = !_rotated;
			UpdateRotate();
		}

		private void UpdateRotate() {
			var s = _item.Size;
			if ( _rotated ) {
				s = new Vector2Int(s.y, s.x);
			}
			SetCellSize(s);
			var r = itemImage.rectTransform;
			r.sizeDelta = (Vector2)s * Constants.CellSize;
			
			if ( _rotated ) {
				
				var size = _item.Size;
				var a1 = (float)size.x / size.y;
				var a2 = 1 / a1;
				r.rotation = Quaternion.Euler(0f, 0f, 90f);
				r.localScale = new Vector3(a1, a2, 1);
			}
			else {
				r.rotation = Quaternion.identity;
				r.localScale = Vector3.one;
			}
		}

		public Vector2 ActualSize => rectTransform.rect.size;
	}
}