using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaScore : ScoreFunction {
    public override float ComputeScore(Vector2 option, GameState gs) {
        return Random.Range(0f, 100f);
    }
}
