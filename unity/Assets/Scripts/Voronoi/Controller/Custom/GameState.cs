using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class GameState
{

    private Triangulation triangulation;

    private HashSet<Edge> polygon = new HashSet<Edge>();
    private HashSet<Triangle> newTriangles = new HashSet<Triangle>();

    public GameState(Vector2 bottomLeft, Vector2 topRight)
    {
        // Initialize Triangulation
        triangulation = new Triangulation();

        // Border rectangle vertices
        var bLeft = new Vertex(bottomLeft);
        var tLeft = new Vertex(bottomLeft.x, topRight.y);
        var bRight = new Vertex(topRight.x, bottomLeft.y);
        var tRight = new Vertex(topRight);
        triangulation.Add( bLeft, tLeft, bRight, tRight);

        // Border rectangle edges
        var left = new Edge(bLeft, tLeft);
        var top = new Edge(tRight, tLeft);
        var right = new Edge(bRight, tRight);
        var bottom = new Edge(bRight, bLeft);
        triangulation.Add(left, top, right, bottom);
        foreach(Edge edge in triangulation.edges) {
            edge.AddFace(null);
        }

        var diagonal = new Edge(tLeft, bRight);
        triangulation.Add(diagonal);

        // Border rectangle triangles
        var tr1 = new Triangle(left, diagonal, bottom);
        var tr2 = new Triangle(diagonal, right, top);
        triangulation.Add(tr1, tr2);
    }


    public void AddPoint(Vector2 point)
    {
        var badTriangles = FindBadTriangles(point);
        var polygon = FindHoleBoundary(badTriangles);
        this.polygon = polygon;

        foreach(Triangle triangle in badTriangles) {
            triangulation.Remove(triangle);
        }

        newTriangles = new HashSet<Triangle>();

        var vertex = new Vertex(point);
        triangulation.Add(vertex);
        foreach(Edge edge0 in polygon) {
            var edge1 = new Edge(edge0.end, vertex);
            var edge2 = new Edge(vertex, edge0.start);
            triangulation.Add(edge0, edge1, edge2);

            var triangle = new Triangle(edge0, edge1, edge2);
            triangulation.Add(triangle);
            newTriangles.Add(triangle);
        }
    }

    private HashSet<Triangle> FindBadTriangles(Vector2 point)
    {
        var badTriangles = triangulation.triangles.Where(triangle => triangle.IsPointInsideCircumcircle(point));
        return new HashSet<Triangle>(badTriangles);
    }

    private HashSet<Edge> FindHoleBoundary(HashSet<Triangle> badTriangles)
    {
        var allEdges = new List<Edge>();
        foreach(Triangle triangle in badTriangles) {
            allEdges.AddRange(triangle.Edges);
        }
        var polygon = allEdges.GroupBy(edge => edge).Where(group => group.Count() == 1).Select(group => group.First());
        return new HashSet<Edge>(polygon);
    }

    private void Undo()
    {

    }

    public void DebugDraw(float y)
    {
        triangulation.DebugDraw(y);

        foreach(Triangle t in newTriangles) {
            var sx = 0.0f;
            var sy = 0.0f;
            foreach(Edge e in t.Edges) {
                Debug.DrawLine(new Vector3(e.start.X, y, e.start.Y), new Vector3(e.end.X, y, e.end.Y), Color.red, 0);
                sx += e.start.X;
                sy += e.start.Y;
            }
            sx /= 3;
            sy /= 3;

            //Debug.DrawLine(new Vector3(t.p0.X, y, t.p0.Y), new Vector3(sx, y, sy), Color.red, 0);
            //Debug.DrawLine(new Vector3(t.p1.X, y, t.p1.Y), new Vector3(sx, y, sy), Color.red, 0);
            //Debug.DrawLine(new Vector3(t.p2.X, y, t.p2.Y), new Vector3(sx, y, sy), Color.red, 0);

        }

        foreach(Edge e in polygon) {
            Debug.DrawLine(new Vector3(e.start.X, y, e.start.Y), new Vector3(e.end.X, y, e.end.Y), Color.green, 0);
        }
    }
}
