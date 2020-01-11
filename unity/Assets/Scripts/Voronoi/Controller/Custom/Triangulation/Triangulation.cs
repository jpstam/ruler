using UnityEngine;
using System.Collections.Generic;
using System;

public class Triangulation
{
    public Dictionary<Vector2, Vertex> Vertices { get; private set; }
    public HashSet<Triangle> Triangles { get; private set; }

    public Triangulation()
    {
        Vertices = new Dictionary<Vector2, Vertex>();
        Triangles = new HashSet<Triangle>();
    }

    public void Add(params Vertex[] vertices)
    {
        foreach(Vertex vertex in vertices)
            this.Add(vertex);
    }

    public Vertex Add(Vertex vertex)
    {
        if(Vertices.ContainsKey(vertex.point)) {
            Vertex existingVertex;
            Vertices.TryGetValue(vertex.point, out existingVertex);
            return existingVertex;
        } else {
            Vertices.Add(vertex.point, vertex);
            return vertex;
        }
    }

    public void Add(params Triangle[] triangles)
    {
        foreach(Triangle triangle in triangles)
        this.Triangles.Add(triangle);
    }

    public void Remove(Triangle triangle)
    {
        triangle.p0.Faces.Remove(triangle);
        triangle.p1.Faces.Remove(triangle);
        triangle.p2.Faces.Remove(triangle);
        Triangles.Remove(triangle);
    }

    public Triangulation Copy()
    {
        var tri = new Triangulation();
        foreach(Triangle triangle in Triangles) {
            var p0 = tri.Add(triangle.p0.Copy());
            var p1 = tri.Add(triangle.p1.Copy());
            var p2 = tri.Add(triangle.p2.Copy());
            tri.Add(new Triangle(p0, p1, p2));
        }
        return tri;
    }

    public void DebugDraw(float y)
    {
        /*foreach(Edge e in edges) {
            Debug.DrawLine(new Vector3(e.start.X, y, e.start.Y), new Vector3(e.end.X, y, e.end.Y), Color.white, 0);
        }*/

        foreach(Triangle t in Triangles) {
            var sx = 0.0f;
            var sy = 0.0f;
            foreach(Vertex v in t.Vertices) {
                sx += v.X;
                sy += v.Y;
            }
            sx /= 3;
            sy /= 3;

            Debug.DrawLine(new Vector3(t.p0.X, y, t.p0.Y), new Vector3(sx, y, sy), Color.gray, 0);
            Debug.DrawLine(new Vector3(t.p1.X, y, t.p1.Y), new Vector3(sx, y, sy), Color.gray, 0);
            Debug.DrawLine(new Vector3(t.p2.X, y, t.p2.Y), new Vector3(sx, y, sy), Color.gray, 0);

            Debug.DrawLine(new Vector3(t.p0.X, y, t.p0.Y), new Vector3(t.p1.X, y, t.p1.Y), Color.white, 0);
            Debug.DrawLine(new Vector3(t.p1.X, y, t.p1.Y), new Vector3(t.p2.X, y, t.p2.Y), Color.white, 0);
            Debug.DrawLine(new Vector3(t.p2.X, y, t.p2.Y), new Vector3(t.p0.X, y, t.p0.Y), Color.white, 0);

        }
    }

}
