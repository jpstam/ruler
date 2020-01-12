using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceScore : ScoreFunction {
    public override float ComputeScore(Vector2 option, GameState gs) {
        float distance = 0;

        List<Vertex> playerVertices = new List<Vertex>();
        foreach (Vertex vertex in gs.Voronoi.Faces.Keys) {
            Face face;
            bool exists = gs.Voronoi.Faces.TryGetValue(vertex, out face);
            if (!exists) {
                continue;
            }

            int p = face.OwnedByPlayer1 ? 0 : 1;

            if(!face.OwnedByPlayer1) {
                playerVertices.Add(vertex);
            }

        }

        foreach (Vertex v1 in playerVertices) {
            foreach (Vertex v2 in playerVertices) {
                distance += Mathf.Sqrt(Mathf.Pow(v1.X - v2.X, 2) + Mathf.Pow(v1.Y - v2.Y, 2));
            }
        }

        return distance;
    }
}
