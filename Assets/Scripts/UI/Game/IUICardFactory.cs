using GameTable;
using UI.Game.CardPreviews;
using UnityEngine;

namespace UI.Game
{
    public interface IUICardFactory
    {
        UICardPreview CreateUICard(Transform parent, string name);
    }
}