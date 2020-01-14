using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Strategy
{
    public abstract List<Vector2> ComputeOptions(GameState gs, bool player1Move);
}
