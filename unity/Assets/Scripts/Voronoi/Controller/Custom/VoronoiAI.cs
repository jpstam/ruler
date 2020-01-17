using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VoronoiAI : MonoBehaviour
{
    public bool DrawBorders = false;
    public bool DrawDelauneyTriangulation = false;
    public bool DrawDelauneyDebug = false;
    public bool DrawVoronoiDiagram = false;
    public bool DrawVoronoiDebug = false;
    public bool DrawConvexHull = false;

    private bool player1;

    private Vector3 bottomLeft;
    private Vector3 topRight;

    public GameState gs { get; private set; }
    private StrategyHandler sh;
    private ScoreFunction sf;

    public VoronoiAI(bool player1) : base() {
        this.player1 = player1;
    }

    // Use this for initialization
    void Start()
    {

    }

    void FixedUpdate()
    {
        if (gs != null)
            gs.DebugDraw(bottomLeft.y, DrawBorders, DrawDelauneyTriangulation, DrawDelauneyDebug, DrawVoronoiDiagram, DrawVoronoiDebug, DrawConvexHull);
    }

    public void SetCorners(Vector3 bottomLeft, Vector3 topRight)
    {
        this.bottomLeft = bottomLeft;
        this.topRight = topRight;
        gs = new GameState(new Vector2(bottomLeft.x, bottomLeft.z), new Vector2(topRight.x, topRight.z));
    }

    public void SetPlayer1(bool player1)
    {
        this.player1 = player1;
    }

    public void SetStrategyHandler(StrategyHandler sh)
    {
        this.sh = sh;
    }

    public void SetScoreFunction(ScoreFunction sf)
    {
        this.sf = sf;
    }

    public Vector2 GetMove()
    {
        // Clear out any debug spheres used for visualizing the options
        GameObject[] gos = GameObject.FindGameObjectsWithTag("debugSphere");
        foreach (GameObject go in gos) {
            Destroy(go);
        }

        // Get the best move using the minimax algorithm
        Vector2 move = MiniMax.GetBestMove(player1, gs, sh, sf);

        // Get all the options, used for debugging
        List<Vector2> options = MiniMax.GetOptions();

        // For each option, add a sphere to visualize the options
        foreach (Vector2 option in options) {
            GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            go.transform.position = new Vector3(option.x, 1, option.y);
            go.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            go.tag = "debugSphere";
            if (option.Equals(move)) {
                go.name = "Move";
                go.GetComponent<Renderer>().material.shader = Shader.Find("Unlit/Color");
                go.GetComponent<Renderer>().material.color = Color.red;
            }
        }

        //Debug.Log("Options = " + System.String.Join(", ", new List<Vector2>(options).ConvertAll(o => o.ToString()).ToArray()));

        Debug.Log("Ai is making move: " + move);
        return move;
    }

    public void AddMove(Vector2 move, bool playerOne)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();
        gs.AddPoint(move, playerOne);
        watch.Stop();
        Debug.Log("Added move " + move + " in " + watch.ElapsedMilliseconds + "ms");

        //watch = System.Diagnostics.Stopwatch.StartNew();
        //gs.Copy();
        //watch.Stop();
        //Debug.Log("Copied Gamestate in " + watch.ElapsedMilliseconds + "ms");

    }

}
