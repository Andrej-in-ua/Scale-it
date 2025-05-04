using GameTable;
using UnityEngine;

namespace UI.Game
{
    public interface IUICardFactory
    {
        Transform CreateInventory();
        DragCard CreateUICard(Transform parent);
    }
}