using System;
using TMPro;
using UnityEngine;

namespace UI.Game.DebugTools
{
    public class CardSpawner : MonoBehaviour
    {
        public GameObject CardScrollView;
        public Transform Content;
        public TMP_InputField InputField;
        
        public event Action<int> OnCardSpawnRequested;
        
        public void SpawnCard(int cardId)
        {
            OnCardSpawnRequested?.Invoke(cardId);
        }
    }
}

