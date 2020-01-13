using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public Boolean TryFindIntersectionPoint(Vector2 otherStart, Vector2 otherEnd, out Vector2 intersection)
    {
        var denominator = (otherEnd.x - otherStart.x) * (start.Y - end.Y) - (start.X - end.X) * (otherEnd.y - otherStart.y) ;
        if (denominator == 0) {
            intersection = new Vector2();
            return false;
        } else {
            var anum = (otherStart.y - otherEnd.y) * (start.X - otherStart.x) + (otherEnd.x - otherStart.x) * (start.Y - otherStart.y);
            var bnum = (start.Y - end.Y) * (start.X - otherStart.x) + (end.X - start.X) * (start.Y - otherStart.y);
            var a = anum / denominator;
            var b = bnum / denominator;
            if (a < 0 || a > 1 || b < 0 || b > 1) {
                intersection = new Vector2();
                return false;
            }
            var x = start.X + a * (end.X - start.X);
            var y = start.Y + a * (end.Y - start.Y);

            intersection = new Vector2(x, y);
            return true;
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
