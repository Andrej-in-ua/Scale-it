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
            {
                return path;
            }

            var optimizedPath = new List<Vector3> { path[0] };
            int currentIndex = 0;

            while (currentIndex < path.Count - 1)
            {
                Vector3 startPoint = optimizedPath.Last();

                if (path.Count - currentIndex <= 3)
                {
                    optimizedPath.Add(path[currentIndex + 1]);
                    currentIndex++;
                    continue;
                }

                int bestSegmentEnd = FindLongestStaircaseSegment(path, currentIndex);

                if (bestSegmentEnd > currentIndex + 3 && TryAddOptimizedSegment(startPoint, path[bestSegmentEnd], optimizedPath))
                {
                    currentIndex = bestSegmentEnd;
                }
                else
                {
                    optimizedPath.Add(path[currentIndex + 1]);
                    currentIndex++;
                }
            }

            return optimizedPath;
        }

        private bool TryAddOptimizedSegment(Vector3 startPoint, Vector3 endPoint, List<Vector3> optimizedPath)
        {
            Vector3 midPoint = new Vector3(endPoint.x, startPoint.y, startPoint.z);

            if (IsSegmentClear(startPoint, midPoint) && IsSegmentClear(midPoint, endPoint))
            {
                optimizedPath.Add(midPoint);
                optimizedPath.Add(endPoint);
                return true;
            }

            return false;
        }

        private int FindLongestStaircaseSegment(List<Vector3> path, int startIndex)
        {
            int bestEnd = startIndex + 1;
            bool isXMovement = Mathf.Abs(path[startIndex + 1].x - path[startIndex].x) > 0.1f;

            for (int j = startIndex + 1; j < path.Count; j++)
            {
                Vector3 delta = path[j] - path[j - 1];
                bool shouldBeXMovement = (j - startIndex) % 2 == 1 ? isXMovement : !isXMovement;

                bool isValidMove = shouldBeXMovement
                    ? Mathf.Abs(delta.x) > 0.1f && Mathf.Abs(delta.y) < 0.1f
                    : Mathf.Abs(delta.y) > 0.1f && Mathf.Abs(delta.x) < 0.1f;

                if (!isValidMove) break;

                bestEnd = j;
            }

            return (bestEnd - startIndex >= 4) ? bestEnd : startIndex + 1;
        }

        private bool IsSegmentClear(Vector3 start, Vector3 end)
        {
            Vector2Int startGrid = new Vector2Int((int)start.x, (int)start.y);
            Vector2Int endGrid = new Vector2Int((int)end.x, (int)end.y);

            int steps = Mathf.Max(Mathf.Abs(endGrid.x - startGrid.x), Mathf.Abs(endGrid.y - startGrid.y));

            for (int i = 1; i < steps; i++)
            {
                float t = (float)i / steps;
                Vector2 checkPoint = Vector2.Lerp(startGrid, endGrid, t);
                Vector2Int gridPoint = new Vector2Int(Mathf.RoundToInt(checkPoint.x), Mathf.RoundToInt(checkPoint.y));

                if (IsCellOccupied(gridPoint))
                {
                    return false;
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
