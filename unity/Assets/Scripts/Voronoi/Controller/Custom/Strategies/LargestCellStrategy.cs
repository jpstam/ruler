using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LargestCellStrategy : Strategy {

    public override List<Vector2> ComputeOptions(GameState gs, bool player1Move) {
        List<Vector2> options = new List<Vector2>();

        // Compute the largest area face owned by the opponent
        Face largestFace = null;
        float largestArea = float.MinValue;
        foreach (Face face in gs.Voronoi.Faces.Values) {
            // Ignore faces that are owned by the player currently in turn
            if(face.OwnedByPlayer1 == player1Move) {
                continue;
            }
            float area = face.ComputeArea();
            if(largestFace == null || area > largestArea) {
                largestArea = area;
                largestFace = face;
            }
        }

        // If there is a largest face with a CutPolygon, generate a move option
        // in the center of the face
        if (largestFace != null && largestFace.CutPolygon != null) {
            float avgX = 0;
            float avgY = 0;

            foreach (Vertex vertex in largestFace.CutPolygon.Keys) {
                avgX += vertex.X;
                avgY += vertex.Y;
            }

            avgX /= Mathf.Max(largestFace.CutPolygon.Count, 1);
            avgY /= Mathf.Max(largestFace.CutPolygon.Count, 1);

        
            options.Add(new Vector2(avgX, avgY));
        }

        return options;
    }
}
