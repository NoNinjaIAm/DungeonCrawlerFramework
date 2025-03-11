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
        public Vector2Int position; // Room's grid cordinates. For Manhattan Distance
        public Dictionary<Room, string> neighbors = new Dictionary<Room, string>();  // Adjacent rooms and the directions to them
        public float weight = 1.0f; // Room's weight
    }

    // This is the grid with all of the rooms in it
    // Bottom left of the grid is 0, 0
    // Graph is always positive numbers
    public Dictionary<Vector2Int, Room> graph = new Dictionary<Vector2Int, Room>(); // Holds every room based on grid position

    public Vector2 offset;
    public Vector2Int size;
    public GameObject roomPrefab;

    public event System.Action<float> OnMazeGenerated;

    // TO DO: Change this so it clamps to valid values when assigned
    public Vector2Int startPos = new Vector2Int(0, 0);
    public Vector2Int endPos;

    // Probabilities
    public float roomExtraDoorProb = 0.1f;
    public float roomObstacleSpawnProb = 0.1f;

    public float timeToComplete;


    // Start is called before the first frame update
    void Start()
    { 
        if (GlobalOptions.Instance != null)
        {
            size = new Vector2Int(GlobalOptions.Instance.xDungeonSize, GlobalOptions.Instance.xDungeonSize);
            roomExtraDoorProb = GlobalOptions.Instance.extraDoorProb;
            roomObstacleSpawnProb = GlobalOptions.Instance.obsSpawnProb;
        }
        endPos = new Vector2Int(size.x - 1, size.y - 1);

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
                directions.Add(neighborKVP.Value);
            }

            var newRoom = Instantiate(roomPrefab, new Vector3(kvp.Key.x * offset.x, 0, kvp.Key.y * offset.y), Quaternion.identity, transform).GetComponent<RoomBehaviour>();
            
            newRoom.UpdateDoorways(directions);
            newRoom.myGridPosition = kvp.Key;
            
            // Spawning target room object
            if(kvp.Key == endPos)
            {
                UnityEngine.Debug.Log("Placing End Room at " + kvp.Key.x + "-" + kvp.Key.y);
                newRoom.MakeEndRoom();
            }

            // Adding obstacles
            if (graph[kvp.Key].weight > 1.5f) newRoom.AddObstacles(true);
            else newRoom.AddObstacles(false);


            // Printing room name
            newRoom.name += " " + kvp.Key.x + "-" + kvp.Key.y;

        }
        // Dungeon has been generated event
        OnMazeGenerated?.Invoke(timeToComplete);
    }

    void SpawnObstacles()
    {
        foreach (var kvp in graph)
        {
            if(Random.value < roomObstacleSpawnProb)
            {
                //Spawn Obstacles in current room
                var curRoomKey = kvp.Key;

                // Set room weight
                graph[curRoomKey].weight = GlobalOptions.Instance.obsWeight;
                UnityEngine.Debug.Log("Placing Obstacles in room:  " + curRoomKey.x + " " + curRoomKey.y);
            }
        }
    }

    void MazeGenerator()
    {
        // Timing how long it took to generate maze (not including rendering rooms)
        Stopwatch stopwatch = Stopwatch.StartNew();

        Stack<Vector2Int> path = new Stack<Vector2Int>();

        int k = 0;

        // Add starting room to the graph and save it as current room
        var tempRoom = new Room();
        tempRoom.position = startPos;
        graph.Add(startPos, tempRoom);
        Vector2Int curRoomKey = startPos;
        
        while(k<100000000)
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
                    tempRoom = new Room();
                    tempRoom.position = newRoomKey;
                    graph.Add(newRoomKey, tempRoom);
                    UnityEngine.Debug.Log("Creating Room at grid Cords: " + newRoomKey.x + " " + newRoomKey.y);
                }

                if(newRoomKey.y >  curRoomKey.y)
                {
                    // Going North
                    graph[curRoomKey].neighbors.Add(graph[newRoomKey], "North");
                    graph[newRoomKey].neighbors.Add(graph[curRoomKey], "South");
                    curRoomKey = newRoomKey;

                }
                else if (newRoomKey.y < curRoomKey.y)
                {
                    // Going South
                    graph[curRoomKey].neighbors.Add(graph[newRoomKey], "South");
                    graph[newRoomKey].neighbors.Add(graph[curRoomKey], "North");
                    curRoomKey = newRoomKey;

                }
                else if (newRoomKey.x > curRoomKey.x)
                {
                    // Going East
                    graph[curRoomKey].neighbors.Add(graph[newRoomKey], "East");
                    graph[newRoomKey].neighbors.Add(graph[curRoomKey], "West");
                    curRoomKey = newRoomKey;

                }
                else
                {
                    // Going West
                    graph[curRoomKey].neighbors.Add(graph[newRoomKey], "West");
                    graph[newRoomKey].neighbors.Add(graph[curRoomKey], "East");
                    curRoomKey = newRoomKey;

                }
            }
        }
        PlaceRandomDoorsAndObstacles();
        SpawnObstacles();

        // Evaluating time took
        stopwatch.Stop();
        timeToComplete = stopwatch.ElapsedMilliseconds;
        UnityEngine.Debug.Log($"Dungeon Generation Took {timeToComplete} ms.");

        if (GlobalOptions.Instance.renderRooms) GenerateDungeon();
        else OnMazeGenerated?.Invoke(timeToComplete);


    }

    // Runs through the maze and places doors. Some rooms get no extra doors, and some may get 1, 2, or 3.
    private void PlaceRandomDoorsAndObstacles()
    {
        foreach (var kvp in graph)
        {
            // Spawn random door/s
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
                        // Place Edge North
                        graph[curRoomKey].neighbors.Add(graph[newRoomKey], "North");
                        graph[newRoomKey].neighbors.Add(graph[curRoomKey], "South");

                    }
                    else if (newRoomKey.y < curRoomKey.y)
                    {
                        // Place Edge South
                        graph[curRoomKey].neighbors.Add(graph[newRoomKey], "South");
                        graph[newRoomKey].neighbors.Add(graph[curRoomKey], "North");

                    }
                    else if (newRoomKey.x > curRoomKey.x)
                    {
                        // Place Edge East
                        graph[curRoomKey].neighbors.Add(graph[newRoomKey], "East");
                        graph[newRoomKey].neighbors.Add(graph[curRoomKey], "West");

                    }
                    else
                    {
                        // Place Edge West
                        graph[curRoomKey].neighbors.Add(graph[newRoomKey], "West");
                        graph[newRoomKey].neighbors.Add(graph[curRoomKey], "East");

                    }

                    UnityEngine.Debug.Log("Placing Random Door at Room: " + curRoomKey.x + " " + curRoomKey.y + " Connecting it to Room " + newRoomKey.x + " " + newRoomKey.y);
                    UnityEngine.Debug.Log("To cost: " + graph[curRoomKey].weight + " From Cost: " + graph[newRoomKey].weight);

                    // Decide whether to place another door or not based on probability
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
