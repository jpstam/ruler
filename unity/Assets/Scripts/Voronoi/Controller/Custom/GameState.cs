using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameState
{

    private Triangulation Delauney;
    private Graph Voronoi;

    private HashSet<Edge> polygon = new HashSet<Edge>();
    private HashSet<Triangle> newTriangles = new HashSet<Triangle>();
    private HashSet<Edge> newVoronoiEdges = new HashSet<Edge>();

    public GameState(Vector2 bottomLeft, Vector2 topRight)
    {
        // Initialize Triangulation
        Delauney = new Triangulation();
        Voronoi = new Graph();

        // Border rectangle vertices
        var bLeft = new Vertex(bottomLeft);
        var tLeft = new Vertex(bottomLeft.x, topRight.y);
        var bRight = new Vertex(topRight.x, bottomLeft.y);
        var tRight = new Vertex(topRight);
        Delauney.Add(bLeft, tLeft, bRight, tRight);

        bLeft.Boundary = true;
        tLeft.Boundary = true;
        bRight.Boundary = true;
        tRight.Boundary = true;

        // Border rectangle triangles
        var tr1 = new Triangle(bLeft, tLeft, bRight);
        var tr2 = new Triangle(tLeft, bRight, tRight);
        Delauney.Add(tr1, tr2);
    }


    public void AddPoint(Vector2 point)
    {
        var badTriangles = FindBadTriangles(point);
        var polygon = FindHoleBoundary(badTriangles);
        this.polygon = polygon;

        foreach(Triangle triangle in badTriangles) {
            Delauney.Remove(triangle);

            Voronoi.Remove(triangle.Circumcenter);
        }

        newTriangles = new HashSet<Triangle>();
        newVoronoiEdges = new HashSet<Edge>();

        var vertex = new Vertex(point);
        Delauney.Add(vertex);
        foreach(Edge edge in polygon) {
            var triangle = new Triangle(edge.start, edge.end, vertex);
            Delauney.Add(triangle);
            newTriangles.Add(triangle);
        }

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
                    newVoronoiEdges.Add(edge);
                }
            }
        }
    }

    private HashSet<Triangle> FindBadTriangles(Vector2 point)
    {
        var badTriangles = Delauney.triangles.Where(triangle => triangle.IsPointInsideCircumcircle(point));
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

    private void Undo()
    {

    }

    public void DebugDraw(float y, bool delauney, bool delauneyDebug, bool voronoi, bool voronoiDebug)
    {
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
        }
    }
}
