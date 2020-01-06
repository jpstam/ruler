using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Triangle
{
    public Edge e0 { get { return Edges[0]; } }
    public Edge e1 { get { return Edges[1]; } }
    public Edge e2 { get { return Edges[2]; } }

    public Edge[] Edges { get; private set; }

    public Vertex p0 { get { return e0.start; } }
    public Vertex p1 { get { return e1.start; } }
    public Vertex p2 { get { return e2.start; } }

    private Vector2 Circumcenter;
    private float RadiusSquared;

    public Triangle(Edge e0, Edge e1, Edge e2)
    {
        Edges = new Edge[3];

        if(IsCounterClockwise(e0.start, e1.start, e2.start)) {
            Edges[0] = e0;
            Edges[1] = e1;
            Edges[2] = e2;
        } else {
            Edges[0] = e0;
            Edges[1] = e2;
            Edges[2] = e1;
        }

        e0.AddFace(this);
        e1.AddFace(this);
        e2.AddFace(this);

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
            throw new System.Exception();
        }

        var center = new Vector2(aux1 / div, aux2 / div);
        Circumcenter = center;
        RadiusSquared = (center.x - p0.X) * (center.x - p0.X) + (center.y - p0.Y) * (center.y - p0.Y);
    }


    private bool IsCounterClockwise(Vertex point1, Vertex point2, Vertex point3)
    {
        // https://stackoverflow.com/questions/39984709/how-can-i-check-wether-a-point-is-inside-the-circumcircle-of-3-points/44875841#44875841
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

    public override bool Equals(object obj)
    {
        var triangle = obj as Triangle;
        return triangle != null &&
               EqualityComparer<Edge>.Default.Equals(e0, triangle.e0) &&
               EqualityComparer<Edge>.Default.Equals(e1, triangle.e1) &&
               EqualityComparer<Edge>.Default.Equals(e2, triangle.e2);
    }

    public override int GetHashCode()
    {
        var hashCode = 2026591105;
        hashCode = hashCode * -1521134295 + EqualityComparer<Edge>.Default.GetHashCode(e0);
        hashCode = hashCode * -1521134295 + EqualityComparer<Edge>.Default.GetHashCode(e1);
        hashCode = hashCode * -1521134295 + EqualityComparer<Edge>.Default.GetHashCode(e2);
        return hashCode;
    }
}
