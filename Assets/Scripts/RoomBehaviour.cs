using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public GameObject[] walls; // 0 - North 1 -South 2 - East 3- West
    public GameObject[] doors;

    public void UpdateRoom(List<string> directions)
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

    private void SetDoorStatus(bool value, int index)
    {
        doors[index].SetActive(value);
        walls[index].SetActive(!value);
    }
}
