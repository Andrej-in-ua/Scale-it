using GameTable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LinePathCreation : MonoBehaviour
{
    private GridManager _gridManager;
    private float _gridSize;

    private void Awake()
    {
        _gridManager = FindAnyObjectByType<GridManager>();
        _gridSize = _gridManager.GetGridSize();
    }

    public List<Vector3> FindPath(Vector2 startPoint, Vector2 endPoint)
    {
        var startGrid = CordToGridForLine(startPoint);
        var goalGrid = CordToGridForLine(endPoint);
        var goalNode = FindNearestFreeCell(goalGrid);

        var path = FindPathAStarMinTurns(startGrid, goalNode);

        if (goalNode != goalGrid) path.Add(GridToCordForLine(goalGrid));

        return path;
    }

    private List<Vector3> FindPathAStarMinTurns(Vector2Int start, Vector2Int goal)
    {
        var directions = new[]
        {
        Vector2Int.right,
        Vector2Int.left,
        Vector2Int.up,
        Vector2Int.down
        };

        var openSet = new PriorityQueue<(Vector2Int pos, Vector2Int? dir)>();
        var openSetHash = new HashSet<(Vector2Int, Vector2Int?)>();

        var gScore = new Dictionary<(Vector2Int, Vector2Int?), float>();
        var fScore = new Dictionary<(Vector2Int, Vector2Int?), float>();
        var cameFrom = new Dictionary<(Vector2Int, Vector2Int?), (Vector2Int, Vector2Int?)>();

        var startState = (start, (Vector2Int?)null);
        gScore[startState] = 0;
        fScore[startState] = Vector2Int.Distance(start, goal);
        openSet.Enqueue(startState, fScore[startState]);
        openSetHash.Add(startState);

        int iterationLimit = 10_000;
        int iterations = 0;

        while (openSet.Count > 0 && iterations < iterationLimit)
        {
            iterations++;

            var current = openSet.Dequeue();
            openSetHash.Remove(current);

            if (current.pos == goal)
                return ReconstructPathWithDir(cameFrom, current);

            foreach (var dir in directions)
            {
                var neighborPos = current.pos + dir;
                var neighborState = (neighborPos, (Vector2Int?)dir);

                if (neighborPos != goal && IsCellOccupied(neighborPos)) continue;

                bool isTurn = current.dir.HasValue && current.dir.Value != dir;
                float turnPenalty = isTurn ? 0.5f : 0.0f;
                float tentativeG = gScore[current] + 1 + turnPenalty;

                if (!gScore.ContainsKey(neighborState) || tentativeG < gScore[neighborState])
                {
                    cameFrom[neighborState] = current;
                    gScore[neighborState] = tentativeG;
                    fScore[neighborState] = tentativeG + Vector2Int.Distance(neighborPos, goal);

                    if (openSetHash.Add(neighborState))
                        openSet.Enqueue(neighborState, fScore[neighborState]);
                }
            }
        }

        return new List<Vector3>();
    }

    private List<Vector3> ReconstructPathWithDir(Dictionary<(Vector2Int, Vector2Int?), (Vector2Int, Vector2Int?)> cameFrom, (Vector2Int, Vector2Int?) current)
    {
        var path = new List<Vector3>();

        while (cameFrom.ContainsKey(current))
        {
            path.Add(GridToCordForLine(current.Item1));
            current = cameFrom[current];
        }

        path.Add(GridToCordForLine(current.Item1));
        path.Reverse();

        return path;
    }

    private Vector2Int FindNearestFreeCell(Vector2Int start)
    {
        if (!IsCellOccupied(start))
        {
            return start;
        }

        var queue = new Queue<Vector2Int>();
        var visited = new HashSet<Vector2Int>();

        queue.Enqueue(start);
        visited.Add(start);

        Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            foreach (Vector2Int dir in directions)
            {
                Vector2Int neighbor = current + dir;

                if (visited.Contains(neighbor)) continue;

                if (!IsCellOccupied(neighbor))
                {
                    return neighbor;
                }

                queue.Enqueue(neighbor);
                visited.Add(neighbor);
            }
        }

        return start;
    }

    private bool IsCellOccupied(Vector2Int gridPosition) =>
       _gridManager.IsOccupied(0, new Vector2Int(gridPosition.x + 2, gridPosition.y + 3), new Vector2Int(1, 1), 0);

    private Vector2Int CordToGridForLine(Vector2 pos)
    {
        return new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y)); ;
    }

    private Vector3 GridToCordForLine(Vector2Int gridPos)
    {
        Vector3 Pos = new Vector2((gridPos.x * _gridSize) + (_gridSize / 2), (gridPos.y * _gridSize) + (_gridSize / 2));

        return new Vector3(Pos.x / _gridSize, Pos.y / _gridSize, 0);
    }
}

public class PriorityQueue<T>
{
    private List<KeyValuePair<T, float>> elements = new List<KeyValuePair<T, float>>();

    public int Count => elements.Count;

    public void Enqueue(T item, float priority)
    {
        elements.Add(new KeyValuePair<T, float>(item, priority));
        elements = elements.OrderBy(e => e.Value).ToList();
    }

    public T Dequeue()
    {
        var bestItem = elements[0];
        elements.RemoveAt(0);
        return bestItem.Key;
    }
}
