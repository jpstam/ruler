using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Util.Geometry;
using Voronoi.Custom.Algorithms;

public class OutsideCHStrategy : Strategy
{

    protected float distance;
    protected Strategy alternative; //used when only one point is present

    public OutsideCHStrategy(float distance, Strategy alternative) : base()
    {
        this.distance = distance;
        this.alternative = alternative;
    }

    public override List<Vector2> ComputeOptions(GameState gs, bool player1Move) {
        List<Vector2> options = new List<Vector2>();

        // too less
        if (gs.GetPoints().Count < 2)
        {
            return alternative.ComputeOptions(gs, player1Move);
        }

        List<Vector2> cHull = CustomConvexHull.QuickHull(gs.GetPoints());

        Vector2 prev = cHull[cHull.Count - 2]; // Never out of range because the hull contains at least 2 points
        Vector2 cur = cHull.Last();

        foreach (Vector2 next in cHull)
        {
            Vector2 d1 = (prev - cur).normalized;
            Vector2 d2 = (next - cur).normalized;
            Vector2 dout = (d1 + d2).normalized;
            options.Add(cur - distance * dout);
            prev = cur;
            cur = next;
        }

        return options;
    }
}
