using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector3 Size { get; private set; }

    public Vector3 Position => transform.position;

    public void SetUp(Vector3 roomSize)
    {
        Size = roomSize;
    }
}
