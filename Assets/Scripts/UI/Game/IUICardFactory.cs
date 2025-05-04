using GameTable;
using UnityEngine;

namespace UI.Game
{
    public interface IUICardFactory
    {
        Transform CreateInventory();
        CardView CreateUICard(Transform parent);
    }
}