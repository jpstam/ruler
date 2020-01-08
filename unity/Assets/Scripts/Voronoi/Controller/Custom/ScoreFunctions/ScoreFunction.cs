using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ScoreFunction
{
    public List<float> ComputeScores(List<Vector2> options, GameState gs) {
        return options.ConvertAll(o => TestMove(o, gs));
    }

    protected float TestMove(Vector2 option, GameState gs) {
        // First apply the move option to the gamestate

        // TODO: Update the code to reuse the provided gamestate after undo is done
        //gs.AddPoint(option);

        // TEMP: For now, just create a copy of the gamestate and apply the move option
        GameState gsTemp = new GameState(gs.BottomLeft, gs.TopRight);
        foreach (Vector2 point in gs.GetPointOrder()) {
            gsTemp.AddPoint(point);
        }
        Debug.Log(option);
        gsTemp.AddPoint(option);

        float score = ComputeScore(option, gsTemp);

        // TODO: Update the code to reuse the provided gamestate after undo is done
        //gs.Undo(option);

        return score;
    }

    protected abstract float ComputeScore(Vector2 option, GameState gs);
}
