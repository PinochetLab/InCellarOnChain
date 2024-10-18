using System;
using System.Collections;
using System.Collections.Generic;
using Interaction;
using Interaction.UI;
using UnityEngine;

namespace Action {
	public class InteractionTrigger : AbstractInteractionTrigger {
		[SerializeField] private List<InteractionVariant> variants;

		private Character _character;

		//private const float AvailableDelay = 0.3f;

		public override void Notify(Character character, InteractionScreen interactionScreen, SelectCallback callback) {
			switch (variants.Count)
			{
				case 0:
					throw new Exception($"InteractionTrigger({gameObject}) has no variants");
				case 1:
					variants[0].interactable.Interact(character);
					callback?.Invoke();
					break;
				default:
					interactionScreen.TrySelectVariant(variants, OnSelect);
					break;

					void OnSelect(InteractionVariant? variant) {
						Debug.Log("laaal");
						OnSelectVariant(variant);
						callback?.Invoke();
					}
			}
		}

		private void OnSelectVariant(InteractionVariant? variant) {
			if ( !variant.HasValue ) return;
			variant.Value.interactable.Interact(_character);
		}
	}
}