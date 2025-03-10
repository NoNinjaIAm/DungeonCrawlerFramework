using TMPro;
using UnityEngine;

public class GlobalOptions : MonoBehaviour
{
    public static GlobalOptions Instance { get; private set; }

    public int xDungeonSize { get; private set; }
    public int yDungeonSize { get; private set; }
    public float extraDoorProb { get; private set; }
    public float obsSpawnProb { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Optional: Keeps the instance across scenes

            SetOptions();
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
