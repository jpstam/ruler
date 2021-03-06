﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        // Filter out any option which is outside the gamestate playing field
        Vector2 bl = gs.BottomLeft;
        Vector2 tr = gs.TopRight;
        options = options.Where(o => bl.x <= o.x && o.x <= tr.x && bl.y <= o.y && o.y <= tr.y).ToList();

        return options;
    }

}
