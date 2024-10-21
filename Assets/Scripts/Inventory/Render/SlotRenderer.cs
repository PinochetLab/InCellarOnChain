using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.Render {
	public class SlotRenderer : MonoBehaviour, IPointerDownHandler {
		[SerializeField] private GameObject itemGo;
		[SerializeField] private GameObject emptyGo;
		[SerializeField] private GameObject greenSelectionGo;
		[SerializeField] private GameObject redSelectionGo;
		[SerializeField] private Image itemImage;

		[SerializeField] private GameObject leftBorder;
		[SerializeField] private GameObject rightBolder;
		[SerializeField] private GameObject topBolder;
		[SerializeField] private GameObject bottomBolder;

		private UnityEvent _onClick = new();

		public void SetEmpty() {
			SetSelected(false);
			_onClick.RemoveAllListeners();
			emptyGo.SetActive(true);
			itemGo.SetActive(false);
		}

		public void SetSelected(bool selected, bool green = true) {
			greenSelectionGo.SetActive(selected && green);
			redSelectionGo.SetActive(selected && !green);
		}
		
		public void SetItem(Item item, Sprite sprite, Vector2Int v, List<Vector2Int> cells, float angle, UnityAction onClick) {
			leftBorder.SetActive(!cells.Contains(v + Vector2Int.left));
			rightBolder.SetActive(!cells.Contains(v + Vector2Int.right));
			topBolder.SetActive(!cells.Contains(v + Vector2Int.down));
			bottomBolder.SetActive(!cells.Contains(v + Vector2Int.up));
			SetSelected(false);
			_onClick.RemoveAllListeners();
			_onClick.AddListener(onClick);
			emptyGo.SetActive(false);
			itemGo.SetActive(true);
			itemImage.sprite = sprite;
			itemImage.transform.localEulerAngles = Vector3.forward * angle;
		}

		public void OnPointerDown(PointerEventData eventData) {
			_onClick.Invoke();
		}
	}
}