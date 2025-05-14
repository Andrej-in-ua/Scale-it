using GameTable;
using UI.Game.Inventory;
using UnityEngine;

namespace UI.Game
{
    public interface IUIGameFactory
    {
        UIInventory CreateInventory();
        
        CardSpawner CreateCardSpawner(Transform parent);
    }
}