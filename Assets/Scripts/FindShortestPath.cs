using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

using Room = DungeonGenerator.Room;

public class FindShortestPath : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;

    private void Start()
    {
        dungeonGenerator.OnMazeGenerated += OnMazeGenerated;
    }
    
    private void OnMazeGenerated()
    {
        AStar(dungeonGenerator.startPos, dungeonGenerator.endPos);
    }

    public List<Node> AStar (Vector2Int start, Vector2Int target)
    {
        var startRoom = dungeonGenerator.graph[start];
        var endRoom = dungeonGenerator.graph[start];

        List<Room> openSet = new List<Room> { startRoom };
        HashSet<Room> closedSet = new HashSet<Room>();

        return null;
    }
}
