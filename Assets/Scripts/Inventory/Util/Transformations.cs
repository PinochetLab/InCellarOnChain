using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Inventory {
	
	using DictType = Dictionary<Item, List<ItemTransform>>;
	public class Transformations {
		public DictType Value { get; } = new ();

		public Transformations() { }

		public Transformations(DictType value) {
			Value = value;
		}

		public Transformations(Transformations other) {
			foreach ( var (item, transforms) in other.Value ) {
				Value.Add(item, transforms.ToList());
			}
		}
		
		public void Clear() {
			Value.Clear();
		}

		public void Add(Item item, ItemTransform it) {
			if ( !Value.ContainsKey(item) ) {
				Value.Add(item, new List<ItemTransform>());
			}
			Value[item].Add(it);
		}

		public void Remove(Item item, ItemTransform it) {
			if ( !Value.TryGetValue(item, out var transformation) ) {
				throw new KeyNotFoundException($"Item {item} not found");
			}
			transformation.Remove(it);
			if ( transformation.Count == 0 ) {
				Value.Remove(item);
			}
		}

		public bool Contains(Item item) {
			return Value.ContainsKey(item) && Value[item].Count > 0;
		}
		
		public bool IsEmpty() => Value.Count == 0;
		
		public IEnumerator<KeyValuePair<Item, ItemTransform>> GetEnumerator()
		{
			foreach (var (item, transforms) in Value)
			{
				foreach ( var it in transforms ) {
					yield return new KeyValuePair<Item, ItemTransform>(item, it);
				}
			}
		}

		public override string ToString() {
			var result = string.Empty;
			foreach (var (item, transforms) in Value) {
				result += item + " ";
				result = transforms.Aggregate(result, (current, it) => current + (it + " "));
				result += Environment.NewLine;
			}
			return result;
		}
	}
}