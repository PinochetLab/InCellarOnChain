using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Interaction.UI {
	
	public delegate void VariantChoiceCallback(InteractionVariant? variant);
	public class InteractionScreen : MonoBehaviour {
		[SerializeField] private GameObject interactionWindow;
		[SerializeField] private GameObject hintKeyGo;
		[SerializeField] private GameObject hintVariantsGo;
		[SerializeField] private RectTransform hintRt;
		[SerializeField] private Transform variantBlockParent;

		[SerializeField] private PlayerMover playerMover;

		private readonly List<InteractionVariantBlock> _variantBlocks = new();
		private VariantChoiceCallback _callback;

		private bool _selecting;
		private int _selectedIndex;
		private bool _canUseInteractionButton;

		private void ShowKeyHint(bool active) {
			hintKeyGo.SetActive(active);
			hintVariantsGo.SetActive(!active);
		}

		public void SetActive(bool active) {
			interactionWindow.SetActive(active);
		}

		public void SetHintPosition(Vector3 position) {
			var hintPosition = Camera.main.WorldToScreenPoint(position);
			hintRt.anchoredPosition = hintPosition;
		}

		private void Awake() {
			ShowKeyHint(true);
			SetActive(false);
		}

		public void TrySelectVariant(List<InteractionVariant> variants, VariantChoiceCallback callback) {
			ShowKeyHint(false);
			_selecting = true;
			while ( variantBlockParent.childCount > 0 ) {
				DestroyImmediate(variantBlockParent.GetChild(0).gameObject);
			}
			_callback = callback;
			_variantBlocks.Clear();
			for ( var i = 0; i < variants.Count; i++ ) {
				var variant = variants[i];
				var variantBlock = InteractionVariantBlock.Instantiate(
					variantBlockParent, i, variant, SelectVariant, ChooseVariant);
				_variantBlocks.Add(variantBlock);
			}
			SelectVariant(0);
			_canUseInteractionButton = false;
			StartCoroutine(WaitForFrame());
		}

		private IEnumerator WaitForFrame() {
			yield return new WaitUntil(() => !Input.GetButtonDown("Action"));
			_canUseInteractionButton = true;
		}

		private void SelectVariant(int i) {
			_selectedIndex = i;
			_variantBlocks.ForEach(block => block.Deselect());
			_variantBlocks[i].Select();
		}

		private void ChooseVariant(int i) {
			_selecting = false;
			ShowKeyHint(true);
			_callback?.Invoke(_variantBlocks[i].Variant);
		}

		private void Update() {
			if ( !_selecting ) return;
			ProcessChooseButtons();
			ProcessArrowsButtons();
		}

		private void ProcessCancel() {
			if (!Input.GetButtonDown("Cancel") || !playerMover.IsMoving()) return;
			Cancel();
		}

		public void Cancel() {
			_selecting = false;
			ShowKeyHint(true);
			_callback?.Invoke(null);
		}

		private void ProcessChooseButtons() {
			if ( !_canUseInteractionButton ) return;
			if ( !Input.GetButtonDown("Submit") && 
			     !(_canUseInteractionButton && Input.GetButtonDown("Interact")) ) return;
			ChooseVariant(_selectedIndex);
		}

		private void ProcessArrowsButtons() {
			if ( Input.GetKeyDown(KeyCode.UpArrow) ) {
				_selectedIndex--;
				if (_selectedIndex < 0) _selectedIndex = _variantBlocks.Count - 1;
				SelectVariant(_selectedIndex);
			}
			else if ( Input.GetKeyDown(KeyCode.DownArrow) ) {
				_selectedIndex++;
				_selectedIndex %= _variantBlocks.Count;
				SelectVariant(_selectedIndex);
			}
		}
	}
}