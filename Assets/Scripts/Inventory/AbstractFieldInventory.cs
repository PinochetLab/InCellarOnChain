using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory {
	using ItemTransformations = Dictionary<Item, List<ItemTransform>>;
	using ItemSlots = List<List<InventorySlot>>;

	public class Configuration {
		public readonly ItemTransformations Transformations = new ();
		public readonly ItemSlots Slots = new();

		public Configuration(ItemTransformations transformations, ItemSlots slots) {
			foreach ( var (item, transforms) in transformations ) {
				Transformations.Add(item, transforms.Select(it => new ItemTransform(it)).ToList());
			}

			foreach ( var row in slots ) {
				Slots.Add(row.Select(slot => new InventorySlot(slot)).ToList());
			}
			//Slots = slots.Select(row => row.ToList()).ToList();
		}

		public void Print() {
			string s = "";
			for ( var y = 0; y < Slots.Count; y++ ) {
				for ( var x = 0; x < Slots[y].Count; x++ ) {
					s += (Slots[y][x].IsEmpty() ? 0 : 1) + " ";
				}

				s += "\n";
			}
			Debug.Log(s);
			foreach ( var (item, transforms) in Transformations ) {
				Debug.Log(item);
				foreach ( var it in transforms ) {
					Debug.Log(it.Position);
				}
			}
		}
	}
	public abstract class AbstractFieldInventory : AbstractInventory {
		protected const int Width = 4;
		protected const int Height = 4;
		
		private ItemLocator _itemLocator;
		
		private ItemTransformations _itemTransformations = new ();
		
		private ItemSlots _slots = new ();

		public UnityEvent<ItemTransformations> OnUpdated { get; } = new ();
		public UnityEvent<ItemSlots> OnGenerate { get; } = new ();

		public UnityEvent OnStartMove { get; } = new ();
		public UnityEvent OnFinishMove { get; } = new ();

		public void SetLocator(ItemLocator itemLocator) {
			_itemLocator = itemLocator;
		}

		public abstract void Init();

		protected void SetUp() {
			_slots.Clear();
			_itemTransformations.Clear();
			for ( var y = 0; y < Height; y++ ) {
				var slots = Enumerable.Range(0, Width).Select(_ => InventorySlot.CreateEmpty()).ToList();
				_slots.Add(slots);
			}
			OnGenerate?.Invoke(_slots);
		}

		public bool AddItem(Item item) {
			for ( var y = 0; y < Height; y++ )
			for ( var x = 0; x < Width; x++ ) {
				var position = new Vector2Int(x, y);
				foreach (var itemTransform in new List<bool> { true, false }.Select(rot => new ItemTransform(position, rot))) {
					if ( !CanLocateItem(item, itemTransform) ) {
						continue;
					}

					AddItem(item, itemTransform);
					return true;
				}
			}
			return false;
		}
		
		public void AddItem(Item item, ItemTransform itemTransform) {
			AddTransform(item, itemTransform);
			
			itemTransform.GetSlots(item).ToList().ForEach(v => _slots[v.y][v.x].SetItem(item));
			OnUpdated?.Invoke(_itemTransformations);
		}
		
		private bool IsPositionValid(Vector2Int position) {
			return position.x is >= 0 and < Width && 
			       position.y is >= 0 and < Height;
		}

		private bool IsSlotEmpty(Vector2Int position) {
			return _slots[position.y][position.x].IsEmpty();
		}

		private void AddTransform(Item item, ItemTransform itemTransform) {
			if ( !_itemTransformations.ContainsKey(item) ) {
				_itemTransformations.Add(item, new List<ItemTransform>());
			}
			_itemTransformations[item].Add(itemTransform);
		}
		
		private void RemoveTransform(Item item, ItemTransform itemTransform) {
			if ( !_itemTransformations.TryGetValue(item, out var transformation) ) {
				throw new KeyNotFoundException($"Item {item} not found");
			}
			transformation.Remove(itemTransform);
			if ( transformation.Count == 0 ) {
				_itemTransformations.Remove(item);
			}
		}

		public Configuration SaveConfiguration() {
			var configuration = new Configuration(_itemTransformations, _slots);
			return configuration;
		}

		public void LoadConfiguration(Configuration configuration) {
			_itemTransformations = configuration.Transformations;
			_slots = configuration.Slots;
			OnUpdated?.Invoke(_itemTransformations);
		}

		public void Clear() {
			_itemTransformations.Clear();
			_slots.ForEach(slots => slots.ForEach(slot => slot.SetEmpty()));
			OnUpdated?.Invoke(_itemTransformations);
		}
		
		public void TryMoveItem(Item item, ItemTransform itemTransform) {
			/*if ( !_itemTransformations.TryGetValue(item, out var transformation) ) {
				return;
			}*/
			OnStartMove?.Invoke();
			RemoveTransform(item, itemTransform);
			
			OnUpdated?.Invoke(_itemTransformations);
			
			itemTransform.GetSlots(item).ToList().ForEach(v => _slots[v.y][v.x].SetEmpty());

			_itemLocator.TryLocateItem(item, itemTransform, this, TryMoveCallback);
			return;

			void TryMoveCallback(ItemTransform? optionalTransform) {
				OnFinishMove?.Invoke();
				if ( optionalTransform.HasValue && optionalTransform.Value.Equals(ItemTransform.Moved) ) {
					return;
				}
				var it = optionalTransform ?? itemTransform;
				AddItem(item, it);
				it.GetSlots(item).ToList().ForEach(v => _slots[v.y][v.x].SetItem(item));
				OnUpdated?.Invoke(_itemTransformations);
			}
		}
		
		public bool CanLocateItem(Item item, ItemTransform itemTransform) {
			return itemTransform.GetSlots(item).All(v => IsPositionValid(v) && IsSlotEmpty(v));
		}
		
		public override bool HasItem(Item item) {
			return _itemTransformations.ContainsKey(item) && _itemTransformations[item].Count > 0;
		}
		
		public override bool IsEmpty() {
			return _itemTransformations.Count == 0;
		}
	}
}