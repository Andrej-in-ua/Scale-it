using GameTable;
using UnityEngine;

namespace UI.Game
{
    public interface IUICardFactory
    {
        Transform CreateInventory();
        Transform CreateUICard(Transform parent);
        
        Transform CreateLinePort(Transform parent, string inputPath);

        Transform CreateCardName(Transform parent);
    }
}