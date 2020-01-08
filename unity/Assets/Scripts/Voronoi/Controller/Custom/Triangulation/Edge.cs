using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Edge
{
    public Vertex start { get; private set; }
    public Vertex end { get; private set; }

    public Triangle[] faces { get; private set; }

    public Edge(Vertex start, Vertex end)
    {
        this.start = start;
        this.end = end;
        faces = new Triangle[2];
        start.Edges.Add(this);
        end.Edges.Add(this);
    }

    public void AddFace(Triangle triangle)
    {
        if (faces[0] == null) {
            faces[0] = triangle;
        } else if (faces[1] == null) {
            faces[1] = triangle;
        } else {
            throw new System.InvalidProgramException("Edge has more adjacent faces then physically possible");
        }
    }

    public void RemoveFace(Triangle triangle)
    {
        if(faces[0] == triangle) {
            faces[0] = null;
        } else if(faces[1] == triangle) {
            faces[1] = null;
        }
    }

    public Triangle GetOtherFace(Triangle triangle)
    {
        if(faces[0] == triangle) {
            return faces[1];
        } else if(faces[1] == triangle) {
            return faces[0];
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
