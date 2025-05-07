using TMPro;
using UnityEngine;

namespace GameTable
{
    public class CardSpawner : MonoBehaviour
    {
        [SerializeField] private GameObject _parentCanvasForCard;
        [SerializeField] private TMP_Dropdown _IDCardDropdown;
        [SerializeField] private GameObject[] _cards;


       

      /* public void DropCard()
        {
            if (_lastSpawnedCard != null)
            {
                _lastSpawnedCard.StopFollowingCursor();
                _lastSpawnedCard = null;
            }
        }*/
    }
}

