using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Interaction.UI {
	
	public delegate void SelectVariantCallback(int index);
	public delegate void ChooseVariantCallback(int index);
	public class InteractionVariantBlock : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler {
		[SerializeField] private List<Image> backgroundImages;
		[SerializeField] private TMP_Text numberText;
		[SerializeField] private TMP_Text descriptionText;
		[SerializeField] private HorizontalLayoutGroup horizontalLayoutGroup;

		private readonly Color _constSelectedColor = Color.green;
		private readonly Color _constDeselectedColor = Color.white;

		private static GameObject _savedPrefab;

		private int _index;
		private char _symbol;
		private SelectVariantCallback _selectCallback;
		private ChooseVariantCallback _chooseCallback;

		private static GameObject SavedPrefab {
			get {
				if ( !_savedPrefab ) {
					_savedPrefab = Resources.Load<GameObject>( "Prefabs/UI/Interaction/Variant Block" );
				}
				return _savedPrefab;
			}
		}

		public static InteractionVariantBlock Instantiate(Transform parent, int index, InteractionVariant variant,
			SelectVariantCallback selectCallback, ChooseVariantCallback chooseCallback) {
			
			var prefab = SavedPrefab;
			var variantBlock = Instantiate(prefab, parent).GetComponent<InteractionVariantBlock>();
			variantBlock._selectCallback = selectCallback;
			variantBlock._chooseCallback = chooseCallback;
			variantBlock.SetUp(index, variant);
			return variantBlock;
		}

		private void SetUp(int index, InteractionVariant variant) {
			_index = index;
			Variant = variant;
			SetNumber(index + 1);
			SetDescription(variant.description);
		}

		public InteractionVariant Variant { get; private set; }

		private void SetNumber(int number) {
			var numberString = number.ToString();
			if ( number >= 10 ) {
				numberString = ('A' + (number - 10)).ToString();
			}
			_symbol = numberString[0];
			numberText.text = numberString;
		}
		
		private void SetDescription(string description) {
			descriptionText.text = description;
			//var sizeDelta = descriptionText.rectTransform.sizeDelta;
			descriptionText.ForceMeshUpdate();
			var width = descriptionText.preferredWidth;
            descriptionText.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			//descriptionText.rectTransform.sizeDelta = sizeDelta;
			//horizontalLayoutGroup.spacing = 20.01f;
			//descriptionText.ForceMeshUpdate();
			LayoutRebuilder.ForceRebuildLayoutImmediate(horizontalLayoutGroup.GetComponent<RectTransform>());
		}
		
		public void OnPointerEnter(PointerEventData eventData) {
			SelectManually();
		}

		private void SelectManually() {
			_selectCallback.Invoke(_index);
		}
		
		public void OnPointerClick(PointerEventData eventData) {
			_chooseCallback.Invoke(_index);
		}
		
		private void ChooseManually() {
			_chooseCallback.Invoke(_index);
		}

		public void Select() {
			SetSelected(true);
		}
		
		public void Deselect() {
			SetSelected(false);
		}

		private void SetSelected(bool selected) {
			foreach (var image in backgroundImages)
			{
				image.color = selected ? _constSelectedColor : _constDeselectedColor;
			}
		}

		private void Update() {
			var c = _symbol;
			var keyName = c switch {
				>= 'A' and <= 'Z' => c.ToString(),
				>= '0' and <= '9' => "Alpha" + c,
				_ => throw new ArgumentException("Don't know name for this key")
			};

			KeyCode kk = (KeyCode)Enum.Parse(typeof(KeyCode), keyName);
			if ( Input.GetKeyDown(kk) ) {
				ChooseManually();
			}
		}
	}
}