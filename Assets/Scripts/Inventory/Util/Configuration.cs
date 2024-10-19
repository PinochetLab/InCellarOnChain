using System;
using UnityEngine;

namespace Inventory {
	public class Configuration {
		public Transformations Transformations { get; }
		public Slots Slots { get; }
		public Configuration(Transformations transformations, Slots slots) {
			Transformations = new Transformations(transformations);
			Slots = new Slots(slots);
		}
		public override string ToString() {
			Debug.Log(Transformations.IsEmpty());
			return Transformations + Environment.NewLine + Slots;
		}
	}
}