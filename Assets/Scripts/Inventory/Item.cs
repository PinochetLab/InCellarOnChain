using System;
using System.Collections.Generic;
using System.Threading.Tasks;
#if UNITY_EDITOR
using System.IO;
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Serialization;

namespace Inventory {
	[CreateAssetMenu(fileName = "Item", menuName = "Item", order = 0)]
	public class Item : ScriptableObject {

#if UNITY_EDITOR
		[CustomEditor(typeof(Item))]
		private class ItemCe : Editor {
			public override void OnInspectorGUI() {
				base.OnInspectorGUI();
				Item item = (Item)target;
				if ( GUILayout.Button("Generate Sprites") ) {
					item.GenerateSprites();
				}
				var gridSize = item.boundingSize;
				item.cells.RemoveAll(v => v.x >= gridSize.x || v.y >= gridSize.y);
				if ( gridSize.x <= 0 || gridSize.y <= 0 ) {
					return;
				}
				for (var y = 0; y < gridSize.y; y++)
				{
					EditorGUILayout.BeginHorizontal();
					for (var x = 0; x < gridSize.x; x++)
					{
						Vector2Int cellPosition = new Vector2Int(x, y);
						var isCellSelected = item.cells.Contains(cellPosition);
						GUI.backgroundColor = isCellSelected ? Color.green : Color.white;
						
						var inspectorWidth = EditorGUIUtility.currentViewWidth - 45;
						var width = inspectorWidth / gridSize.x;

						if (GUILayout.Button(" ", GUILayout.Width(width), GUILayout.Height(width)))
						{
							if (isCellSelected)
							{
								item.cells.Remove(cellPosition);
							}
							else
							{
								item.cells.Add(cellPosition);
							}
						}

						GUI.backgroundColor = Color.white;
					}
					EditorGUILayout.EndHorizontal();
				}

				if (GUI.changed)
				{
					EditorUtility.SetDirty(item);
				}
			}
		}
#endif
		
		[SerializeField] private Sprite icon;
        [SerializeField] private Vector2Int boundingSize;
        
        [SerializeField] private List<Vector2Int> cells;

        [Serializable]
        private class SpriteList {
	        [SerializeField] private List<Sprite> sprites;

	        public SpriteList(List<Sprite> sprites) {
		        this.sprites = sprites;
	        }
	        public List<Sprite> Sprites => sprites;
        }

        [SerializeField] private List<Sprite> sprites;
		
		public Sprite Icon => icon;
		
		public Vector2Int BoundingSize => boundingSize;
		
		public List<Vector2Int> Cells => cells;

		#if UNITY_EDITOR
		
        [ExecuteInEditMode]
		private void GenerateSprites() {
			DivideSprite(icon, boundingSize.x, boundingSize.y);
		}
		
		[ExecuteInEditMode]
		private void DivideSprite(Sprite originalSprite, int width, int height)
		{
			var path = $"Assets/ItemSprites/{name}/{name}{0}.png";
			var dir = Path.GetDirectoryName(path);
				
			Directory.CreateDirectory(dir);
				
			FileInfo[] files = new DirectoryInfo(dir).GetFiles();
			foreach (FileInfo file in files)
			{
				file.Delete();
			}
			
			sprites.Clear();
			Texture2D texture = originalSprite.texture;
			var pieceWidth = texture.width / (float)width;
			var pieceHeight = texture.height / (float)height;
			var index = 0;
			foreach (var cell in cells) {
				var x = cell.x;
				var y = height - 1 - cell.y;
				Rect rect = new Rect(x * pieceWidth, y * pieceHeight, pieceWidth, pieceHeight);
				Vector2 pivot = new Vector2(0.5f, 0.5f);

				Texture2D pieceTexture = new Texture2D((int)pieceWidth, (int)pieceHeight);
				pieceTexture.SetPixels(texture.GetPixels((int)rect.x, (int)rect.y, (int)pieceWidth, (int)pieceHeight));
				pieceTexture.Apply();

				Sprite newSprite = SaveSprite(pieceTexture, index++);
				sprites.Add(newSprite);
			}
		}
		
		[ExecuteInEditMode]
		private Sprite SaveSprite(Texture2D texture, int index) {
			var pngData = texture.EncodeToPNG();
			if (pngData != null)
			{
				var path = $"Assets/ItemSprites/{name}/{name}{index}.png";
				
				File.WriteAllBytes(path, pngData);
				AssetDatabase.Refresh();
				
				TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
				if (textureImporter != null)
				{
					textureImporter.textureType = TextureImporterType.Sprite;
					textureImporter.SaveAndReimport();
				}
				return AssetDatabase.LoadAssetAtPath<Sprite>(path);
			}
			return null;
		}
		#endif

		public Sprite GetSprite(Vector2Int cell) => sprites[Cells.IndexOf(cell)];
	}
}