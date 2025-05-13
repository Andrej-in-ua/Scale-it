using GameTable;
using UI.Game.CardPreviews;
using UI.Game.Inventory;
using UnityEngine;

namespace UI.Game
{
    public interface IUICardFactory
    {
        UICardPreview CreateUICard(Transform parent, UIInventory _inventory, int cardId);
    }
}