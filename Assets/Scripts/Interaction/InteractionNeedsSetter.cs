using UnityEngine;

namespace Interaction {
	public class InteractionNeedsSetter : MonoBehaviour {
		[SerializeField] private InteractionNeeds playerNeeds;
		[SerializeField] private InteractionNeeds maniacNeeds;

		private void Awake() {
			InteractionNeeds.Add(Character.Player, playerNeeds);
			InteractionNeeds.Add(Character.Maniac, maniacNeeds);
		}
	}
}