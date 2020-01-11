using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertex
{
    public Vector2 point { get; private set; }
    public float X { get { return point.x; } }
    public float Y { get { return point.y; } }

    public bool Boundary = false;

    public List<Triangle> Faces { get; private set; }
    public HashSet<Edge> Edges { get; private set; }

    public Vertex(Vector2 point, bool isBoundary = false)
    {
        this.Boundary = isBoundary;
        this.point = point;
        this.Faces = new List<Triangle>();
        this.Edges = new HashSet<Edge>();
    }

    public Vertex(float x, float y, bool isBoundary = false)
    {
        this.Boundary = isBoundary;
        point = new Vector2(x, y); 
        this.Faces = new List<Triangle>();
        this.Edges = new HashSet<Edge>();
    }

    public Vertex Copy()
    {
        return new Vertex(X, Y, Boundary);
    }

    public override bool Equals(object obj)
    {
        var vertex = obj as Vertex;
        return vertex != null &&
               point.Equals(vertex.point);
    }

    public override int GetHashCode()
    {
        return 1595967545 + EqualityComparer<Vector2>.Default.GetHashCode(point);
    }

    public override string ToString()
    {
        return point.ToString();
    }
}
