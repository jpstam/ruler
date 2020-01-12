using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConvexHullSorter : IComparer<Vertex>
{
    private readonly Vertex point;

    public ConvexHullSorter(Vertex point)
    {
        this.point = point;
    }

    int IComparer<Vertex>.Compare(Vertex a, Vertex b)
    {
        if (a == b) {
            return 0;
        }
        var aDegree = (Mathf.Atan2(a.X - point.X, a.Y - point.Y) * Mathf.Rad2Deg + 360) % 360 ;
        var bDegree = (Mathf.Atan2(b.X - point.X, b.Y - point.Y) * Mathf.Rad2Deg + 360) % 360;
        return aDegree > bDegree ? 1 : -1;
    }
}

public class Face
{

    public bool OwnedByPlayer1 { get; set; }

    public Vertex Point { get; private set; }

    public SortedList<Vertex, Edge> Edges { get; private set; }

    public IList<Vertex> Vertices { get { return Edges.Keys; } }

    public bool IsComplete { get; private set; }
    public Edge Start { get; private set; }
    public Edge End { get; private set; }

    public Face(Vertex point)
    {
        this.Point = point;
        this.Edges = new SortedList<Vertex, Edge>(new ConvexHullSorter(Point));
    }
    
    public void Add(Edge edge)
    {
        var adjusted = GetAdjustedStart(edge);
        if (adjusted != null && !Edges.ContainsKey(adjusted)) {
            Edges.Add(GetAdjustedStart(edge), edge);
            edge.SetFace(this);
            FindHole();
        } else {
            //Debug.Log(Point.ToString() + " : " + edge.ToString());
        }
    }

    public void Remove(Edge edge)
    {
        Edges.Remove(GetAdjustedStart(edge));
    }

    public Vertex GetAdjustedStart(Edge edge)
    {
        if (edge.right == this) {
            return edge.start;
        } else if (edge.left == this) {
            return edge.end;
        } else {
            return null;
        }
    }

    public Vertex GetAdjustedEnd(Edge edge)
    {
        if(edge.right == this) {
            return edge.end;
        } else if(edge.left == this) {
            return edge.start;
        } else {
            return null;
        }
    }

    public float ComputeArea()
    {
        float area = 0;

        Debug.Log("edges: " + this.Edges.Count);

        area += Random.Range(0f, 100f);

        return area;
    }

    private void FindHole()
    {
        var prev = Edges.Last();
        foreach(KeyValuePair<Vertex, Edge> next in Edges) {
            if(GetAdjustedEnd(prev.Value) != next.Key) {
                this.End = prev.Value;
                this.Start = next.Value;
                this.IsComplete = false;
                return;
            } else {
                prev = next;
            }
        }
        this.End = null;
        this.Start = null;
        this.IsComplete = true;
    }
}
