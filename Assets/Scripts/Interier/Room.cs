#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using Cameras;
using UnityEngine;

#if UNITY_EDITOR
[CustomEditor(typeof(Room))]
public class RoomInspector : Editor
{
	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		if (GUILayout.Button("Build"))
		{
			(target as Room)?.BuildRoom();
		}
	}
}
#endif

public class Room : MonoBehaviour
{
    [SerializeField] private Vector3 size;

    [SerializeField] private WallInfo floorInfo;
    [SerializeField] private WallInfo ceilInfo;
    [SerializeField] private WallInfo leftWallInfo;
    [SerializeField] private WallInfo backWallInfo;
    [SerializeField] private WallInfo rightWallInfo;
    
    public Vector3 Position => transform.position;
    public Vector3 Size => size;

    private void RemoveChildren() {
	    while ( transform.childCount > 0 ) {
		    DestroyImmediate( transform.GetChild(0).gameObject );
	    }
    }
    
    [ContextMenu("Build")]
    public void BuildRoom()
    {
	    RemoveChildren();
	    
        var widthX = size.x;
        var widthZ = size.z;
        var height = size.y;
        
        var pos = transform.position;
        
        // Floor
        WallCreator.CreateWall(
            floorInfo,
            pos + new Vector3(-widthX / 2, 0, 0),
            new Vector2(widthX, widthZ),
            Quaternion.Euler(90, 0, 0),
            transform,
            "Floor"
        );
        
        var ceilCutouts = ceilInfo.Cutouts;
        ceilCutouts = ceilCutouts.Select(cutout => cutout.ReverseY(size.z)).ToList();
        
        // Ceil
        WallCreator.CreateWall(
            new WallInfo(ceilInfo.Material, ceilCutouts),
            pos + new Vector3(-widthX / 2, height, widthZ),
            new Vector2(widthX, widthZ),
            Quaternion.Euler(-90, 0, 0),
            transform,
            "Ceil"
        );
        
        // Left Wall
        WallCreator.CreateWall(
            leftWallInfo,
            pos + new Vector3(-widthX / 2, 0, 0),
            new Vector2(widthZ, height),
            Quaternion.Euler(0, -90, 0),
            transform,
            "Left Wall"
        );
        
        // Back Wall
        WallCreator.CreateWall(
            backWallInfo,
            pos + new Vector3(-widthX / 2, 0, widthZ),
            new Vector2(widthX, height),
            Quaternion.identity,
            transform,
            "Back Wall"
        );
        
        var rightWallCutouts = rightWallInfo.Cutouts;
        rightWallCutouts = rightWallCutouts.Select(cutout => cutout.ReverseX(size.z)).ToList();
        
        // Right Wall
        WallCreator.CreateWall(
            new WallInfo(rightWallInfo.Material, rightWallCutouts),
            pos + new Vector3(widthX / 2, 0, widthZ),
            new Vector2(widthZ, height),
            Quaternion.Euler(0, 90, 0),
            transform,
            "Right Wall"
        );
    }

    private void DrawWall(WallInfo wallInfo, float width, float height, Vector3 position, Quaternion rotation) {
	    var points = new List<Vector3>();
	    points.Add(Vector3.zero);
	    points.Add(Vector3.up * height);
	    points.Add(Vector3.up * height + Vector3.right * width);
	    points.Add(Vector3.right * width);
	    foreach ( var cutout in wallInfo.Cutouts ) {
		    points.Add(cutout.Start);
		    points.Add(new Vector2(cutout.Start.x, cutout.End.y));
		    points.Add(cutout.End);
		    points.Add(new Vector2(cutout.End.x, cutout.Start.y));
	    }
	    points = points.Select(x => rotation * x + position).ToList();
	    for ( var i = 0; i < points.Count; i += 4 ) {
		    Gizmos.DrawLineStrip(new ReadOnlySpan<Vector3>(points.Skip(i).Take(4).ToArray()), true);
	    }
    }

    private void OnDrawGizmos() {
	    Gizmos.color = Color.green;
	    var s  = size * 0.999f;
	    var pos = transform.position;
	    DrawWall(floorInfo, s.x, s.z, pos + new Vector3(-s.x / 2, 0, 0), Quaternion.Euler(90, 0, 0));
	    DrawWall(leftWallInfo, s.z, s.y, pos + new Vector3(-s.x / 2, 0, 0), Quaternion.Euler(0, -90, 0));
	    DrawWall(backWallInfo, s.x, s.y, pos + new Vector3(-s.x / 2, 0, s.z), Quaternion.Euler(0, 0, 0));
	    DrawWall(rightWallInfo, s.z, s.y, pos + new Vector3(s.x / 2, 0, 0), Quaternion.Euler(0, -90, 0));
	    DrawWall(ceilInfo, s.x, s.z, pos + new Vector3(-s.x / 2, s.y, 0), Quaternion.Euler(90, 0, 0));
    }

    private void OnTriggerEnter(Collider other) {
	    var camera = CameraMover.Camera;
	    CameraMover.MoveTo(CalculateCameraPosition(camera.fieldOfView, camera.aspect));
    }

    private Vector3 CalculateCameraPosition(float fov, float aspectRatio) {
	    var x = Position.x;
	    var y = Position.y + Size.y / 2;
	    var z = Position.z;
	    var deltaZ1 = Size.y / 2 / Mathf.Tan(fov * Mathf.Deg2Rad / 2);
	    var deltaZ2 = Size.y / 2 / Mathf.Tan(fov * aspectRatio * Mathf.Deg2Rad / 2);
	    var delta = Mathf.Max(deltaZ1, deltaZ2);
	    return new Vector3(x, y, z - delta);
    }
}
