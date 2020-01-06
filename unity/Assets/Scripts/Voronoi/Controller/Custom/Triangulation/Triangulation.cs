using UnityEngine;
using System.Collections.Generic;
using System;

public class Triangulation
{
    public HashSet<Vertex> vertices { get; private set; }
    public HashSet<Edge> edges { get; private set; }
    public HashSet<Triangle> triangles { get; private set; }

    public Triangulation()
    {
        vertices = new HashSet<Vertex>();
        edges = new HashSet<Edge>();
        triangles = new HashSet<Triangle>();
    }

    public void Add(params Vertex[] vertices)
    {
        foreach(Vertex vertex in vertices)
            this.vertices.Add(vertex);
    }

    public void Add(params Edge[] edges)
    {
        foreach(Edge edge in edges)
            this.edges.Add(edge);
    }

    public void Add(params Triangle[] triangles)
    {
        foreach(Triangle triangle in triangles)
        this.triangles.Add(triangle);
    }

    public void Remove(Triangle triangle)
    {
        foreach(Edge edge in triangle.Edges) {
            //edge.RemoveFace(triangle);
            //if (edge.faces <= 0) {
                edges.Remove(edge);
            //}
        }
        triangles.Remove(triangle);
    }

    public void DebugDraw(float y)
    {
        /*foreach(Edge e in edges) {
            Debug.DrawLine(new Vector3(e.start.X, y, e.start.Y), new Vector3(e.end.X, y, e.end.Y), Color.white, 0);
        }*/

        foreach(Triangle t in triangles) {
            var sx = 0.0f;
            var sy = 0.0f;
            foreach(Edge e in t.Edges) {
                Debug.DrawLine(new Vector3(e.start.X, y, e.start.Y), new Vector3(e.end.X, y, e.end.Y), Color.white, 0);
                sx += e.start.X;
                sy += e.start.Y;
            }
            sx /= 3;
            sy /= 3;

            Debug.DrawLine(new Vector3(t.p0.X, y, t.p0.Y), new Vector3(sx, y, sy), Color.gray, 0);
            Debug.DrawLine(new Vector3(t.p1.X, y, t.p1.Y), new Vector3(sx, y, sy), Color.gray, 0);
            Debug.DrawLine(new Vector3(t.p2.X, y, t.p2.Y), new Vector3(sx, y, sy), Color.gray, 0);

        }
    }

}
