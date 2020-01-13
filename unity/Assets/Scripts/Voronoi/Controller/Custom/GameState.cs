using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameState
{
    // TEMP: keep track of the order of points added (for copy of gamestate)
    private List<Vector2> pointOrder = new List<Vector2>();


    public Vector2 BottomLeft { get; private set; }
    public Vector2 TopRight { get; private set; }

    private Triangulation Delauney;
    public Graph Voronoi { get; private set; }

    private HashSet<Edge> polygon = new HashSet<Edge>();
    private HashSet<Triangle> newTriangles = new HashSet<Triangle>();
    private HashSet<Edge> newVoronoiEdges = new HashSet<Edge>();

    public GameState(Vector2 bottomLeft, Vector2 topRight)
    {
        this.BottomLeft = bottomLeft;
        this.TopRight = topRight;
        Debug.Log(BottomLeft + " " + topRight);
        // Initialize Data Structures
        Delauney = new Triangulation();
        Voronoi = new Graph();

        // Set up Border triangle 
        float dx = topRight.x - bottomLeft.x;
        float dy = topRight.y - bottomLeft.y;

        var bLeft = new Vertex(dx * -30, dy * -30);
        var bRight = new Vertex(dx * 30, dy * -30);
        var top = new Vertex(0, dy * 30);

        bLeft.Boundary = true;
        bRight.Boundary = true;
        top.Boundary = true;

        var triangle = new Triangle(bLeft, top, bRight);
        Delauney.Add(bLeft, bRight, top);
        Delauney.Add(triangle);
    }

    private GameState(GameState original)
    {
        this.BottomLeft = new Vector2(original.BottomLeft.x, original.BottomLeft.y);
        this.TopRight = new Vector2(original.TopRight.x, original.TopRight.y);
        this.Delauney = original.Delauney.Copy();
        this.Voronoi = original.Voronoi.Copy();

        foreach(Face face in this.Voronoi.Faces.Values) {
            face.CalculateCutPolygon(BottomLeft, TopRight);
        }
    }


    public void AddPoint(Vector2 point, bool playerOne)
    {
        // TEMP: Add points to the point order (for copy of gamestate)
        pointOrder.Add(new Vector2(point.x, point.y));


        var badTriangles = FindBadTriangles(point);
        var polygon = FindHoleBoundary(badTriangles);
        this.polygon = polygon;

        foreach(Triangle triangle in badTriangles) {
            Delauney.Remove(triangle);
        }

        newTriangles = new HashSet<Triangle>();

        var vertex = new Vertex(point);
        Delauney.Add(vertex);
        foreach(Edge edge in polygon) {
            var triangle = new Triangle(edge.start, edge.end, vertex);
            Delauney.Add(triangle);
            newTriangles.Add(triangle);
        }

        AdjustVoronoi(vertex, badTriangles, newTriangles, playerOne);
    }

    private HashSet<Triangle> FindBadTriangles(Vector2 point)
    {
        var badTriangles = Delauney.Triangles.Where(triangle => triangle.IsPointInsideCircumcircle(point));
        return new HashSet<Triangle>(badTriangles);
    }

    private HashSet<Edge> FindHoleBoundary(HashSet<Triangle> badTriangles)
    {
        var allEdges = new List<Edge>();
        foreach(Triangle triangle in badTriangles) {
            allEdges.Add(new Edge(triangle.p0, triangle.p1));
            allEdges.Add(new Edge(triangle.p1, triangle.p2));
            allEdges.Add(new Edge(triangle.p2, triangle.p0));
        }
        var polygon = allEdges.GroupBy(edge => edge).Where(group => group.Count() == 1).Select(group => group.First());
        return new HashSet<Edge>(polygon);
    }

    private void AdjustVoronoi(Vertex vertex, HashSet<Triangle> badTriangles, HashSet<Triangle> newTriangles, bool playerOne)
    {
        newVoronoiEdges = new HashSet<Edge>();
        var affectedVoronoiFaces = new HashSet<Face>();
        foreach(Triangle triangle in badTriangles) {
            Voronoi.Remove(triangle.Circumcenter);
        }

        var newFace = new Face(vertex);
        newFace.OwnedByPlayer1 = playerOne;
        Voronoi.Faces.Add(vertex, newFace);

        foreach(Triangle triangle in newTriangles) {
            if(!triangle.Boundary) {
                var start = Voronoi.Add(new Vertex(triangle.Circumcenter));

                var neighbors = new HashSet<Triangle>();
                foreach(var corner in triangle.Vertices) {
                    var trianglesWithSharedEdge = corner.Faces.Where(f => {
                        return f != triangle && triangle.SharesEdgeWith(f);
                    });
                    neighbors.UnionWith(trianglesWithSharedEdge);
                }

                foreach(Triangle n in neighbors) {
                    var end = Voronoi.Add(new Vertex(n.Circumcenter));
                    var edge = Voronoi.Add(new Edge(start, end));

                    var points = triangle.GetSharedVerticesWith(n); // Always returns 2 points
                    foreach(Vertex p in points) {
                        Face face;
                        Voronoi.Faces.TryGetValue(p, out face);
                        edge.SetFace(face);
                        face.Add(edge);
                        affectedVoronoiFaces.Add(face);
                    }

                    newVoronoiEdges.Add(edge);
                }
            }
        }

        foreach(Face face in affectedVoronoiFaces) {
            face.CalculateCutPolygon(BottomLeft, TopRight);
        }
    }

    public GameState Copy()
    {
        return new GameState(this);
    }

    public void DebugDraw(float y, bool borders, bool delauney, bool delauneyDebug, bool voronoi, bool voronoiDebug)
    {
        if(borders) {
            Debug.DrawLine(new Vector3(BottomLeft.x, y, BottomLeft.y), new Vector3(BottomLeft.x, y, TopRight.y), Color.white, 0);
            Debug.DrawLine(new Vector3(BottomLeft.x, y, TopRight.y), new Vector3(TopRight.x, y, TopRight.y), Color.white, 0);
            Debug.DrawLine(new Vector3(TopRight.x, y, TopRight.y), new Vector3(TopRight.x, y, BottomLeft.y), Color.white, 0);
            Debug.DrawLine(new Vector3(TopRight.x, y, BottomLeft.y), new Vector3(BottomLeft.x, y, BottomLeft.y), Color.white, 0);
        }


        if(delauney) {
            Delauney.DebugDraw(y);
        }

        if(delauneyDebug) {
            foreach(Triangle t in newTriangles) {
                Debug.DrawLine(new Vector3(t.p0.X, y, t.p0.Y), new Vector3(t.p1.X, y, t.p1.Y), Color.red, 0);
                Debug.DrawLine(new Vector3(t.p1.X, y, t.p1.Y), new Vector3(t.p2.X, y, t.p2.Y), Color.red, 0);
                Debug.DrawLine(new Vector3(t.p2.X, y, t.p2.Y), new Vector3(t.p0.X, y, t.p0.Y), Color.red, 0);
            }

            foreach(Edge e in polygon) {
                Debug.DrawLine(new Vector3(e.start.X, y, e.start.Y), new Vector3(e.end.X, y, e.end.Y), Color.green, 0);
            }
        }

        if(voronoi) {
            Voronoi.DebugDraw(y);
        }

        if(voronoiDebug) {
            foreach(Edge e in newVoronoiEdges) {
                Debug.DrawLine(new Vector3(e.start.X, y, e.start.Y), new Vector3(e.end.X, y, e.end.Y), Color.cyan, 0);
            }

            //foreach(Face face in Voronoi.faces.Values) {
            //    foreach(Edge e in face.Edges.Values) {
            //        Debug.DrawLine(new Vector3(e.start.X, y, e.start.Y), new Vector3(e.end.X, y, e.end.Y), Color.magenta, 0);
            //    }
            //}

            // Draw Lines to Voronoi Points;
            //foreach(Edge e in Voronoi.Edges.Keys) {
            //    var middle = new Vector3((e.start.X + e.end.X) / 2, y, (e.start.Y + e.end.Y) / 2);
            //    var left = new Vector3(e.left.Point.X, y, e.left.Point.Y);
            //    var right = new Vector3(e.right.Point.X, y, e.right.Point.Y);
            //    Debug.DrawLine(middle, left, Color.green, 0);
            //    Debug.DrawLine(middle, right, Color.red, 0);
            //}

            // Draw Lines to Circumcircle centers
            // foreach(Triangle t in Delauney.triangles) {
            //     if(t.Boundary) continue;
            //     Debug.DrawLine(new Vector3(t.p0.X, y, t.p0.Y), new Vector3(t.Circumcenter.x, y, t.Circumcenter.y), Color.magenta, 0);
            //     Debug.DrawLine(new Vector3(t.p1.X, y, t.p1.Y), new Vector3(t.Circumcenter.x, y, t.Circumcenter.y), Color.magenta, 0);
            //     Debug.DrawLine(new Vector3(t.p2.X, y, t.p2.Y), new Vector3(t.Circumcenter.x, y, t.Circumcenter.y), Color.magenta, 0);
            // }

            // Draw incomplete Voronoi cells
            //foreach(Face f in Voronoi.Faces.Values) {
            //    if(f.IsComplete) continue;
            //    if(f.Start == f.End) continue;

            //    var middle = new Vector3(f.Point.X, y, f.Point.Y);
            //    var start = f.GetAdjustedStart(f.Start);
            //    var end = f.GetAdjustedEnd(f.End);

            //    Debug.DrawLine(middle, new Vector3(start.X, y, start.Y), Color.magenta, 0);
            //    Debug.DrawLine(middle, new Vector3(end.X, y, end.Y), Color.magenta, 0);
            //}


            foreach(Face f in Voronoi.Faces.Values) {
                if(f.CutPolygon == null) continue;
                if(f.CutPolygon.Count < 2) continue;

                var prev = f.CutPolygon.Values.Last();
                foreach(Vertex v in f.CutPolygon.Keys) {
                    Debug.DrawLine(new Vector3(prev.X, y, prev.Y), new Vector3(v.X, y, v.Y), Color.magenta, 0);
                    prev = v;
                    Debug.DrawLine(new Vector3(f.Point.X, y, f.Point.Y), new Vector3(v.X, y, v.Y), Color.green, 0);
                }
            }
        }
    }

    public List<Vector2> GetPointOrder()
    {
        return pointOrder;
    }
}
