using UnityEngine;

public class FindShortestPath : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;

    public void GetMazeGraphAndRun()
    {
        var graph = dungeonGenerator.graph;
    }

    public void GetShortestPath ()
    {
        return;
    }
}
