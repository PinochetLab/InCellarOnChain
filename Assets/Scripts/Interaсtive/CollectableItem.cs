using Interaction;
using Inventory;
using UnityEngine;
using UnityEngine.Events;

namespace Interaсtive {
	public class CollectableItem : AbstractInteractable {
		[SerializeField] private Item item;
		
		private readonly UnityEvent _onCollect = new();

		public override void Interact(Character character, UnityAction callback) {
			_onCollect.RemoveAllListeners();
			_onCollect.AddListener(callback);
			var keeper = InteractionNeeds.Get(character).Keeper;
			keeper.TryAddItem(item, ProcessResult);
		}

		private void ProcessResult(bool success) {
			_onCollect?.Invoke();
			if ( success ) {
				DestroyImmediate(gameObject);
			}
		}
	}
}