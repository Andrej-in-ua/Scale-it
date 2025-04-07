using System.Collections.Generic;
using System.Linq;
using System.Net;
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
            {
                return;
            }

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
            {
                _objectInGrid[objectID] = new List<Vector2Int>();
            }

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

        public List<Vector3> FindPath(Vector2 startPoint, Vector2 endPoint)
        {
            Vector2Int startGrid = CordToGridForLine(startPoint);
            Vector2Int goalGrid = CordToGridForLine(endPoint);

            Vector2Int startNode = FindNearestFreeCell(startGrid);
            Vector2Int goalNode = FindNearestFreeCell(goalGrid);

            var path = FindPathAStar(startNode, goalNode);

            path = OptimizePath(path);

            if (startNode != startGrid)
            {
                path.Insert(0, GridToCordForLine(startGrid));
            }

            if (goalNode != goalGrid)
            {
                path.Add(GridToCordForLine(goalGrid));
            }

            return path;
        }

        private List<Vector3> FindPathAStar(Vector2Int start, Vector2Int goal)
        {
            var openSet = new PriorityQueue<Vector2Int>();
            var openSetHash = new HashSet<Vector2Int>();
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var gScore = new Dictionary<Vector2Int, float>();
            var fScore = new Dictionary<Vector2Int, float>();

            InitializeSearch(start, goal, openSet, openSetHash, gScore, fScore);

            Vector2Int[] directions = { Vector2Int.right, Vector2Int.left, Vector2Int.up, Vector2Int.down };

            while (openSet.Count > 0)
            {
                Vector2Int current = openSet.Dequeue();
                openSetHash.Remove(current);

                if (current == goal)
                {
                    return ReconstructPath(cameFrom, current);
                }

                ExploreNeighbors(current, goal, directions, openSet, openSetHash, cameFrom, gScore, fScore);
            }

            return new List<Vector3>();
        }

        private void InitializeSearch(Vector2Int start, Vector2Int goal,
            PriorityQueue<Vector2Int> openSet, HashSet<Vector2Int> openSetHash,
            Dictionary<Vector2Int, float> gScore, Dictionary<Vector2Int, float> fScore)
        {
            openSet.Enqueue(start, 0);
            openSetHash.Add(start);
            gScore[start] = 0;
            fScore[start] = Vector2Int.Distance(start, goal);
        }

        private void ExploreNeighbors(Vector2Int current, Vector2Int goal, Vector2Int[] directions,
            PriorityQueue<Vector2Int> openSet, HashSet<Vector2Int> openSetHash,
            Dictionary<Vector2Int, Vector2Int> cameFrom, Dictionary<Vector2Int, float> gScore,
            Dictionary<Vector2Int, float> fScore)
        {
            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighbor = current + direction;

                if (IsCellOccupied(neighbor))
                {
                    continue;
                }

                float tentativeGScore = gScore[current] + 1;

                if (!gScore.ContainsKey(neighbor) || tentativeGScore < gScore[neighbor])
                {
                    UpdateNode(neighbor, current, tentativeGScore, goal,
                        openSet, openSetHash, cameFrom, gScore, fScore);
                }
            }
        }

        private void UpdateNode(Vector2Int neighbor, Vector2Int current, float tentativeGScore, Vector2Int goal,
            PriorityQueue<Vector2Int> openSet, HashSet<Vector2Int> openSetHash,
            Dictionary<Vector2Int, Vector2Int> cameFrom, Dictionary<Vector2Int, float> gScore,
            Dictionary<Vector2Int, float> fScore)
        {
            cameFrom[neighbor] = current;
            gScore[neighbor] = tentativeGScore;
            fScore[neighbor] = tentativeGScore + Vector2Int.Distance(neighbor, goal);

            if (!openSetHash.Contains(neighbor))
            {
                openSet.Enqueue(neighbor, fScore[neighbor]);
                openSetHash.Add(neighbor);
            }
        }

        private List<Vector3> OptimizePath(List<Vector3> path)
        {
            if (path == null || path.Count < 3)
                return path;

            List<Vector3> optimizedPath = new List<Vector3>();
            optimizedPath.Add(path[0]);

            int i = 0;
            while (i < path.Count - 1)
            {
                Vector3 startPoint = optimizedPath[optimizedPath.Count - 1];
                int bestEndIndex = i + 1;
                bool optimized = false;
                Vector3? bestMidPoint = null;
                Vector3? bestEndPoint = null;
                float shortestDistance = float.MaxValue;

                for (int lookahead = Mathf.Min(path.Count, path.Count - i - 1); lookahead >= 2; lookahead--)
                {
                    int currentEnd = i + lookahead;
                    if (currentEnd >= path.Count)
                        continue;

                    Vector3 endPoint = path[currentEnd];
                    float totalX = endPoint.x - startPoint.x;
                    float totalY = endPoint.y - startPoint.y;

                    if (Mathf.Abs(totalX) > 0.1f && Mathf.Abs(totalY) > 0.1f)
                    {
                        Vector3[] midVariants = new Vector3[]
                        {
                    new Vector3(endPoint.x, startPoint.y, startPoint.z),
                    new Vector3(startPoint.x, endPoint.y, startPoint.z)
                        };

                        foreach (var midPoint in midVariants)
                        {
                            if (IsPathCompletelyClear(startPoint, midPoint) &&
                                IsPathCompletelyClear(midPoint, endPoint))
                            {
                                float distance = Vector3.Distance(startPoint, midPoint) + Vector3.Distance(midPoint, endPoint);
                                if (distance < shortestDistance)
                                {
                                    bestMidPoint = midPoint;
                                    bestEndPoint = endPoint;
                                    shortestDistance = distance;
                                    bestEndIndex = currentEnd;
                                    optimized = true;
                                }
                            }
                        }

                        if (optimized)
                            break;
                    }
                }

                if (optimized && bestMidPoint.HasValue && bestEndPoint.HasValue)
                {
                    optimizedPath.Add(bestMidPoint.Value);
                    optimizedPath.Add(bestEndPoint.Value);
                    i = bestEndIndex;
                }
                else
                {
                    optimizedPath.Add(path[i + 1]);
                    i++;
                }
            }

            return optimizedPath;
        }

        private bool IsPathCompletelyClear(Vector3 start, Vector3 end)
        {
            Vector2 startGrid = new Vector2(start.x, start.y);
            Vector2 endGrid = new Vector2(end.x, end.y);
            int steps = Mathf.CeilToInt(Vector2.Distance(startGrid, endGrid) * 4);

            bool useBuffer = Vector2.Distance(startGrid, endGrid) <= 3f;

            for (int i = 0; i <= steps; i++)
            {
                float t = (float)i / steps;
                Vector2 point = Vector2.Lerp(startGrid, endGrid, t);
                Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(point.x), Mathf.FloorToInt(point.y));

                if (IsCellOccupied(gridPos))
                    return false;

                if (useBuffer)
                {
                    Vector2 direction = (endGrid - startGrid).normalized;
                    Vector2 perp = new Vector2(-direction.y, direction.x);

                    for (float offset = -0.01f; offset <= 0.01f; offset += 0.01f)
                    {
                        Vector2 sidePoint = point + perp * offset;
                        Vector2Int sideGrid = new Vector2Int(Mathf.FloorToInt(sidePoint.x), Mathf.FloorToInt(sidePoint.y));
                        if (IsCellOccupied(sideGrid))
                            return false;
                    }
                }
            }

            return true;
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

        private List<Vector3> ReconstructPath(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int current)
        {
            var path = new List<Vector3>();

            while (cameFrom.ContainsKey(current))
            {
                path.Add(GridToCordForLine(current));
                current = cameFrom[current];
            }

            path.Add(GridToCordForLine(current));
            path.Reverse();

            return path;
        }

        private bool IsCellOccupied(Vector2Int gridPosition) =>
            IsOccupied(0, gridPosition, new Vector2Int(1, 1), 0, false);

        private Vector2Int CordToGrid(Vector2 position)
        {
            return new Vector2Int(Mathf.FloorToInt(position.x / _gridSize), Mathf.FloorToInt(position.y / _gridSize));
        }

        private Vector2 GridToCord(Vector2Int gridPos)
        {
            return new Vector2((gridPos.x * _gridSize) + (_gridSize / 2), (gridPos.y * _gridSize) + (_gridSize / 2));
        }
        private Vector2Int CordToGridForLine(Vector2 pos)
        {
            Vector2Int gridPos = new Vector2Int(Mathf.FloorToInt(pos.x), Mathf.FloorToInt(pos.y));

            return gridPos;
        }

        private Vector3 GridToCordForLine(Vector2Int gridPos)
        {
            Vector3 Pos = GridToCord(gridPos);

            return new Vector3(Pos.x / _gridSize, Pos.y / _gridSize, 0);
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
