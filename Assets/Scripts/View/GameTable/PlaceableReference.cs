using System.Collections.Generic;
using UnityEngine;

namespace View.GameTable
{
    public record PlaceableReference
    {
        public Object Object;

        public int Index => Object.GetInstanceID();

        public Vector2Int ObjectSize;

        // Padding between objects with ExclusiveOccupation=true
        public int Padding = 0;

        public int MaxRelocateRange = 30;

        // Defines how many grid cells correspond to one unit of the object size.
        // Example:
        // - If CellScale is 2 and the object size is 2x2 cells, the object will occupy a 4x4 area in the grid.
        // - If CellScale is 1, the object size directly matches the grid cell size.
        // Directly affects ObjectSize, Padding and MaxRelocateRange.
        // Indirectly affects FindEmptyPlot: the object will only be placed in cells where the 
        // grid cell index modulo CellScale is zero.
        public int CellScale = 1;

        // Cant be placed any other objects on the same cell
        public bool ExclusiveOccupation = true;

        public Vector2Int PlotSize => new Vector2Int(
            (ObjectSize.x + (Padding * 2)) * CellScale,
            (ObjectSize.y + (Padding * 2)) * CellScale
        );

        public IEnumerable<Vector2Int> CellIterator(Vector2Int plotCenter = default)
        {
            var minCell = new Vector2Int(
                plotCenter.x - ((PlotSize.x / 2) - (PlotSize.x % 2)),
                plotCenter.y - ((PlotSize.y / 2) - (PlotSize.y % 2))
            );
            var maxCell = new Vector2Int(
                plotCenter.x + ((PlotSize.x / 2) + (PlotSize.x % 2)),
                plotCenter.y + ((PlotSize.y / 2) + (PlotSize.y % 2))
            );

            for (var i = minCell.x; i <= maxCell.x; i++)
            {
                for (var j = minCell.y; j <= maxCell.y; j++)
                {
                    yield return new Vector2Int(i, j);
                }
            }
        }
    }
}