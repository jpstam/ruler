using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScoreFunction
{
    public List<float> ComputeScores(List<Vector2> options, GameState gs) {
        return options.ConvertAll(o => ComputeScore(o, gs));
    }

    public abstract float ComputeScore(Vector2 option, GameState gs);
}
