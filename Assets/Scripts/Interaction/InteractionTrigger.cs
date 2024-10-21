using System;
using System.Collections;
using System.Collections.Generic;
using Interaction;
using Interaction.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Action {
	public class InteractionTrigger : AbstractInteractionTrigger {
		[SerializeField] private List<InteractionVariant> variants;

		public override void Notify(Character character, InteractionScreen interactionScreen, UnityAction callback) {
			switch (variants.Count)
			{
				case 0:
					throw new Exception($"InteractionTrigger({gameObject}) has no variants");
				case 1:
					variants[0].interactable.Interact(character, callback);
					break;
				default:
					interactionScreen.TrySelectVariant(variants, OnSelect);
					break;

					void OnSelect(InteractionVariant? variant) {
						if ( !variant.HasValue ) {
							callback?.Invoke();
							return;
						}
						var v = variant.Value;
						v.interactable.Interact(character, callback, v.name);
					}
			}
		}
	}
}