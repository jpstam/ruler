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

    public override List<Vector2> ComputeOptions(GameState gs, bool player1Move) {
        List<Vector2> options = new List<Vector2>();

        for (int x = 0; x < xOptions; x++) {
            for (int y = 0; y < yOptions; y++) {
                float startX = 1f / (xOptions + 1) / 2;
                float endX = 1 - startX;
                float stepX = (endX - startX) / (xOptions - 1);
                float startY = 1f / (yOptions + 1) / 2;
                float endY = 1 - startY;
                float stepY = (endY - startY) / (yOptions - 1);
                float xCoord = Mathf.Lerp(gs.BottomLeft.x, gs.TopRight.x, startX + x * stepX) + Random.value * 0.2f - 0.1f;
                float yCoord = Mathf.Lerp(gs.BottomLeft.y, gs.TopRight.y, startY + y * stepY) + Random.value * 0.2f - 0.1f;
                options.Add(new Vector2(xCoord, yCoord));
            }
        }

        return options;
    }
}
