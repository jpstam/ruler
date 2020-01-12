using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardDeviationScore : ScoreFunction {
    public override float ComputeScore(Vector2 option, GameState gs) {
        List<float> distances = new List<float>();

        List<Vertex> playerVertices = new List<Vertex>();
        foreach (Vertex vertex in gs.Voronoi.Faces.Keys) {
            Face face;
            bool exists = gs.Voronoi.Faces.TryGetValue(vertex, out face);
            if (!exists) {
                continue;
            }

            int p = face.OwnedByPlayer1 ? 0 : 1;

            playerVertices.Add(vertex);

        }

        foreach (Vertex v1 in playerVertices) {
            foreach (Vertex v2 in playerVertices) {
                distances.Add(Mathf.Sqrt(Mathf.Pow(v1.X - v2.X, 2) + Mathf.Pow(v1.Y - v2.Y, 2)));
            }
        }

        float mean = ComputeMean(distances);
        float meanSq = ComputeMean(distances.ConvertAll(v => v * v));

        return -Mathf.Sqrt(meanSq - Mathf.Pow(mean, 2));
    }

    private float ComputeMean(List<float> values) {
        float total = 0;
        foreach (float value in values) {
            total += value;
        }
        return total / Mathf.Max(values.Count, 1);
    }
}
