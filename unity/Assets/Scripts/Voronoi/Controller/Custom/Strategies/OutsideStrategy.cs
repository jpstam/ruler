﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutsideStrategy : Strategy
{

    protected float distance;

    public OutsideStrategy(float distance) : base()
    {
        this.distance = distance;
    }

    public override List<Vector2> ComputeOptions(GameState gs, bool player1Move) {
        List<Vector2> options = new List<Vector2>();

        Vector2 left = gs.TopRight;
        Vector2 right = gs.BottomLeft;
        Vector2 top = gs.BottomLeft;
        Vector2 bottom = gs.TopRight;

        foreach (Face face in gs.Voronoi.Faces.Values)
        {
            if (face.Point.X < left.x)
            {
                left = face.Point.point;
            }
            if (face.Point.X > right.x)
            {
                right = face.Point.point;
            }
            if (face.Point.Y < bottom.y)
            {
                bottom = face.Point.point;
            }
            if (face.Point.Y > top.y)
            {
                top = face.Point.point;
            }
        }
        options.Add(new Vector2(left.x - distance, left.y));
        options.Add(new Vector2(right.x + distance, right.y));
        options.Add(new Vector2(top.x, top.y + distance));
        options.Add(new Vector2(bottom.x, bottom.y - distance));

        return options;
    }
}
