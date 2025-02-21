using UnityEngine;

public class PathManager : MonoBehaviour
{
    [SerializeField] FindShortestPath aStar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        aStar = GetComponent<FindShortestPath>();

        DungeonGenerator.OnMazeGenerated += OnMazeGenerated;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMazeGenerated()
    {
        if (aStar != null)
        {
            aStar.GetMazeGraphAndRun();
        }
    }

    private void OnDestroy()
    {
        DungeonGenerator.OnMazeGenerated -= OnMazeGenerated;
    }
}
