using UnityEngine;

public static class WallCreator
{
    public static GameObject CreateWall(
        WallInfo wallInfo,
        Vector3 position,
        Vector2 scale,
        Quaternion rotation,
        Transform parent = null,
        string name = "")
    {
        var mesh = WallMeshMaster.CreateMesh(scale.x, scale.y, wallInfo.Cutouts);
        var go = new GameObject
        {
            name = (parent ? parent.name : "") + " : " + name
        };
        go.AddComponent<MeshFilter>().sharedMesh = mesh;
        go.AddComponent<MeshCollider>().sharedMesh = mesh;
        var meshRenderer = go.AddComponent<MeshRenderer>();
        meshRenderer.material = wallInfo.Material;
        go.transform.rotation = rotation;
        go.transform.position = position;
        go.transform.parent = parent;
        return go;
    }
}
