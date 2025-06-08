using System;
using Services.Input;
using TMPro;
using UnityEngine;

namespace UI.Game.CardPreviews
{
    public class UICardPreview : MonoBehaviour, IDraggable
    {
        public TMP_Text Name;

        public GameObject CardsCountWindow;
        
        public int Priority => 1;

        [NonSerialized] public int CardId;
    }
}