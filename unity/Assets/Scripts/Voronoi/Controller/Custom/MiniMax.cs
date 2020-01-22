using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MiniMax
{

    class ScoredMove
    {
        public ScoredMove(Vector2 value) {
            this.Move = value;
        }

        public Vector2 Move {
            get; private set;
        }

        public float Score {
            get; private set;
        }

        public void SetScore(float value) {
            this.Score = value;
        }
    }

    private static List<Vector2> storedOptions;

    public static Vector2 GetBestMove(bool player1, GameState gs, StrategyHandler sh, ScoreFunction sf) {
        // First let the strategy handler determine all possible move options for this round
        List<Vector2> options = sh.ComputeOptions(gs, player1);

        // Convert the options to scored moves
        List<ScoredMove> scoredMoves = options.ConvertAll(o => new ScoredMove(o));

        foreach (ScoredMove scoredMove in scoredMoves) {
            // The depth specified is the number of moves after the first next move
            float value = DoMiniMax(scoredMove.Move, scoredMove, 1, !player1, gs, sh, sf);
            // Debug.Log("==============================> score: " + value);
            // Debug.Log("-----> Move: " + scoredMove.Move);
            scoredMove.SetScore(value);
        }

        float bestScore;
        if(player1) {
            bestScore = float.MinValue;
        } else {
            bestScore = float.MaxValue;
        }
        ScoredMove bestMove = scoredMoves[0];
        foreach (ScoredMove scoredMove in scoredMoves) {
            if(player1) {
                // Maximize the score if minimax is used for player 1
                if (scoredMove.Score > bestScore) {
                    bestScore = scoredMove.Score;
                    bestMove = scoredMove;
                }
            } else {
                // Minimize the score if minimax is used for player 2
                if (scoredMove.Score < bestScore) {
                    bestScore = scoredMove.Score;
                    bestMove = scoredMove;
                }
            }
            
        }

        Vector2 move = bestMove.Move;

        storedOptions = options;
        return move;
    }

    public static List<Vector2> GetOptions() {
        return storedOptions;
    }

    private static float DoMiniMax(Vector2 nextMove, ScoredMove scoredMove, int depth, bool maximizingPlayer, GameState gs, StrategyHandler sh, ScoreFunction sf) {
        // Create a copy of the gamestate and apply the move option
        GameState gsTemp = gs.Copy();

        gsTemp.AddPoint(nextMove, !maximizingPlayer);

        if (depth <= 0) {
            return sf.ComputeScore(nextMove, gsTemp);
        }

        List<Vector2> options = sh.ComputeOptions(gsTemp, maximizingPlayer);

        if (maximizingPlayer) {
            float max = float.MinValue;
            foreach (Vector2 option in options) {
                // Execute minimax on the new game state
                float value = DoMiniMax(option, scoredMove, depth - 1, false, gsTemp, sh, sf);
                max = Mathf.Max(max, value);
            }
            return max;
        } else {
            float min = float.MaxValue;
            foreach (Vector2 option in options) {
                // Execute minimax on the new game state
                float value = DoMiniMax(option, scoredMove, depth - 1, true, gsTemp, sh, sf);
                min = Mathf.Min(min, value);
            }
            return min;
        }
    }
}
