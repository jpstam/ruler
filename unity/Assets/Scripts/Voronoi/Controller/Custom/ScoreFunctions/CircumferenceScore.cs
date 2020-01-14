using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircumferenceScore : ScoreFunction {
    public override float ComputeScore(Vector2 option, GameState gs) {
        float[] circumferences = gs.Voronoi.ComputeCircumferences();
        Debug.Log("Circumferences score: " + circumferences[0] + " - " + circumferences[1]);
        return circumferences[0];
    }
}
