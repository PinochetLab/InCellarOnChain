using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Inventory {
	[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 0)]
	public class Item : ScriptableObject {
		
		private const float ButtonSize = 40f;

		[CustomEditor(typeof(Item))]
		private class ItemCe : Editor {
			public override void OnInspectorGUI() {
				base.OnInspectorGUI();
				
				Item item = (Item)target;

				// Отображаем размеры сетки
				var gridSize = item.boundingSize;
				
				item.cells.RemoveAll(v => v.x >= gridSize.x || v.y >= gridSize.y);
				
				if ( gridSize.x <= 0 || gridSize.y <= 0 ) {
					return;
				}
				//EditorGUILayout.LabelField("Grid Size", $"{item.gridSize.x} x {item.gridSize.y}");

				for (int y = 0; y < gridSize.y; y++)
				{
					EditorGUILayout.BeginHorizontal();
					for (int x = 0; x < gridSize.x; x++)
					{
						Vector2Int cellPosition = new Vector2Int(x, y);
						bool isCellSelected = item.cells.Contains(cellPosition);
                
						// Применяем цвет в зависимости от состояния клетки
						GUI.backgroundColor = isCellSelected ? Color.green : Color.white;
						
						float inspectorWidth = EditorGUIUtility.currentViewWidth - 45;
						float width = inspectorWidth / gridSize.x;

						if (GUILayout.Button(" ", GUILayout.Width(width), GUILayout.Height(width)))
						{
							// Добавление или удаление клетки при нажатии
							if (isCellSelected)
							{
								item.cells.Remove(cellPosition);
							}
							else
							{
								item.cells.Add(cellPosition);
							}
						}

						GUI.backgroundColor = Color.white; // Сбрасываем цвет
					}
					EditorGUILayout.EndHorizontal();
				}

				// Обновляем инспектор
				if (GUI.changed)
				{
					EditorUtility.SetDirty(item);
				}
			}
		} 
		
		[SerializeField] private Sprite icon;
		[SerializeField] private Vector2Int size = Vector2Int.one;
        [SerializeField] private Vector2Int boundingSize;
        
        [SerializeField] private List<Vector2Int> cells;
		
		public Sprite Icon => icon;
		public Vector2Int Size => size;
	}
}