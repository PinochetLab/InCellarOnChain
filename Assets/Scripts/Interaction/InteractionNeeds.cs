using System;
using System.Collections.Generic;
using Inventory;
using UnityEngine;

namespace Interaction {
	[Serializable]
	public class InteractionNeeds {
		[SerializeField] private Transform characterTransform;
		[SerializeField] private AbstractInventory inventory;
		
		private static Dictionary<Character, InteractionNeeds> _characterNeeds = new ();

		public static void Add(Character character, InteractionNeeds needs) {
			_characterNeeds.Add(character, needs);
		}
		
		public static InteractionNeeds Get(Character character) => _characterNeeds[character];

		public Vector3 CharacterPosition => characterTransform.position;
		
		public IInventory Inventory => inventory;
	}
}