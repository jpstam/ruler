using UnityEngine;
using System.Collections;

public class VoronoiAI : MonoBehaviour
{
    private Vector3 bottomLeft;
    private Vector3 topRight;

    private GameState gs;

    // Use this for initialization
    void Start()
    {

    }

    void FixedUpdate()
    {
        if (gs != null)
            gs.DebugDraw(bottomLeft.y);
    }

    public void SetCorners(Vector3 bottomLeft, Vector3 topRight)
    {
        this.bottomLeft = bottomLeft;
        this.topRight = topRight;
        gs = new GameState(new Vector2(bottomLeft.x, bottomLeft.z), new Vector2(topRight.x, topRight.z));
    }

    public Vector2 GetMove()
    {
        var x = Random.Range(bottomLeft.x, topRight.x);
        var y = Random.Range(bottomLeft.z, topRight.z);
        var move = new Vector2(x, y);
        Debug.Log("Ai is making move: " + move);
        gs.AddPoint(move);
        return move;
    }

    public void AddMove(Vector2 move)
    {
        gs.AddPoint(move);
    }

}
