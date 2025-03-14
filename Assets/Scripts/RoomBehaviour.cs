using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Video;
using static DungeonGenerator;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls; // 0 - North 1 -South 2 - East 3- West
    public GameObject[] doors;
    public GameObject[] doorVisualizers;
    public GameObject floor;
    public GameObject fillerWalls;
    public GameObject sucessObject;
    public GameObject obstacles;
    public GameObject weightVisual;
    public GameObject shortestPathHighlight;
    public Vector2Int myGridPosition;
    
    public Material weightedRoomMaterial;
    public Material startingRoomMaterial;
    public Material endRoomMaterial;

    private void Start()
    {
        Debug.Log("Room" + myGridPosition.x + "-" + myGridPosition.y + "is ready to listen for shortest path");

        //gameObject.SetActive(false);    

        FindShortestPath.Instance.OnPathUpdated += OnPathUpdated;
        if (!GlobalOptions.Instance.showObjects)
        {
            foreach (GameObject wall in walls)
            {
                wall.SetActive(false);
            }
            fillerWalls.SetActive(false);
            floor.SetActive(false);
        }

        if (GlobalOptions.Instance.showVisualizer)
        {
            weightVisual.SetActive(true);

        }
    }

    public void UpdateDoorways(List<string> directions)
    {
        //for (int i = 0; i < status.Length; i++)
        //{
        //    doors[i].SetActive(status[i]);
        //    walls[i].SetActive(!status[i]);
        //}
        
        if(directions.Contains("North")) SetDoorStatus(true, 0);
        else SetDoorStatus(false, 0);

        if (directions.Contains("South")) SetDoorStatus(true, 1);
        else SetDoorStatus(false, 1);

        if (directions.Contains("East")) SetDoorStatus(true, 2);
        else SetDoorStatus(false, 2);

        if (directions.Contains("West")) SetDoorStatus(true, 3);
        else SetDoorStatus(false, 3);
    }

    public void AddObstacles(bool addObstacles)
    {
        
        if (addObstacles && GlobalOptions.Instance.showVisualizer)
        {
            weightVisual.GetComponent<MeshRenderer>().material = weightedRoomMaterial;
        }

        // Return here if objects won't be rendered anyway
        if (!GlobalOptions.Instance.showObjects) return;

        obstacles.SetActive(addObstacles);
    }

    public void MakeEndRoom()
    {
        if (GlobalOptions.Instance.showVisualizer)
        {
            weightVisual.GetComponent<MeshRenderer>().material = endRoomMaterial;
        }

        if (!GlobalOptions.Instance.showObjects) return;
        sucessObject.SetActive(true);
    }

    public void MakeStartRoom()
    {
        if (!GlobalOptions.Instance.showVisualizer) return;
        weightVisual.GetComponent<MeshRenderer>().material = startingRoomMaterial;
    }

    private void SetDoorStatus(bool value, int index)
    {
        if (GlobalOptions.Instance.showVisualizer)
        {
            doorVisualizers[index].SetActive(value);
        }

        if (!GlobalOptions.Instance.showObjects) return;
        doors[index].SetActive(value);
        walls[index].SetActive(!value);
    }

    private void OnPathUpdated(List<Room> path)
    {
        if (!GlobalOptions.Instance.showVisualizer) return;

        bool inPath = false;
        foreach (var room in path)
        {
            if (room.position == myGridPosition)
            {
                inPath = true;
                break;
            }
        }

        if (inPath)
        {
            Debug.Log("Room" + myGridPosition.x + "-" + myGridPosition.y + "IS IN THE PATH");
            shortestPathHighlight.SetActive(true);
        }
        else
        {
            Debug.Log("Room" + myGridPosition.x + "-" + myGridPosition.y + "IS NOT IN THE PATH");
            shortestPathHighlight.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        FindShortestPath.Instance.OnPathUpdated -= OnPathUpdated;
    }
}
