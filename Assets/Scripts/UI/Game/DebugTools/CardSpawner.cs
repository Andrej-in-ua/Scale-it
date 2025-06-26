using System;
using TMPro;
using UnityEngine;

namespace UI.Game.DebugTools
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _IDCardDropdown;
        
        public event Action<int> OnCardSpawnRequested;

        public void SpawnCard()
        {
            Debug.Log("spawned");
            int cardId = _IDCardDropdown.value;
            OnCardSpawnRequested?.Invoke(cardId);
        }
    }
}

