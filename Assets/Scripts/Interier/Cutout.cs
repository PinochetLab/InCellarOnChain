using UnityEngine;

[System.Serializable]
public class Cutout
{
    [SerializeField] private Vector2 start;
    [SerializeField] private Vector2 end;

    public Cutout(Vector2 start, Vector2 end)
    {
        this.start = start;
        this.end = end;
    }

    public Vector2 Start => start;
    public Vector2 End => end;
}