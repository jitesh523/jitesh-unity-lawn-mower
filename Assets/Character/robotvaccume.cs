using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotVacuum : MonoBehaviour
{
    public int gridSize = 20;
    public GameObject obstaclePrefab;
    public GameObject robotPrefab;
    private Vector2Int[] obstacles = {
        new Vector2Int(2, 2), new Vector2Int(2, 3), new Vector2Int(3, 2), new Vector2Int(3, 3),
        new Vector2Int(5, 5), new Vector2Int(5, 6), new Vector2Int(6, 5), new Vector2Int(6, 6),
        new Vector2Int(12, 12), new Vector2Int(12, 13), new Vector2Int(13, 12), new Vector2Int(13, 13),
        new Vector2Int(15, 15), new Vector2Int(15, 16), new Vector2Int(16, 15), new Vector2Int(16, 16)
    };
    private Vector2Int vacuumPosition = new Vector2Int(3, 7);
    private List<Vector2Int> path = new List<Vector2Int>();
    private GameObject robotInstance;

    void Start()
    {
        GenerateGrid();
        StartCoroutine(MoveRobot());
    }

    void GenerateGrid()
    {
        foreach (Vector2Int obstacle in obstacles)
        {
            Instantiate(obstaclePrefab, new Vector3(obstacle.x, 0, obstacle.y), Quaternion.identity);
        }
        robotInstance = Instantiate(robotPrefab, new Vector3(vacuumPosition.x, 0, vacuumPosition.y), Quaternion.identity);
    }

    IEnumerator MoveRobot()
    {
        List<Vector2Int> path = AStar(vacuumPosition, new Vector2Int(18, 18)); // Example target

        foreach (Vector2Int step in path)
        {
            vacuumPosition = step;
            robotInstance.transform.position = new Vector3(vacuumPosition.x, 0, vacuumPosition.y);
            yield return new WaitForSeconds(0.1f);
        }
    }

    List<Vector2Int> AStar(Vector2Int start, Vector2Int goal)
    {
        List<Vector2Int> openSet = new List<Vector2Int> { start };
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        Dictionary<Vector2Int, int> gScore = new Dictionary<Vector2Int, int> { { start, 0 } };
        Dictionary<Vector2Int, int> fScore = new Dictionary<Vector2Int, int> { { start, Heuristic(start, goal) } };

        while (openSet.Count > 0)
        {
            openSet.Sort((a, b) => fScore[a] - fScore[b]);
            Vector2Int current = openSet[0];
            openSet.RemoveAt(0);

            if (current == goal)
            {
                List<Vector2Int> path = new List<Vector2Int>();
                while (cameFrom.ContainsKey(current))
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (Vector2Int neighbor in GetNeighbors(current))
            {
                int tentativeGScore = gScore[current] + 1;
                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeGScore;
                    fScore[neighbor] = gScore[neighbor] + Heuristic(neighbor, goal);
                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return new List<Vector2Int>(); // Return empty path if no path found
    }

    int Heuristic(Vector2Int a, Vector2Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    List<Vector2Int> GetNeighbors(Vector2Int current)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>
        {
            new Vector2Int(current.x + 1, current.y),
            new Vector2Int(current.x - 1, current.y),
            new Vector2Int(current.x, current.y + 1),
            new Vector2Int(current.x, current.y - 1)
        };

        neighbors.RemoveAll(n => !IsValidPosition(n));
        return neighbors;
    }

    bool IsValidPosition(Vector2Int position)
    {
        if (position.x < 0 || position.x >= gridSize || position.y < 0 || position.y >= gridSize)
            return false;

        foreach (Vector2Int obstacle in obstacles)
        {
            if (position == obstacle)
                return false;
        }

        return true;
    }
}
