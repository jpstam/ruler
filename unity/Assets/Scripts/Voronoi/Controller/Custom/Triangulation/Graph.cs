using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class Graph
{
    public Dictionary<Vector2, Vertex> vertices { get; private set; }
    public Dictionary<Edge, Edge> Edges { get; private set; }
    public Dictionary<Vertex, Face> Faces { get; private set; }

    public Graph()
    {
        this.vertices = new Dictionary<Vector2, Vertex>();
        this.Edges = new Dictionary<Edge, Edge>();
        this.Faces = new Dictionary<Vertex, Face>();
    }

    public Vertex Add(Vertex vertex)
    {
        if(vertices.ContainsKey(vertex.point)) {
            Vertex existingVertex;
            vertices.TryGetValue(vertex.point, out existingVertex);
            return existingVertex;
        } else {
            vertices.Add(vertex.point, vertex);
            return vertex;
        }
    }

    public Edge Add(Edge edge)
    {
        if(Edges.ContainsKey(edge)) {
            Edge existingVertex;
            Edges.TryGetValue(edge, out existingVertex);
            return existingVertex;
        } else {
            Edges.Add(edge, edge);
            return edge;
        }
    }

    public void Remove(Vector2 point)
    {
        Vertex removedPoint;
        vertices.TryGetValue(point, out removedPoint);
        if(removedPoint != null) {
            vertices.Remove(point);
            foreach(Edge edge in removedPoint.Edges) {
                Edges.Remove(edge);
                if (edge.left != null) {
                    edge.left.Remove(edge);
                }
                if (edge.right != null) {
                    edge.right.Remove(edge);
                }
            }
        }
    }

    public Graph Copy()
    {
        var graph = new Graph();

        foreach(Edge edge in Edges.Keys) {
            var start = graph.Add(edge.start.Copy());
            var end = graph.Add(edge.end.Copy());
            graph.Add(new Edge(start, end));
        }

        foreach(KeyValuePair<Vertex, Face> pair in Faces) {
            var face = new Face(pair.Key.Copy());
            foreach(Edge old in pair.Value.Edges.Values) {
                var edge = graph.Add(old);
                face.Add(edge);
                face.OwnedByPlayer1 = pair.Value.OwnedByPlayer1;
            }
            graph.Faces.Add(face.Point, face);
        }

        return graph;
    }

    public void DebugDraw(float y)
    {
        foreach(Edge e in Edges.Keys) {
            Debug.DrawLine(new Vector3(e.start.X, y, e.start.Y), new Vector3(e.end.X, y, e.end.Y), Color.blue, 0);
        }
    }
}