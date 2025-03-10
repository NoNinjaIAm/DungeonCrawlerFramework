using UnityEngine;
using TMPro;

public class DungeonUIManager : MonoBehaviour
{
    public TMP_Text aStarTime;
    public TMP_Text generatorTime;

    public DungeonGenerator generator;
    public FindShortestPath shortestPathFinder;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        shortestPathFinder.OnAStarFinished += OnAStarRan;
        generator.OnMazeGenerated += OnMazeGenerated;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMazeGenerated(float time)
    {
        generatorTime.text = "Generation Time: " + time + " ms";
    }

    void OnAStarRan(float time)
    {
        aStarTime.text = "Astar Time: " + time + " ms";
    }

    private void OnDestroy()
    {
        shortestPathFinder.OnAStarFinished -= OnAStarRan;
        generator.OnMazeGenerated -= OnMazeGenerated;
    }
}
