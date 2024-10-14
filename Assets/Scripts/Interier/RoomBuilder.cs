using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RoomBuilder))]
public class RoomBuilderInspector : Editor
{

    private string _roomName = string.Empty;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        _roomName = EditorGUILayout.TextField(_roomName);

        if (GUILayout.Button("Build"))
        {
            (target as RoomBuilder)?.BuildRoom(_roomName);
        }
    }
}

public class RoomBuilder : MonoBehaviour
{
    [SerializeField] private Vector3 position;
    [SerializeField] private Vector3 size;

    [SerializeField] private WallInfo floorInfo;
    [SerializeField] private WallInfo ceilInfo;
    [SerializeField] private WallInfo leftWallInfo;
    [SerializeField] private WallInfo backWallInfo;
    [SerializeField] private WallInfo rightWallInfo;


    public void BuildRoom(string roomName)
    {
        var room = new GameObject
        {
            name = roomName
        }.transform;

        var widthX = size.x;
        var widthZ = size.z;
        var height = size.y;
        
        // Floor
        WallCreator.CreateWall(
            floorInfo,
            new Vector3(-widthX / 2, 0, 0),
            new Vector2(widthX, widthZ),
            Quaternion.Euler(90, 0, 0),
            room,
            "Floor"
        );
        
        // Ceil
        WallCreator.CreateWall(
            ceilInfo,
            new Vector3(-widthX / 2, height, widthZ),
            new Vector2(widthX, widthZ),
            Quaternion.Euler(-90, 0, 0),
            room,
            "Ceil"
        );
        
        // Left Wall
        WallCreator.CreateWall(
            leftWallInfo,
            new Vector3(-widthX / 2, 0, 0),
            new Vector2(widthZ, height),
            Quaternion.Euler(0, -90, 0),
            room,
            "Left Wall"
        );
        
        // Back Wall
        WallCreator.CreateWall(
            backWallInfo,
            new Vector3(-widthX / 2, 0, widthZ),
            new Vector2(widthX, height),
            Quaternion.identity,
            room,
            "Back Wall"
        );
        
        // Right Wall
        WallCreator.CreateWall(
            rightWallInfo,
            new Vector3(widthX / 2, 0, widthZ),
            new Vector2(widthZ, height),
            Quaternion.Euler(0, 90, 0),
            room,
            "Right Wall"
        );

        /*var roomScript = room.AddComponent<Room>();
        roomScript.SetUp(size);
        roomScript.transform.position = position;*/
    }
}
