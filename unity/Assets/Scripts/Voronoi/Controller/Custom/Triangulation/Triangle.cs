using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class Triangle
{
    public Vertex p0 { get { return Vertices[0]; } }
    public Vertex p1 { get { return Vertices[1]; } }
    public Vertex p2 { get { return Vertices[2]; } }

    public Vertex[] Vertices { get; private set; }

    public Vector2 Circumcenter { get; private set; }
    private float RadiusSquared;

    public bool Boundary { get { return p0.Boundary || p1.Boundary || p2.Boundary; } }

    public Triangle(Vertex p0, Vertex p1, Vertex p2)
    {
        Vertices = new Vertex[3];

        if(IsCounterClockwise(p0, p1, p2)) {
            Vertices[0] = p0;
            Vertices[1] = p1;
            Vertices[2] = p2;
        } else {
            Vertices[0] = p0;
            Vertices[1] = p2;
            Vertices[2] = p1;
        }

        p0.Faces.Add(this);
        p1.Faces.Add(this);
        p2.Faces.Add(this);

        CalculateCircumcircle();
    }

    private void CalculateCircumcircle()
    {
        // https://codefound.wordpress.com/2013/02/21/how-to-compute-a-circumcircle/#more-58
        // https://en.wikipedia.org/wiki/Circumscribed_circle
        var dA = p0.X * p0.X + p0.Y * p0.Y;
        var dB = p1.X * p1.X + p1.Y * p1.Y;
        var dC = p2.X * p2.X + p2.Y * p2.Y;

        var aux1 = (dA * (p2.Y - p1.Y) + dB * (p0.Y - p2.Y) + dC * (p1.Y - p0.Y));
        var aux2 = -(dA * (p2.X - p1.X) + dB * (p0.X - p2.X) + dC * (p1.X - p0.X));
        var div = (2 * (p0.X * (p2.Y - p1.Y) + p1.X * (p0.Y - p2.Y) + p2.X * (p1.Y - p0.Y)));

        if(div == 0) {
            div += float.Epsilon;
        }

        var center = new Vector2(aux1 / div, aux2 / div);
        Circumcenter = center;
        RadiusSquared = (center.x - p0.X) * (center.x - p0.X) + (center.y - p0.Y) * (center.y - p0.Y);
    }


    private bool IsCounterClockwise(Vertex point1, Vertex point2, Vertex point3)
    {
        // https://stackoverflow.com/a/44875841
        var result = (point2.X - point1.X) * (point3.Y - point1.Y) -
            (point3.X - point1.X) * (point2.Y - point1.Y);
        return result > 0;
    }

    public bool IsPointInsideCircumcircle(Vector2 point)
    {
        var d_squared = (point.x - Circumcenter.x) * (point.x - Circumcenter.x) +
            (point.y - Circumcenter.y) * (point.y - Circumcenter.y);
        return d_squared < RadiusSquared;
    }

    public bool SharesEdgeWith(Triangle triangle)
    {
        var sharedVertices = Vertices.Where(o => triangle.Vertices.Contains(o)).Count();
        return sharedVertices == 2;
    }

    public IEnumerable<Vertex> GetSharedVerticesWith(Triangle triangle)
    {
        return Vertices.Where(o => triangle.Vertices.Contains(o));

    }

    public override bool Equals(object obj)
    {
        var triangle = obj as Triangle;
        return triangle != null && Vertices.All(vertex => triangle.Vertices.Contains(vertex));
    }

    public override int GetHashCode()
    {
        var hashCode = 2026591105;
        hashCode = hashCode * -1521134295 + EqualityComparer<Vertex>.Default.GetHashCode(p0);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vertex>.Default.GetHashCode(p1);
        hashCode = hashCode * -1521134295 + EqualityComparer<Vertex>.Default.GetHashCode(p2);
        return hashCode;
    }
}
