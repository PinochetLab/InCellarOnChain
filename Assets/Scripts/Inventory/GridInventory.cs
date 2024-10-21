using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory {
	public class GridInventory {
		private const int Width = 10;
		private const int Height = 10;
		
		public UnityEvent<Transformations> OnUpdate { get; } = new ();
		public UnityEvent<int, int> OnBuild { get; } = new ();

		public UnityEvent<Item, ItemTransform> OnAdd { get; } = new();
		
		public UnityEvent<Item, ItemTransform> OnRemove { get; } = new();
		public UnityEvent OnBeginMove { get; } = new ();
		public UnityEvent OnEndMove { get; } = new ();
		
		protected Transformations Transformations = new ();
		
		protected Slots Slots = new ();
		
		private ItemLocator _itemLocator;

		public void SetLocator(ItemLocator locator) {
			_itemLocator = locator;
		}

		public GridInventory() {
			Build();
		}

		public void Reset() {
			Slots.Clear();
			Transformations.Clear();
			Build();
		}

		public void Clear() {
			Transformations.Clear();
		}
		
		public void Update() {
			OnBuild?.Invoke(Width, Height);
			OnUpdate?.Invoke(Transformations);
		}

		private void Build() {
			for ( var y = 0; y < Height; y++ ) {
				var slots = Enumerable.Range(0, Width).Select(_ => InventorySlot.CreateEmpty()).ToList();
				Slots.Add(slots);
			}
			OnBuild?.Invoke(Width, Height);
		}

		public bool TryAutoAddItem(Item item) {
			var it = FindPlaceForItem(item);
			if ( it.HasValue ) {
				AddItem(item, it.Value);
			}
			return it.HasValue;
		}

		private ItemTransform? FindPlaceForItem(Item item) {
			for ( var y = 0; y < Height; y++ )
			for ( var x = 0; x < Width; x++ ) {
				var position = new Vector2Int(x, y);
				foreach ( var rotateIndex in Enumerable.Range(0, 4) ) {
					var it = new ItemTransform(position, rotateIndex);
					if ( CanPlaceItem(item, it) ) {
						return it;
					}
				}
			}
			return null;
		}
		
		public void AddItem(Item item, ItemTransform it) {
			Transformations.Add(item, it);
			FillItem(item, it);
			OnUpdate?.Invoke(Transformations);
		}
		
		private static bool IsPositionValid(Vector2Int position) {
			return position.x is >= 0 and < Width && 
			       position.y is >= 0 and < Height;
		}

		private bool IsSlotEmpty(Vector2Int position) {
			return Slots.GetSlot(position.x, position.y).IsEmpty();
		}

		private void FillItem(Item item, ItemTransform it) {
			it.GetCells(item).ToList().ForEach(v => Slots.GetSlot(v.x, v.y).SetItem(item));
		}
		
		private void EmptyItem(Item item, ItemTransform it) {
			it.GetCells(item).ToList().ForEach(v => Slots.GetSlot(v.x, v.y).SetEmpty());
		}
		
		public void TryMoveItem(Item item, ItemTransform it) {
			OnBeginMove?.Invoke();
			Transformations.Remove(item, it);
			
			//OnUpdate?.Invoke(Transformations);
			OnRemove?.Invoke(item, it);
			
			EmptyItem(item, it);

			_itemLocator.TryLocateItem(item, it, this, TryMoveCallback);
			return;

			void TryMoveCallback(ItemTransform? maybeTransform) {
				OnEndMove?.Invoke();
				if ( maybeTransform.HasValue && maybeTransform.Value.Equals(ItemTransform.Moved) ) {
					return;
				}
				it = maybeTransform ?? it;
				AddItem(item, it);
				FillItem(item, it);
				//OnUpdate?.Invoke(Transformations);
				OnAdd?.Invoke(item, it);
			}
		}

		public ItemTransform Correct(Item item, ItemTransform it) {
			var size = it.RotatedSize(item);
			var position = it.Pos;
			var old = position;
			position.x = Mathf.Clamp(position.x, 0, Width - size.x);
			position.y = Mathf.Clamp(position.y, 0, Height - size.y);
			it.SetPosition(position);
			return it;
		}
		
		public bool CanPasteItem(Item item, ItemTransform itemTransform) {
			return itemTransform.GetCells(item).All(IsPositionValid);
		}
		
		public bool CanPlaceItem(Item item, ItemTransform itemTransform) {
			return itemTransform.GetCells(item).All(v => IsPositionValid(v) && IsSlotEmpty(v));
		}
		
		public bool HasItem(Item item) {
			return Transformations.Contains(item);
		}
		
		public bool IsEmpty() {
			return Transformations.IsEmpty();
		}

		public Configuration Save() {
			return new Configuration(Transformations, Slots);
		}

		public void Load(Configuration configuration) {
			Transformations = configuration.Transformations;
			Slots = configuration.Slots;
			OnBuild?.Invoke(Width, Height);
			OnUpdate?.Invoke(Transformations);
		}
	}
}