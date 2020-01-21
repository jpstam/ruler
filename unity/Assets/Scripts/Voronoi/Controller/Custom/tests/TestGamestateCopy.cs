using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGamestateCopy
{
    public static void Test(GameState gs)
    {
        int iterations = 100;
        System.Diagnostics.Stopwatch watch;
        // Test the difference in speed for duplicating the gamestate from scratch and making a deep copy

        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++) {
            duplicateGamestate(gs);
        }
        watch.Stop();
        Debug.Log("TEST: duplicated gamestate " + iterations + " times in " + watch.ElapsedMilliseconds + "ms");

        watch = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++) {
            copyGamestate(gs);
        }
        watch.Stop();
        Debug.Log("TEST: copied gamestate " + iterations + " times in " + watch.ElapsedMilliseconds + "ms");

    }

    private static void duplicateGamestate(GameState gs)
    {
        // Test the speed for duplicating the gamestate
        GameState gsTemp = new GameState(gs.BottomLeft, gs.TopRight);
        bool tempPlayer1 = true;
        foreach (Vector2 point in gs.GetPoints()) {
            gsTemp.AddPoint(point, tempPlayer1);
            tempPlayer1 = !tempPlayer1;
        }
    }

    private static void copyGamestate(GameState gs) {
        // Test the speed for copying the gamestate
        GameState gsTemp = gs.Copy();
    }
}
