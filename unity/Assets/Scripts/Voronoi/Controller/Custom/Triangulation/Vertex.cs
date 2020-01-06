﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Vertex
{
    public Vector2 point { get; private set; }
    public float X { get { return point.x; } }
    public float Y { get { return point.y; } }

    public Vertex(Vector2 point)
    {
        this.point = point;
    }

    public Vertex(float x, float y)
    {
        point = new Vector2(x, y);
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