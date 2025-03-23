using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameTable
{
    public class GridManager : MonoBehaviour
    {
        [SerializeField] private float _maxSearchRange = 10;
        [SerializeField] private float _gridSize;

        private int _padding = 1;

        private Dictionary<Vector2Int, int> _grid = new Dictionary<Vector2Int, int>();
        private Dictionary<int, List<Vector2Int>> _objectInGrid = new Dictionary<int, List<Vector2Int>>();

        public Vector2? PlaceOnGrid(int objectID, Vector2 position, Vector2Int objectSize)
        {

            var gridPos = CordToGrid(position);

            if (IsOccupied(objectID, gridPos, objectSize, _padding, true))
            {
                var newGridPos = FindNearestAvailablePosition(objectID, gridPos, objectSize, _padding);
                if (!newGridPos.HasValue)
                {
                    return null;
                }

                gridPos = newGridPos.Value;
            }

            RemoveFromGrid(objectID);
            PlaceObjectOnGrid(objectID, gridPos, objectSize);
            return GridToCord(gridPos);
        }

        private void RemoveFromGrid(int objectID)
        {
            if (!_objectInGrid.ContainsKey(objectID))
                return;

            foreach (var cell in _objectInGrid[objectID])
            {
                _grid.Remove(cell);
            }

            _objectInGrid.Remove(objectID);
        }

        private Vector2Int? FindNearestAvailablePosition(
            int objectID,
            Vector2Int startGridPos,
            Vector2Int objectSize,
            int padding
        )
        {
            Queue<Vector2Int> searchQueue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visitedCells = new HashSet<Vector2Int>();

            searchQueue.Enqueue(startGridPos);
            visitedCells.Add(startGridPos);

            Vector2Int[] searchDirections = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };
            int searchDepth = 0;

            while (searchQueue.Count > 0 && searchDepth < _maxSearchRange)
            {
                int currentLevelSize = searchQueue.Count;
                searchDepth++;

                for (int i = 0; i < currentLevelSize; i++)
                {
                    Vector2Int currentPos = searchQueue.Dequeue();

                    if (!IsOccupied(objectID, currentPos, objectSize, padding, true))
                    {
                        return currentPos;
                    }

                    foreach (var direction in searchDirections)
                    {
                        Vector2Int nextPos = currentPos + direction;

                        if (!visitedCells.Contains(nextPos))
                        {
                            searchQueue.Enqueue(nextPos);
                            visitedCells.Add(nextPos);
                        }
                    }
                }
            }

            return null;
        }

        private bool IsOccupied(int objectID, Vector2Int gridPos, Vector2Int size, int padding, bool isCard)
        {
            Vector2Int cell = Vector2Int.zero;

            for (int i = gridPos.x - padding; i < gridPos.x + size.x + padding; i++)
            {
                for (int j = gridPos.y - padding; j < gridPos.y + size.y + padding; j++)
                {
                    if (isCard)
                    {
                        cell = new Vector2Int(i, j);
                    }
                    else
                    {
                        cell = new Vector2Int(i + 2, j + 3);
                    }

                    if (_grid.ContainsKey(cell) && _grid[cell] != objectID)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private void PlaceObjectOnGrid(int objectID, Vector2Int gridPos, Vector2 objectSize)
        {
            if (!_objectInGrid.ContainsKey(objectID))
                _objectInGrid[objectID] = new List<Vector2Int>();

            for (int i = gridPos.x; i < gridPos.x + objectSize.x; i++)
            {
                for (int j = gridPos.y; j < gridPos.y + objectSize.y; j++)
                {
                    Vector2Int cell = new Vector2Int(i, j);
                    _grid[cell] = objectID;
                    _objectInGrid[objectID].Add(cell);
                }
            }
        }

        public List<Vector3> FindPath(Vector2 A, Vector2 B)
        {
            Vector2Int start = CordToGridForLine(A);
            Vector2Int goal = CordToGridForLine(B);

            PriorityQueue<Vector2Int> openSet = new PriorityQueue<Vector2Int>();
            HashSet<Vector2Int> openSetHash = new HashSet<Vector2Int>();
            Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            Dictionary<Vector2Int, float> gScore = new Dictionary<Vector2Int, float>();
            Dictionary<Vector2Int, float> fScore = new Dictionary<Vector2Int, float>();

            openSet.Enqueue(start, 0);
            openSetHash.Add(start);
            gScore[start] = 0;
            fScore[start] = Heuristic(start, goal);

            Vector2Int[] directions = {
        Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down,
    };

            int iterationLimit = 10000;
            int iterations = 0;

            while (openSet.Count > 0)
            {
                if (iterations++ > iterationLimit)
                {
                    Debug.LogWarning("Caput!");
                    return new List<Vector3>();
                }

                Vector2Int current = openSet.Dequeue();
                openSetHash.Remove(current);

                if (current == goal)
                    return ReconstructPath(cameFrom, current);

                foreach (Vector2Int direction in directions)
                {
                    Vector2Int neighbor = current + direction;

                    if (IsOccupied(0, neighbor, new Vector2Int(1, 1), 0, false))
                    {
                        // Debug.Log("ok");
                        continue;
                    }

                    float tentativeGScore = gScore[current] + 1;

                    if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeGScore;
                        fScore[neighbor] = tentativeGScore + Heuristic(neighbor, goal);

                        if (!openSetHash.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                            openSetHash.Add(neighbor);
                        }
                    }
                }
            }

            return new List<Vector3>();
        }

        float Heuristic(Vector2Int a, Vector2Int b)
        {
            return Vector2Int.Distance(a, b);
        }

        private List<Vector3> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            List<Vector3> path = new List<Vector3> { GridToCordForLine(current) };

            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                path.Add(GridToCordForLine(current));
            }

            path.Reverse();
            return path;
        }

        private Vector2Int CordToGridForLine(Vector2 pos)
        {
            Vector2Int gridPos = new Vector2Int(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));

            return gridPos;
        }

        private Vector2Int CordToGrid(Vector2 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / _gridSize),
                Mathf.FloorToInt(position.y / _gridSize)
            );
        }

        private Vector2 GridToCord(Vector2Int gridPos)
        {
            return new Vector2(
                (gridPos.x * _gridSize) + (_gridSize / 2),
                (gridPos.y * _gridSize) + (_gridSize / 2)
            );
        }

        private Vector3 GridToCordForLine(Vector2Int gridPos)
        {
            float cellSize = 1f;
            return new Vector3(gridPos.x * cellSize + cellSize / 2, gridPos.y * cellSize + cellSize / 2, 0);
        }
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

    public bool Contains(T item)
    {
        return elements.Any(e => EqualityComparer<T>.Default.Equals(e.Key, item));
    }
}
