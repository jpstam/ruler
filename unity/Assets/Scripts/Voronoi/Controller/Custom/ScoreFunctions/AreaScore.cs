using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaScore : ScoreFunction {
    public override float ComputeScore(Vector2 option, GameState gs) {
        float[] areas = gs.Voronoi.ComputeArea();
        Debug.Log("Area score: " + areas[0] + " - " + areas[1]);
        // Return the area for the red player
        // TODO: allow computing the score for both players
        return areas[1];
    }
}
