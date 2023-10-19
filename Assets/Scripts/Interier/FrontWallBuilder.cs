using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FrontWallBuilder))]
public class FrontWallBuilderCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Build/Rebuild"))
        {
            (target as FrontWallBuilder)?.Build();
        }
    }
}

public class FrontWallBuilder : MonoBehaviour
{
    [SerializeField] private float edgeWidth = 10;
    [SerializeField] private Material material;

    private void RemoveChildren()
    {
        while (transform.childCount > 0)
        {
            DestroyImmediate(transform.GetChild(0).gameObject);
        }
    }
    
    public void Build()
    {
        RemoveChildren();
        
        var rooms = FindObjectsOfType<Room>().ToList();

        var minX = rooms.Min(room => room.Position.x - room.Size.x / 2) - edgeWidth;
        var maxX = rooms.Max(room => room.Position.x + room.Size.x / 2) + edgeWidth;
        var minY = rooms.Min(room => room.Position.y) - edgeWidth;
        var maxY = rooms.Max(room => room.Position.y + room.Size.y) + edgeWidth;

        var offset = new Vector2(minX, minY);
        var scale = new Vector2(maxX - minX, maxY - minY);

        var cutouts = rooms.Select(room => new Cutout(
            new Vector2(room.Position.x - room.Size.x / 2, room.Position.y) - offset,
            new Vector2(room.Position.x + room.Size.x / 2, room.Position.y + room.Size.y) - offset
        )).ToList();
        
        var wall = WallCreator.CreateWall(
            new WallInfo(cutouts),
            offset,
            scale,
            Quaternion.identity,
            transform,
            "Front Wall"
        );

        wall.GetComponent<MeshRenderer>().material = material;
    }
}
