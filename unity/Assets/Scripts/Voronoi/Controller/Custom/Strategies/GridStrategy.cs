using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridStrategy : Strategy {

    protected int xOptions;
    protected int yOptions;


    public GridStrategy(int xOptions, int yOptions) : base() {
        this.xOptions = xOptions;
        this.yOptions = yOptions;
    }

    public override List<Vector2> ComputeOptions(GameState gs) {
        List<Vector2> options = new List<Vector2>();

        for (int x = 0; x < xOptions; x++) {
            for (int y = 0; y < yOptions; y++) {
                float xCoord = Mathf.Lerp(gs.BottomLeft.x, gs.TopRight.x, x * 1f / (xOptions - 1)) + Random.value - 0.5f;
                float yCoord = Mathf.Lerp(gs.BottomLeft.y, gs.TopRight.y, y * 1f / (yOptions - 1)) + Random.value - 0.5f;
                options.Add(new Vector2(xCoord, yCoord));
            }
        }

        return options;
    }
}
