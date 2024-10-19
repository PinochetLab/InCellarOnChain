using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Inventory {
	
	using DictType = Dictionary<Item, List<ItemTransform>>;
	public class Transformations {
		private readonly DictType _dict = new DictType();

		public Transformations() { }

		public Transformations(Transformations other) {
			foreach ( var (item, transforms) in other._dict ) {
				_dict.Add(item, transforms.ToList());
			}
		}
		
		public void Clear() {
			_dict.Clear();
		}

		public void Add(Item item, ItemTransform it) {
			if ( !_dict.ContainsKey(item) ) {
				_dict.Add(item, new List<ItemTransform>());
			}
			_dict[item].Add(it);
		}

		public void Remove(Item item, ItemTransform it) {
			if ( !_dict.TryGetValue(item, out var transformation) ) {
				throw new KeyNotFoundException($"Item {item} not found");
			}
			transformation.Remove(it);
			if ( transformation.Count == 0 ) {
				_dict.Remove(item);
			}
		}

		public bool Contains(Item item) {
			return _dict.ContainsKey(item) && _dict[item].Count > 0;
		}
		
		public bool IsEmpty() => _dict.Count == 0;
		
		public IEnumerator<KeyValuePair<Item, ItemTransform>> GetEnumerator()
		{
			foreach (var (item, transforms) in _dict)
			{
				foreach ( var it in transforms ) {
					yield return new KeyValuePair<Item, ItemTransform>(item, it);
				}
			}
		}

		public override string ToString() {
			var result = string.Empty;
			foreach (var (item, transforms) in _dict) {
				result += item + " ";
				result = transforms.Aggregate(result, (current, it) => current + (it + " "));
				result += Environment.NewLine;
			}
			return result;
		}
	}
}