using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using System.Diagnostics;
using UnityEngine.Analytics;

public class DungeonGenerator : MonoBehaviour
{

    public class Room
    {
        public Dictionary<Room, (float cost, string direction)> neighbors = new Dictionary<Room, (float cost, string direction)>();  // Adjacent rooms with movement cost
    }

    // This is the grid with all of the rooms in it
    // Bottom left of the grid is 0, 0
    // Graph is always positive numbers
    Dictionary<Vector2Int, Room> graph = new Dictionary<Vector2Int, Room>(); // Holds every room based on grid position

    public Vector2 offset;
    public Vector2Int size;
    public GameObject roomPrefab;

    // TO DO: Change this so it clamps to valid values when assigned
    public Vector2Int startPos = new Vector2Int(0, 0);

    // Prob of Random doors being spawned
    public float roomExtraDoorProb = 0.1f;


    // Start is called before the first frame update
    void Start()
    {
        MazeGenerator();
    }

    void GenerateDungeon()
    {
        // Key: Position on grid, value: Room
        foreach (var kvp in graph)
        {
            List<string> directions = new List<string>();
            foreach(var neighborKVP in kvp.Value.neighbors)
            {
                directions.Add(neighborKVP.Value.direction);
            }

            var newRoom = Instantiate(roomPrefab, new Vector3(kvp.Key.x * offset.x, 0, kvp.Key.y * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
            newRoom.UpdateRoom(directions);

            newRoom.name += " " + kvp.Key.x + "-" + kvp.Key.y;
        }
    }

    void MazeGenerator()
    {
        // Timing how long it took to generate maze (not including rendering rooms)
        Stopwatch stopwatch = Stopwatch.StartNew();

        Stack<Vector2Int> path = new Stack<Vector2Int>();

        int k = 0;

        // Add starting room to the graph and save it as current room
        graph.Add(startPos, new Room());
        Vector2Int curRoomKey = startPos;
        
        while(k<10000)
        {
            k++;

            // Get valid neighbors spots
            List<Vector2Int> neighbors = CheckNeighbors(curRoomKey);

            if (neighbors.Count == 0)
            {
                // Break out of loop if nowhere left to go
                if (path.Count == 0) break;
                else
                {
                    curRoomKey = path.Pop();
                }

            }
            else
            {
                path.Push(curRoomKey);
                Vector2Int newRoomKey = neighbors[Random.Range(0, neighbors.Count)];

                // Create new room from previous if it does not exist (shouldn't exist yet actually)
                if (!graph.ContainsKey(newRoomKey))
                {
                    graph.Add(newRoomKey, new Room());
                    UnityEngine.Debug.Log("Creating Room at grid Cords: " + newRoomKey.x + " " + newRoomKey.y);
                }

                if(newRoomKey.y >  curRoomKey.y)
                {
                    // Going up
                    graph[curRoomKey].neighbors.Add(graph[newRoomKey], (1f, "North"));
                    graph[newRoomKey].neighbors.Add(graph[curRoomKey], (1f, "South"));
                    curRoomKey = newRoomKey;

                }
                else if (newRoomKey.y < curRoomKey.y)
                {
                    // Going down
                    graph[curRoomKey].neighbors.Add(graph[newRoomKey], (1f, "South"));
                    graph[newRoomKey].neighbors.Add(graph[curRoomKey], (1f, "North"));
                    curRoomKey = newRoomKey;

                }
                else if (newRoomKey.x > curRoomKey.x)
                {
                    // Going right
                    graph[curRoomKey].neighbors.Add(graph[newRoomKey], (1f, "East"));
                    graph[newRoomKey].neighbors.Add(graph[curRoomKey], (1f, "West"));
                    curRoomKey = newRoomKey;

                }
                else
                {
                    // Going left
                    graph[curRoomKey].neighbors.Add(graph[newRoomKey], (1f, "West"));
                    graph[newRoomKey].neighbors.Add(graph[curRoomKey], (1f, "East"));
                    curRoomKey = newRoomKey;

                }
            }
        }
        PlaceRandomDoors();
        GenerateDungeon();

        // Evaluating time took
        stopwatch.Stop();
        UnityEngine.Debug.Log($"Dungeon Generation Took {stopwatch.ElapsedMilliseconds} ms.");
    }

    // Runs through the maze and places doors. Some rooms get no extra doors, and some may get 1, 2, or 3.
    private void PlaceRandomDoors()
    {
        foreach (var kvp in graph)
        {
            if (Random.value < roomExtraDoorProb)
            {
                var curRoomKey = kvp.Key;

                List<Vector2Int> neighbors = CheckNeighborsForRandomDoor(kvp.Key);
                
                while (neighbors.Count > 0)
                {
                    Vector2Int newRoomKey = neighbors[Random.Range(0, neighbors.Count)];
                    neighbors.Remove(newRoomKey); // Remove 

                    if (newRoomKey.y > curRoomKey.y)
                    {
                        // Going up
                        graph[curRoomKey].neighbors.Add(graph[newRoomKey], (1f, "North"));
                        graph[newRoomKey].neighbors.Add(graph[curRoomKey], (1f, "South"));

                    }
                    else if (newRoomKey.y < curRoomKey.y)
                    {
                        // Going down
                        graph[curRoomKey].neighbors.Add(graph[newRoomKey], (1f, "South"));
                        graph[newRoomKey].neighbors.Add(graph[curRoomKey], (1f, "North"));

                    }
                    else if (newRoomKey.x > curRoomKey.x)
                    {
                        // Going right
                        graph[curRoomKey].neighbors.Add(graph[newRoomKey], (1f, "East"));
                        graph[newRoomKey].neighbors.Add(graph[curRoomKey], (1f, "West"));

                    }
                    else
                    {
                        // Going left
                        graph[curRoomKey].neighbors.Add(graph[newRoomKey], (1f, "West"));
                        graph[newRoomKey].neighbors.Add(graph[curRoomKey], (1f, "East"));

                    }

                    UnityEngine.Debug.Log("Placing Random Door at Room: " + curRoomKey.x + " " + curRoomKey.y + " Connecting it to Room " + newRoomKey.x + " " + newRoomKey.y);

                    // Decide whether to place another door or not
                    if (Random.value < roomExtraDoorProb) continue;
                    else break;
                }
                
            }
        }
    }

    // Checking each neighbor for if a room could exist there and if we haven't already visited that room
    // NOTE: Can optimize better by not taking parameters
    List<Vector2Int> CheckNeighbors(Vector2Int roomKey)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Check North Neighbor (roomKey.y + 1)
        var northRoomKey = new Vector2Int(roomKey.x, roomKey.y + 1);
        if (northRoomKey.y < size.y && !graph.ContainsKey(northRoomKey))
        {
            neighbors.Add(northRoomKey);
        }

        // Check South Neighbor
        var southRoomKey = new Vector2Int(roomKey.x, roomKey.y - 1);
        if (southRoomKey.y >= 0 && !graph.ContainsKey(southRoomKey))
        {
            neighbors.Add(southRoomKey);
        }

        // Check East Neighbor
        var eastRoomKey = new Vector2Int(roomKey.x+1, roomKey.y);
        if (eastRoomKey.x < size.x && !graph.ContainsKey(eastRoomKey))
        {
            neighbors.Add(eastRoomKey);
        }

        // Check West Neighbor
        var westRoomKey = new Vector2Int(roomKey.x - 1, roomKey.y);
        if (westRoomKey.x >= 0 && !graph.ContainsKey(westRoomKey))
        {
            neighbors.Add(westRoomKey);
        }

        return neighbors;
    }

    // Checking if we can place a door across a room
    // Cannot place doors where doors already exist
    private List<Vector2Int> CheckNeighborsForRandomDoor(Vector2Int roomKey)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        // Check North Neighbor (roomKey.y + 1)
        var northRoomKey = new Vector2Int(roomKey.x, roomKey.y + 1);
        if (northRoomKey.y < size.y && graph.ContainsKey(northRoomKey) && !graph[roomKey].neighbors.ContainsKey(graph[northRoomKey]))
        {
            neighbors.Add(northRoomKey);
        }

        // Check South Neighbor
        var southRoomKey = new Vector2Int(roomKey.x, roomKey.y - 1);
        if (southRoomKey.y >= 0 && graph.ContainsKey(southRoomKey) && !graph[roomKey].neighbors.ContainsKey(graph[southRoomKey]))
        {
            neighbors.Add(southRoomKey);
        }

        // Check East Neighbor
        var eastRoomKey = new Vector2Int(roomKey.x + 1, roomKey.y);
        if (eastRoomKey.x < size.x && graph.ContainsKey(eastRoomKey) && !graph[roomKey].neighbors.ContainsKey(graph[eastRoomKey]))
        {
            neighbors.Add(eastRoomKey);
        }

        // Check West Neighbor
        var westRoomKey = new Vector2Int(roomKey.x - 1, roomKey.y);
        if (westRoomKey.x >= 0 && graph.ContainsKey(westRoomKey) && !graph[roomKey].neighbors.ContainsKey(graph[westRoomKey]))
        {
            neighbors.Add(westRoomKey);
        }

        return neighbors;
    }
}
