using System;
using System.Collections.Generic;

public class CutoutEvent : IComparable<CutoutEvent>
{
    public float X { get; private set; }
    public float StartY { get; private set; }
    public float EndY { get; private set; }
    
    public bool IsStarted { get; private set; }

    private CutoutEvent(float x, float startY, float endY, bool isStarted)
    {
        X = x;
        StartY = startY;
        EndY = endY;
        IsStarted = isStarted;
    }

    public static List<CutoutEvent> GetEvents(Cutout cutout)
    {
        return new List<CutoutEvent> {
            new CutoutEvent(cutout.Start.x, cutout.Start.y, cutout.End.y, true),
            new CutoutEvent(cutout.End.x, cutout.Start.y, cutout.End.y, false)
        };
    }

    public int CompareTo(CutoutEvent other)
    {
        return X.CompareTo(other.X);
    }
}