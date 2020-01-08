using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoronoiAI : MonoBehaviour
{
    private Vector3 bottomLeft;
    private Vector3 topRight;

    private GameState gs;
    private StrategyHandler sh;
    private ScoreFunction sf;

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
        sh = new StrategyHandler().Add(new GridStrategy(6,5));
        sf = new AreaScore();
    }

    public Vector2 GetMove()
    {
        // Clear out any debug spheres used for visualizing the options
        GameObject[] gos = GameObject.FindGameObjectsWithTag("debugSphere");
        foreach (GameObject go in gos) {
            Destroy(go);
        }

        // Get the best move using the minimax algorithm
        Vector2 move = MiniMax.GetBestMove(gs, sh, sf);

        // Get all the options, used for debugging
        List<Vector2> options = MiniMax.GetOptions();

        // For each option, add a sphere to visualize the options
        foreach (Vector2 option in options) {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = new Vector3(option.x, 1, option.y);
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            go.tag = "debugSphere";
        }

        Debug.Log("Options = " + System.String.Join(", ", new List<Vector2>(options).ConvertAll(o => o.ToString()).ToArray()));

        Debug.Log("Ai is making move: " + move);
        return move;
    }

    public void AddMove(Vector2 move)
    {
        gs.AddPoint(move);
    }

}
