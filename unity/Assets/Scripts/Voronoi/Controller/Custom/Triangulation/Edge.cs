using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge
{
    public Vertex start { get; private set; }
    public Vertex end { get; private set; }

    public int faces { get; private set; }

    public Edge(Vertex start, Vertex end)
    {
        this.start = start;
        this.end = end;
        faces = 0;
    }

    public void AddFace(Triangle triangle)
    {
        faces++;
    }

    public void RemoveFace(Triangle triangle)
    {
        faces--;
    }

    public override bool Equals(object obj)
    {
        var edge = obj as Edge;
        return edge != null &&
               ((EqualityComparer<Vertex>.Default.Equals(start, edge.start) &&
               EqualityComparer<Vertex>.Default.Equals(end, edge.end)) || (EqualityComparer<Vertex>.Default.Equals(start, edge.end) &&
               EqualityComparer<Vertex>.Default.Equals(end, edge.start)));
    }

    public override int GetHashCode()
    {
        var smallest = start.X < end.X ? start : end;
        var largest = start.X < end.X ? end : start;

        var hashCode = -731044709;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vertex>.Default.GetHashCode(smallest);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vertex>.Default.GetHashCode(largest);
        return hashCode;
    }

    public override string ToString()
    {
        return "[ " + start.ToString() + ", " + end.ToString() + " ]";
    }
}
