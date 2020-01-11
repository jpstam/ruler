using System;
using System.Collections;
using System.Collections.Generic;

public class Edge
{
    public Vertex start { get; private set; }
    public Vertex end { get; private set; }

    public Face left;
    public Face right;

    public Edge(Vertex start, Vertex end)
    {
        this.start = start;
        this.end = end;
        start.Edges.Add(this);
        end.Edges.Add(this);
    }

    public void SetFace(Face face)
    {
        // From: https://stackoverflow.com/a/3461533 
        var dotProduct = (end.X - start.X) * (face.Point.Y - start.Y) - (end.Y - start.Y) * (face.Point.X - start.X);
        if(dotProduct > 0) {
            left = face;
        } else {
            right = face;
        }
    }

    public Face GetOther(Face face)
    {
        if(face == left) {
            return right;
        } else if(face == right) {
            return left;
        } else {
            return null;
        }
    }

    public Vertex GetOther(Vertex vertex)
    {
        if (vertex == start) {
            return end;
        } else if (vertex == end) {
            return start;
        } else {
            return null;
        }
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
