using UnityEngine;

namespace Interaction {
	public interface IInteractable {
		public Vector3 Position { get; }
		public abstract void Interact();
	}
}