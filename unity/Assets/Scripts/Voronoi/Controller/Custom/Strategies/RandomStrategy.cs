using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomStrategy : Strategy {

    protected int samples;


    public RandomStrategy(int samples) : base() {
        this.samples = samples;
    }

    public override List<Vector2> ComputeOptions(GameState gs) {
        List<Vector2> options = new List<Vector2>();

        for (int x = 0; x < samples; x++) {
            float xCoord = Random.Range(gs.BottomLeft.x, gs.TopRight.x);
            float yCoord = Random.Range(gs.BottomLeft.y, gs.TopRight.y);
            options.Add(new Vector2(xCoord, yCoord));
        }

        return options;
    }
}
