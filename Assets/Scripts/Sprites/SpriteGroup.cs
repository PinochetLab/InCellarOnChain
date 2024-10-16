using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Sprites {
	[RequireComponent(typeof(SortingGroup))]
	public class SpriteGroup : MonoBehaviour {
		private SortingGroup _sortingLayer;

		private void Awake() {
			_sortingLayer = GetComponent<SortingGroup>();
		}

		private void Update() {
			_sortingLayer.sortingOrder = Mathf.RoundToInt(-transform.position.z * 100f);
		}
	}
}