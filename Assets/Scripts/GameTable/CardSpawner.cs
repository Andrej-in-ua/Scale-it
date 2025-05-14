using System;
using TMPro;
using UnityEngine;

namespace GameTable
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _IDCardDropdown;
        
        public event Action<int> OnCardSpawnRequested;

        public void SpawnCard()
        {
            int cardId = _IDCardDropdown.value;
            OnCardSpawnRequested?.Invoke(cardId);
        }
    }
}

