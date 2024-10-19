using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Inventory {
	
	using ListType = List<List<InventorySlot>>;
	
	public class Slots {
		private readonly ListType _list;

		public Slots() {
			_list = new ListType();
		}

		public Slots(Slots other) {
			_list = other._list.Select(row => row.Select(slot => new InventorySlot(slot)).ToList()).ToList();
		}

		public void Clear() {
			_list.Clear();
		}

		public void Add(List<InventorySlot> row) {
			_list.Add(row);
		}

		public InventorySlot GetSlot(int x, int y) => _list[y][x];
		
		public IEnumerator<KeyValuePair<Vector2Int, InventorySlot>> GetEnumerator()
		{
			for ( var y = 0; y < _list.Count; y++ )
			for ( var x = 0; x < _list[y].Count; x++ ) {
				yield return new KeyValuePair<Vector2Int,InventorySlot>(new Vector2Int(x, y), GetSlot(x, y));
			}
		}

		public override string ToString() {
			var result = string.Empty;
			foreach (var t in _list) {
				result = t.Aggregate(result, (current, t1) => current + (t1 + " "));
				result += Environment.NewLine;
			}
			return result;
		}
	}
}