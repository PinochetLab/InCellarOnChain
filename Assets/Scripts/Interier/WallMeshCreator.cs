using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SweepSegment
{
    public float Start { get; set; }
    public float End { get; set; }

    public SweepSegment(float start, float end)
    {
        Start = start;
        End = end;
    }

    public bool Contains(float x) => x > Start && x < End;

    public bool IsOutside(float start, float end) => start >= End || end <= Start;
}

public static class WallMeshMaster
{
    private static void AddQuad(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3, float widthX, float widthY,
        ref List<Vector3> vertices, ref List<Vector3> normals, ref List<int> triangles, ref List<Vector2> uv)
    {
        var vCount = vertices.Count;

        var verticesToAdd = new List<Vector3>() 
            { v0, v1, v2, v0, v2, v3 };

        vertices.AddRange(verticesToAdd);
        normals.AddRange(Enumerable.Repeat(-Vector3.forward, 6));
        triangles.AddRange(new List<int>{ 
            vCount, vCount + 1, vCount + 2, 
            vCount + 3, vCount + 4, vCount + 5 
        });
        //uv.AddRange(verticesToAdd.Select(v => (Vector2)v));
        uv.AddRange(verticesToAdd.Select(v => new Vector2(v.x / widthX, v.y / widthY)));
    }
    
    public static Mesh CreateSimpleMesh(float widthX, float widthY)
    {
        return CreateMesh(widthX, widthY, new List<Cutout>());
    }

    public static Mesh CreateMesh(float widthX, float widthY, List<Cutout> cutouts)
    {
        var mesh = new Mesh();
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var triangles = new List<int>();
        var uv = new List<Vector2>();

        var sweepSegments = new List<SweepSegment>() { new SweepSegment(0, widthY) };

        var cutoutEvents = new List<CutoutEvent>();
        cutouts.ForEach(c => cutoutEvents.AddRange(CutoutEvent.GetEvents(c)));
        
        cutoutEvents.Sort();

        var lastX = 0f;

        foreach (var cutoutEvent in cutoutEvents)
        {
            sweepSegments.ForEach(s => AddSegmentQuad(lastX, cutoutEvent.X, s.Start, s.End));
            if (cutoutEvent.IsStarted)
            {
                var segmentsForAdd = new List<SweepSegment>();
                var segmentsForRemove = new List<SweepSegment>();

                var start = cutoutEvent.StartY;
                var end = cutoutEvent.EndY;

                foreach (var segment in sweepSegments)
                {
                    if (segment.Contains(start) && segment.Contains(end))
                    {
                        segmentsForAdd.Add(new SweepSegment(end, segment.End));
                        segment.End = start;
                    }
                    else if (segment.Contains(start))
                    {
                        segment.End = start;
                    }
                    else if (segment.Contains(end))
                    {
                        segment.Start = end;
                    }
                    else if (!segment.IsOutside(start, end))
                    {
                        segmentsForRemove.Add(segment);
                    }
                }
                sweepSegments.AddRange(segmentsForAdd);
                segmentsForRemove.ForEach(s => sweepSegments.Remove(s));
            }
            else
            {
                sweepSegments.Add(new SweepSegment(cutoutEvent.StartY, cutoutEvent.EndY));
            }

            lastX = cutoutEvent.X;
        }
        
        sweepSegments.ForEach(s => AddSegmentQuad(lastX, widthX, s.Start, s.End));

        mesh.vertices = vertices.ToArray();
        mesh.normals = normals.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        return mesh;

        void AddSegmentQuad(float startX, float endX, float startY, float endY)
        {
            var v0 = new Vector2(startX, startY);
            var v1 = new Vector2(startX, endY);
            var v2 = new Vector2(endX, endY);
            var v3 = new Vector2(endX, startY);
            AddQuad(v0, v1, v2, v3, widthX, widthY, ref vertices, ref normals, ref triangles, ref uv);
        }
    }
}
