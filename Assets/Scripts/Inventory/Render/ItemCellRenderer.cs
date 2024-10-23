using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Render {
	public class ItemCellRenderer : MonoBehaviour {
		[SerializeField] private Image itemImage;
		[SerializeField] private RectTransform rectTransform;
		[SerializeField] private Image colorImage;
		
		[SerializeField] private GameObject leftBorder;
		[SerializeField] private GameObject rightBolder;
		[SerializeField] private GameObject topBolder;
		[SerializeField] private GameObject bottomBolder;

		public void SetUp(Vector2Int cell, Sprite sprite, EdgeInfo edgeInfo) {
			cell.y *= -1;
			rectTransform.anchoredPosition = (Vector2)cell * Constants.CellSize;
			leftBorder.SetActive(edgeInfo.Left);
			rightBolder.SetActive(edgeInfo.Right);
			topBolder.SetActive(edgeInfo.Top);
			bottomBolder.SetActive(edgeInfo.Bottom);
			
			itemImage.sprite = sprite;
		}

		public void SetColor(Color color) {
			colorImage.color = color;
		}
	}
}