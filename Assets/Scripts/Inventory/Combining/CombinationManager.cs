using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Combining {

	public class CombinationAnswer {
		public ItemTransform TargetIt;
		public Item OtherItem;
		public ItemTransform OtherIt;
		public Item ResultItem;
		public ItemTransform ResultIt;

		public CombinationAnswer(
			ItemTransform targetIt,
			Item otherItem,
			ItemTransform otherIt,
			Item resultItem,
			ItemTransform resultIt
			) {
			TargetIt = targetIt;
			OtherItem = otherItem;
			OtherIt = otherIt;
			ResultItem = resultItem;
			ResultIt = resultIt;
		}
	}
	public class CombinationManager : MonoBehaviour {
		[SerializeField] private List<Combination> combinations;
		
		private readonly Dictionary<Item, Dictionary<Item, Combination>> _combinationsByItem = new ();

		private void AddCombination(Item item1, Item item2, Combination combination) {
			var cs = _combinationsByItem.GetValueOrDefault(item1, new Dictionary<Item, Combination>());
			cs.Add(item2, combination);
			_combinationsByItem[item1] = cs;
		}

		private void Awake() {
			foreach ( var combination in combinations ) {
				AddCombination(combination.First, combination.Second, combination);
				AddCombination(combination.Second, combination.First, combination);
			}
		}

		public IEnumerable<CombinationAnswer> GetAnswers(Item item, Transformations allItems) {
			if ( !_combinationsByItem.TryGetValue(item, out var cs) ) {
				yield break;
			}
			foreach ( var (otherItem, otherIt) in allItems ) {
				if ( !cs.TryGetValue(otherItem, out var c) ) {
					continue;
				}
				var currentIt = c.GetOtherTransform(otherItem, otherIt);
				var resultIt = c.GetResultTransform(otherItem, otherIt);
				yield return new CombinationAnswer(currentIt, otherItem, otherIt, c.Result, resultIt);
			}
		}
	}
}