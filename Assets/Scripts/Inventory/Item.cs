using UnityEngine;

namespace Inventory {
	[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 0)]
	public class Item : ScriptableObject {
		[SerializeField] private Sprite icon;
		[SerializeField] private Vector2Int size = Vector2Int.one;
		
		public Sprite Icon => icon;
		public Vector2Int Size => size;
	}
}