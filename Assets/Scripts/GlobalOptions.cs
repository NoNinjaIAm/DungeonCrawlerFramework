using TMPro;
using UnityEngine;

public class GlobalOptions : MonoBehaviour
{
    public static GlobalOptions Instance { get; private set; }

    public int xDungeonSize = 5;
    public int yDungeonSize = 5;
    public float extraDoorProb = .5f;
    public float obsSpawnProb = .5f;
    public float obsWeight = 3f;

    public bool renderRooms;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the instance across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetOptions(int xSize = 5, int ySize = 5, float doorProb = 0.5f, float obsProb = 0.5f)
    {
        xDungeonSize = xSize;
        yDungeonSize = ySize;
        extraDoorProb = doorProb;
        obsSpawnProb = obsProb;
    }
}
