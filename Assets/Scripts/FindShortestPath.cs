using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NUnit.Framework;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using System.Diagnostics;

using Room = DungeonGenerator.Room;

public class FindShortestPath : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;

    private Room startRoom;
    private Room targetRoom;

    public static FindShortestPath Instance { get; private set; }
    public event Action<List<Room>> OnPathUpdated;

    private List<Room> shortestPath;

    public event System.Action<float> OnAStarFinished;
    private float timeTaken;



    private void Awake()
    {
        // Static singleton instance
        if (Instance == null) Instance = this;
        else Destroy(gameObject); // Prevents duplicates}
    }

    private void Start()
    {
        dungeonGenerator.OnMazeGenerated += OnMazeGenerated;
    }
    
    private void OnMazeGenerated(float time)
    {
        UnityEngine.Debug.Log("Looking for shortest path");
        startRoom = dungeonGenerator.graph[dungeonGenerator.startPos];
        targetRoom = dungeonGenerator.graph[dungeonGenerator.endPos];

        // Do AStar and Time it
        Stopwatch stopwatch = Stopwatch.StartNew();
        shortestPath = AStar(startRoom, targetRoom);
        timeTaken = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"AStar Took {timeTaken} ms.");
        OnAStarFinished?.Invoke( timeTaken ); // Signal

        UnityEngine.Debug.Log("Shortest path found! It is this: ");
        foreach (var room in shortestPath)
        {
            UnityEngine.Debug.Log("Room" + room.position.x + "-" + room.position.y);
        }

        StartCoroutine(DelayedSignalInvoke());
    }

    public static List<Room> AStar (Room startRoom, Room targetRoom)
    {

        // Open Set: Rooms to evaluate
        List<Room> openSet = new List<Room> { startRoom };

        // Closed Set: Rooms already evaluated
        HashSet<Room> closedSet = new HashSet<Room>();

        // Cost Tracking
        Dictionary<Room, float> gCost = new Dictionary<Room, float> { [startRoom] = 0 };
        Dictionary<Room, float> hCost = new Dictionary<Room, float> { [startRoom] = ManhattanDistance(startRoom, targetRoom) };
        Dictionary<Room, Room> cameFrom = new Dictionary<Room, Room>(); // Keeps track of path

        while (openSet.Count > 0)
        {
            // Select the Room with the lowest FCost (GCost + HCost)
            Room currentRoom = openSet.OrderBy(room => gCost[room] + hCost[room]).First();

            // Get the cost of moving anywhere from this room
            float moveCost = currentRoom.weight;

            // If we reached the target room, reconstruct and return the path
            if (currentRoom == targetRoom)
                return ReconstructPath(cameFrom, targetRoom);

            openSet.Remove(currentRoom);
            closedSet.Add(currentRoom);

            UnityEngine.Debug.Log($"Processing room {currentRoom.position.x}-{currentRoom.position.y}");
            // Debug.Log("Has neighbors: ");

            // Process each neighboring room
            foreach (var neighborPair in currentRoom.neighbors)
            {
                Room neighbor = neighborPair.Key;

                // Debug.Log(neighbor.position.x + " " + neighbor.position.y);

                if (closedSet.Contains(neighbor))
                    continue; // Skip already processed rooms

                float tentativeGCost = gCost[currentRoom] + moveCost;

                if (!gCost.ContainsKey(neighbor) || tentativeGCost < gCost[neighbor])
                {
                    // Update costs and parent reference
                    gCost[neighbor] = tentativeGCost;
                    hCost[neighbor] = ManhattanDistance(neighbor, targetRoom);
                    cameFrom[neighbor] = currentRoom;

                    if (!openSet.Contains(neighbor)) openSet.Add(neighbor);
                }
            }
        }

        return null;

    }

    private static List<Room> ReconstructPath(Dictionary<Room, Room> cameFrom, Room currentRoom)
    {
        List<Room> path = new List<Room> { currentRoom };

        while (cameFrom.ContainsKey(currentRoom))
        {
            currentRoom = cameFrom[currentRoom];
            path.Add(currentRoom);

            UnityEngine.Debug.Log("Shortest Path: " + currentRoom.position.x + " " + currentRoom.position.y);
        }

        

        path.Reverse(); // Start from beginning
        return path;
    }

    private static float ManhattanDistance(Room a, Room b)
    {
        return Mathf.Abs(a.position.x - b.position.x) + Mathf.Abs(a.position.y - b.position.y);
    }

    IEnumerator DelayedSignalInvoke()
    {
        UnityEngine.Debug.Log("Before pause");
        yield return new WaitForSeconds(0.5f); // Waits for 1 second
        UnityEngine.Debug.Log("After pause");

        OnPathUpdated?.Invoke(shortestPath);
    }
}
