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

    public Cutout ReverseY(float height) {
	    return new Cutout(new Vector2(start.x, height - end.y), new Vector2(end.x, height - start.y));
    }
    
    public Cutout ReverseX(float width) {
	    return new Cutout(new Vector2(width - end.x, start.y), new Vector2(width - start.x, end.y));
    }
}