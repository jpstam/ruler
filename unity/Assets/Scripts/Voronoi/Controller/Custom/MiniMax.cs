using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MiniMax
{
    private static List<Vector2> storedOptions;

    public static Vector2 GetBestMove(GameState gs, StrategyHandler sh, ScoreFunction sf) {
        // First let the strategy handler determine all possible move options
        List<Vector2> options = sh.ComputeOptions(gs);

        // Then for each option, compute a score
        List<float> scores = sf.ComputeScores(options, gs);


        // For now make a random move from all options
        // TODO: Compute a score for each option and choose the highest score
        // TODO: Finally, implement minimax using the options in recursion and the scores
        int index = 0;
        float max = float.MinValue;
        for (int i = 0; i < scores.Count; i++) {
            if (scores[i] > max) {
                max = scores[i];
                index = i;
            }
        }

        Vector2 move = options[index];

        storedOptions = options;
        return move;
    }

    public static List<Vector2> GetOptions() {
        return storedOptions;
    }
}
