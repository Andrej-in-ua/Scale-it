using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace View.GameTable
{
    // TODO: mb split it to specific services?
    public class GridManager
    {
        private readonly GridFactory _gridFactory;

        private Grid _grid;
        private Dictionary<Vector2Int, PlaceableReference> _occupiedExclusive;
        private Dictionary<Vector2Int, List<PlaceableReference>> _occupiedShared;

        private Dictionary<int, List<Vector2Int>> _objectsIndex;

        public GridManager(GridFactory gridFactory)
        {
            _gridFactory = gridFactory;
        }

        public void Construct()
        {
            _grid = _gridFactory.Create();

            _occupiedExclusive = new Dictionary<Vector2Int, PlaceableReference>();
            _occupiedShared = new Dictionary<Vector2Int, List<PlaceableReference>>();
            _objectsIndex = new Dictionary<int, List<Vector2Int>>();
        }

        public Vector3? PlaceOnNearestAvailablePosition(
            Vector3 worldPosition,
            PlaceableReference reference,
            out List<PlaceableReference> needRelocate
        )
        {
            return PlaceOnNearestAvailablePosition(WorldToCell(worldPosition, reference), reference, out needRelocate);
        }

        public Vector3? PlaceOnNearestAvailablePosition(
            Vector2Int startCellPosition,
            PlaceableReference reference,
            out List<PlaceableReference> needRelocate
        )
        {
            // Debug.Log("PlaceOnNearestAvailablePosition " + startCellPosition + " " + reference.Object);
            AssertConstructed();

            if (startCellPosition.x % reference.CellScale > 0 || startCellPosition.y % reference.CellScale > 0)
                Debug.LogWarning("Start cell position is not aligned with the grid for " + reference);

            var cellPosition = startCellPosition;

            if (!IsAllowToPlace(startCellPosition, reference))
            {
                var foundPosition = FindEmptyPlot(startCellPosition, reference);
                if (!foundPosition.HasValue)
                {
                    // Debug.Log("FindEmptyPlot NO RESULT " + reference.Object);
                    needRelocate = null;
                    return null;
                }

                // Debug.Log("FindEmptyPlot result " + foundPosition.Value + " " + reference.Object);
                cellPosition = foundPosition.Value;
            }

            needRelocate = PlaceOnGrid(cellPosition, reference);

            return CellToWorld(cellPosition);
        }

        private bool IsAllowToPlace(Vector2Int cellPosition, PlaceableReference reference)
        {
            foreach (var cell in reference.CellIterator(cellPosition))
            {
                if (_occupiedExclusive.ContainsKey(cell) && !_occupiedExclusive[cell].Object.Equals(reference.Object))
                {
                    return false;
                }
            }

            return true;
        }

        private Vector2Int? FindEmptyPlot(Vector2Int startCell, PlaceableReference reference)
        {
            Queue<Vector2Int> searchQueue = new Queue<Vector2Int>();
            HashSet<Vector2Int> visitedCells = new HashSet<Vector2Int>();

            searchQueue.Enqueue(startCell);
            visitedCells.Add(startCell);

            Vector2Int[] searchDirections =
            {
                Vector2Int.up * reference.CellScale,
                Vector2Int.right * reference.CellScale,
                Vector2Int.down * reference.CellScale,
                Vector2Int.left * reference.CellScale,
            };

            int searchDepth = 0;

            while (searchQueue.Count > 0 && searchDepth < (reference.MaxRelocateRange * reference.CellScale))
            {
                int currentLevelSize = searchQueue.Count;
                searchDepth++;

                for (int i = 0; i < currentLevelSize; i++)
                {
                    Vector2Int currentPos = searchQueue.Dequeue();

                    if (IsAllowToPlace(currentPos, reference))
                        return currentPos;

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

        private List<PlaceableReference> PlaceOnGrid(Vector2Int cellPosition, PlaceableReference reference)
        {
            Release(reference);

            return reference.ExclusiveOccupation
                ? OccupyExclusive(cellPosition, reference)
                : OccupyShare(cellPosition, reference);
        }

        private List<PlaceableReference> OccupyShare(Vector2Int cellPosition, PlaceableReference reference)
        {
            _objectsIndex[reference.Index] = new List<Vector2Int>();
            foreach (var cell in reference.CellIterator(cellPosition))
            {
                _objectsIndex[reference.Index].Add(cell);

                if (!_occupiedShared.ContainsKey(cell))
                    _occupiedShared[cell] = new List<PlaceableReference>();

                _occupiedShared[cell].Add(reference);
            }

            return null;
        }

        private List<PlaceableReference> OccupyExclusive(Vector2Int cellPosition, PlaceableReference reference)
        {
            var needRelocateIndex = new Dictionary<int, PlaceableReference>();

            _objectsIndex[reference.Index] = new List<Vector2Int>();
            foreach (var cell in reference.CellIterator(cellPosition))
            {
                _objectsIndex[reference.Index].Add(cell);
                _occupiedExclusive[cell] = reference;

                if (_occupiedShared.ContainsKey(cell))
                {
                    foreach (var sharedReference in _occupiedShared[cell])
                    {
                        if (!needRelocateIndex.ContainsKey(sharedReference.Index))
                            needRelocateIndex[sharedReference.Index] = sharedReference;
                    }
                }
            }

            return needRelocateIndex.Values.ToList();
        }

        public void Release(PlaceableReference reference)
        {
            if (!_objectsIndex.TryGetValue(reference.Index, out var objectCells))
                return;

            foreach (var cell in objectCells)
            {
                _occupiedExclusive.Remove(cell);

                if (_occupiedShared.ContainsKey(cell))
                {
                    foreach (var sharedReference in _occupiedShared[cell])
                    {
                        if (reference.Object.Equals(sharedReference.Object))
                        {
                            _occupiedShared[cell].Remove(sharedReference);
                        }
                    }
                }
            }

            _objectsIndex.Remove(reference.Index);
        }

        public Vector2Int WorldToCell(Vector3 worldPosition, PlaceableReference reference)
        {
            AssertConstructed();

            var cellPosition = _grid.WorldToCell(worldPosition);

            return new Vector2Int(
                cellPosition.x - (cellPosition.x % reference.CellScale),
                cellPosition.y - (cellPosition.y % reference.CellScale)
            );
        }

        public Vector3 CellToWorld(Vector2Int cellPosition)
        {
            AssertConstructed();

            return _grid.CellToWorld((Vector3Int)cellPosition) + (_grid.cellSize / 2);
        }

        private void AssertConstructed()
        {
            if (_grid == null)
                throw new System.Exception("GridManager is not constructed");
        }

        public void Destruct()
        {
            if (_grid != null)
            {
                _grid.gameObject.SetActive(false);
                Object.Destroy(_grid.gameObject);
                _grid = null;
            }

            _occupiedShared = null;
        }
    }
}