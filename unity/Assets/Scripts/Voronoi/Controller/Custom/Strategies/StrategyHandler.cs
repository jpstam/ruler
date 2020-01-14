using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StrategyHandler
{
    private List<Strategy> strategies = new List<Strategy>();

    public StrategyHandler Add(Strategy strategy) {
        strategies.Add(strategy);
        return this;
    }

    public List<Vector2> ComputeOptions(GameState gs, bool player1Move) {
        List<Vector2> options = new List<Vector2>();

        foreach (Strategy strategy in strategies) {
            options.AddRange(strategy.ComputeOptions(gs, player1Move));
        }

        return options;
    }

}
