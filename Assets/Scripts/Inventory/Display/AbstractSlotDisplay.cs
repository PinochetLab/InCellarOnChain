using UnityEngine;

namespace Inventory.Display {
	public abstract class AbstractSlotDisplay : MonoBehaviour {
		[SerializeField] protected RectTransform rectTransform;

		public void SetCellSize(Vector2Int size) {
			rectTransform.sizeDelta = new Vector2(size.x, size.y) * Constants.CellSize;
		}

		public void SetCellPosition(Vector2Int position) {
			position.y *= -1;
			rectTransform.anchoredPosition = new Vector2(position.x, position.y) * Constants.CellSize;
		}

		public void SetPosition(Vector2 position) {
			position.y *= -1;
			rectTransform.anchoredPosition = position;
		}
	}
}