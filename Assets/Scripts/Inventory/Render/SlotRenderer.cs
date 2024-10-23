using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.Render {
	public class SlotRenderer : MonoBehaviour, IPointerDownHandler {
		[SerializeField] private GameObject itemGo;
		[SerializeField] private GameObject emptyGo;
		[SerializeField] private Image itemImage;

		[SerializeField] private GameObject leftBorder;
		[SerializeField] private GameObject rightBolder;
		[SerializeField] private GameObject topBolder;
		[SerializeField] private GameObject bottomBolder;

		private UnityEvent _onClick = new();

		public void SetEmpty() {
			_onClick.RemoveAllListeners();
			emptyGo.SetActive(true);
			itemGo.SetActive(false);
		}
		
		public void SetItem(Sprite sprite, EdgeInfo edgeInfo, float angle, UnityAction onClick) {
			leftBorder.SetActive(edgeInfo.Left);
			rightBolder.SetActive(edgeInfo.Right);
			topBolder.SetActive(edgeInfo.Top);
			bottomBolder.SetActive(edgeInfo.Bottom);
			
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