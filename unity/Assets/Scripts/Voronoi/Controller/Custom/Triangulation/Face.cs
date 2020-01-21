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
        if(a == b) {
            return 0;
        }
        var aDegree = (Mathf.Atan2(a.X - point.X, a.Y - point.Y) * Mathf.Rad2Deg + 360) % 360;
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

    public SortedList<Vertex, Vertex> CutPolygon { get; private set; }

    public Face(Vertex point)
    {
        this.Point = point;
        this.Edges = new SortedList<Vertex, Edge>(new ConvexHullSorter(Point));
    }

    public void Add(Edge edge)
    {
        if(edge != null) {
            edge.SetFace(this);
            var adjusted = GetAdjustedStart(edge);
            if(adjusted != null && !Edges.ContainsKey(adjusted)) {
                Edges.Add(GetAdjustedStart(edge), edge);

                FindHole();
            }
        }
    }

    public void Remove(Edge edge)
    {
        Edges.Remove(GetAdjustedStart(edge));
    }

    public Vertex GetAdjustedStart(Edge edge)
    {
        if(edge == null) {
            return null;
        }
        if(edge.right == this) {
            return edge.start;
        } else if(edge.left == this) {
            return edge.end;
        } else {
            return null;
        }
    }

    public Vertex GetAdjustedEnd(Edge edge)
    {
        if(edge == null) {
            return null;
        }
        if(edge.right == this) {
            return edge.end;
        } else if(edge.left == this) {
            return edge.start;
        } else {
            return null;
        }
    }

    public bool ContainsPoint(Vector2 p)
    {
        if(Edges.Count < 1) return false;

        //Not used Source: https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html

        //var c = false;
        foreach(Edge e in Edges.Values) {
            var start = GetAdjustedStart(e);
            var end = GetAdjustedEnd(e);
            //if(((end.Y > p.y) != (start.Y > p.y)) &&
            //    (p.x < (start.X - end.X) * (p.y - end.Y) / (start.Y - end.Y) + end.X))
            //    c = !c;
            var d = (end.X - start.X) * (p.y - start.Y) - (end.Y - start.Y) * (p.x - start.X);
            if(d > 0) {
                return false;
            }
        }

        //if (!IsComplete) {
        //    var start = GetAdjustedEnd(End);
        //    var end = GetAdjustedStart(Start);
        //    if(((end.Y > p.y) != (start.Y > p.y)) &&
        //        (p.x < (start.X - end.X) * (p.y - end.Y) / (start.Y - end.Y) + end.X))
        //        c = !c;
        //}

        return true;
    }

    public void CalculateCutPolygon(Vector2 bottomLeft, Vector2 topRight)
    {
        var PartialCutPolygon = new SortedList<Vertex, Vertex>(Edges.Count + 10, new ConvexHullSorter(Point));


        foreach(KeyValuePair<Vertex, Edge> next in Edges) {
            var startInside = IsPointInRectangle(next.Value.start.point, bottomLeft, topRight);
            var endInside = IsPointInRectangle(next.Value.end.point, bottomLeft, topRight);
            if(startInside && endInside) {
                var vertex = GetAdjustedStart(next.Value);
                if(vertex != null) {
                    PartialCutPolygon.Add(vertex, vertex);
                }
            } else {
                if(IsPointInRectangle(GetAdjustedStart(next.Value).point, bottomLeft, topRight)) {
                    var vertex = GetAdjustedStart(next.Value);
                    PartialCutPolygon.Add(vertex, vertex);
                }

                Vector2 intersection;
                if(next.Value.TryFindIntersectionPoint(bottomLeft, new Vector2(bottomLeft.x, topRight.y), out intersection)) {
                    var vertex = new Vertex(intersection);
                    PartialCutPolygon.Add(vertex, vertex);
                }
                if(next.Value.TryFindIntersectionPoint(new Vector2(bottomLeft.x, topRight.y), topRight, out intersection)) {
                    var vertex = new Vertex(intersection);
                    PartialCutPolygon.Add(vertex, vertex);
                }
                if(next.Value.TryFindIntersectionPoint(topRight, new Vector2(topRight.x, bottomLeft.y), out intersection)) {
                    var vertex = new Vertex(intersection);
                    PartialCutPolygon.Add(vertex, vertex);
                }
                if(next.Value.TryFindIntersectionPoint(new Vector2(topRight.x, bottomLeft.y), bottomLeft, out intersection)) {
                    var vertex = new Vertex(intersection);
                    PartialCutPolygon.Add(vertex, vertex);
                }
            }
        }


        //Adding Corners
        if(PartialCutPolygon.Count > 1) {
            CutPolygon = new SortedList<Vertex, Vertex>(Edges.Count + 10, new ConvexHullSorter(Point));
            var prev = PartialCutPolygon.Last().Value;
            foreach(Vertex vertex in PartialCutPolygon.Values) {
                CutPolygon.Add(vertex, vertex);

                if(IsPointOnEdge(prev, bottomLeft, topRight)
                    && IsPointOnEdge(vertex, bottomLeft, topRight)) {

                    // Intersections could be with different edges
                    var left = Mathf.Approximately(bottomLeft.x, prev.X) || Mathf.Approximately(bottomLeft.x, vertex.X);
                    var right = Mathf.Approximately(topRight.x, prev.X) || Mathf.Approximately(topRight.x, vertex.X);
                    var top = Mathf.Approximately(topRight.y, prev.Y) || Mathf.Approximately(topRight.y, vertex.Y);
                    var bottom = Mathf.Approximately(bottomLeft.y, prev.Y) || Mathf.Approximately(bottomLeft.y, vertex.Y);
                     if((left || right) && (bottom || top)) {
                        var x = left ? bottomLeft.x : topRight.x;
                        var y = bottom ? bottomLeft.y : topRight.y;
                        var extra = new Vertex(new Vector2(x, y));
                        CutPolygon.Add(extra, extra);
                    } else if(left && right) {
                        if(this.ContainsPoint(topRight)) {
                            var eTopLeft = new Vertex(new Vector2(bottomLeft.x, topRight.y));
                            CutPolygon.Add(eTopLeft, eTopLeft);
                            var eTopRight = new Vertex(new Vector2(topRight.x, topRight.y));
                            CutPolygon.Add(eTopRight, eTopRight);
                        } else if(this.ContainsPoint(bottomLeft)) {
                            var eBottomLeft = new Vertex(new Vector2(bottomLeft.x, bottomLeft.y));
                            CutPolygon.Add(eBottomLeft, eBottomLeft);
                            var eBottomRight = new Vertex(new Vector2(topRight.x, bottomLeft.y));
                            CutPolygon.Add(eBottomRight, eBottomRight);
                        }
                    } else if(top && bottom) {
                        if(this.ContainsPoint(bottomLeft)) {
                            var eTopLeft = new Vertex(new Vector2(bottomLeft.x, topRight.y));
                            CutPolygon.Add(eTopLeft, eTopLeft);
                            var eBottomLeft = new Vertex(new Vector2(bottomLeft.x, bottomLeft.y));
                            CutPolygon.Add(eBottomLeft, eBottomLeft);
                        } else if(this.ContainsPoint(topRight)) {
                            var eTopRight = new Vertex(new Vector2(topRight.x, topRight.y));
                            CutPolygon.Add(eTopRight, eTopRight);
                            var eBottomRight = new Vertex(new Vector2(topRight.x, bottomLeft.y));
                            CutPolygon.Add(eBottomRight, eBottomRight);
                        }
                    }
                }
                prev = vertex;
            }
        } else {
            CutPolygon = new SortedList<Vertex, Vertex>(1, new ConvexHullSorter(Point));
        }


    }

    public float ComputeArea()
    {
        float area = 0;

        if(this.CutPolygon == null) {
            return area;
        }
        if(this.CutPolygon.Count < 2) {
            return area;
        }

        for(int i = 0; i < this.CutPolygon.Count; i++) {
            Vertex v1 = this.CutPolygon.Keys[i];
            Vertex v2 = this.CutPolygon.Keys[(i + 1) % this.CutPolygon.Count];
            area += v1.X * v2.Y - v1.Y * v2.X;
        }

        return Mathf.Abs(area / 2f);
    }

    public float ComputeCircumference()
    {
        float circumference = 0;

        if(this.CutPolygon == null) {
            return circumference;
        }
        if(this.CutPolygon.Count < 2) {
            return circumference;
        }

        for(int i = 0; i < this.CutPolygon.Count; i++) {
            Vertex v1 = this.CutPolygon.Keys[i];
            Vertex v2 = this.CutPolygon.Keys[(i + 1) % this.CutPolygon.Count];
            circumference += Mathf.Sqrt(Mathf.Pow(v1.X - v2.X, 2) + Mathf.Pow(v1.Y - v2.Y, 2));
        }

        return circumference;
    }

    private bool IsPointOnEdge(Vertex vertex, Vector2 bottomLeft, Vector2 topRight)
    {
        return Mathf.Approximately(bottomLeft.x, vertex.X)
            || Mathf.Approximately(topRight.x, vertex.X)
            || Mathf.Approximately(topRight.y, vertex.Y)
            || Mathf.Approximately(bottomLeft.y, vertex.Y);
    }

    private bool IsPointInRectangle(Vector2 point, Vector2 bottomLeft, Vector2 topRight)
    {
        return point.x >= bottomLeft.x && point.x <= topRight.x && point.y >= bottomLeft.y && point.y <= topRight.y;
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
