using UnityEngine;

namespace Inventory.Combining {
	[CreateAssetMenu(fileName = "Combination", menuName = "Combination", order = 0)]
	public class Combination : ScriptableObject {
		[SerializeField] private Item result;
		[SerializeField] private Item first;
		[SerializeField] private ItemTransform firstTransform;
		[SerializeField] private Item second;
		[SerializeField] private ItemTransform secondTransform;
	}
}