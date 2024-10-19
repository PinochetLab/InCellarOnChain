using UnityEngine;

namespace Inventory.Render {
	public class GridInventoryRenderer : MonoBehaviour {
		[SerializeField] private GameObject window;
		[SerializeField] private GameObject emptySlotPrefab;
		[SerializeField] private GameObject itemSlotPrefab;
		[SerializeField] private Transform emptySlotParent;
		[SerializeField] private Transform itemSlotParent;
		[SerializeField] private RectTransform locatorRt;

		public RectTransform LocatorRt => locatorRt;
		
		public GridInventory GridInventory { get; private set; }

		public void Show() {
			SetActive(true);
		}
		
		public void Hide() {
			SetActive(false);
		}

		private void SetActive(bool active) {
			window.SetActive(active);
		}

		public void SetInventory(GridInventory gridInventory) {
			GridInventory = gridInventory;
			
			GridInventory.OnBuild.RemoveAllListeners();
			GridInventory.OnUpdate.RemoveAllListeners();
			
			GridInventory.OnBuild.AddListener(Build);
			GridInventory.OnUpdate.AddListener(UpdateItems);
			
			GridInventory.Update();
		}
		
		private void Build(Slots slots) {
			while ( itemSlotParent.childCount > 0 ) {
				DestroyImmediate(itemSlotParent.GetChild(0).gameObject);
			}

			foreach (var (position, _) in slots) {
				var emptySlotGo = Instantiate(emptySlotPrefab, emptySlotParent);
				var emptySlotDisplay = emptySlotGo.GetComponent<EmptySlotDisplay>();
				
				emptySlotDisplay.SetCellPosition(position);
				emptySlotDisplay.SetCellSize(Vector2Int.one);
			}
		}
		
		private void UpdateItems(Transformations transformations) {
			while ( itemSlotParent.childCount > 0 ) {
				DestroyImmediate(itemSlotParent.GetChild(0).gameObject);
			}

			foreach ( var (item, it) in transformations ) {
				var itemSlotGo = Instantiate(itemSlotPrefab, itemSlotParent);
				var itemSlotDisplay = itemSlotGo.GetComponent<ItemSlotDisplay>();

				itemSlotDisplay.SetUp(item, it, TryMove);
				
				itemSlotDisplay.SetCellPosition(it.Position);
				itemSlotDisplay.SetCellSize(it.RotatedSize(item));
				continue;

				void TryMove() {
					GridInventory.TryMoveItem(item, it);
				}
			}
		}
	}
}