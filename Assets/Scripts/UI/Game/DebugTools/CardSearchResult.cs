using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.DebugTools
{
    public class CardSearchResult : MonoBehaviour
    {
        [SerializeField] private TMP_Text _cardIdText, _cardNameText;
        
        public void Initialize(CardSpawner spawner, int cardId, string cardName)
        {
            _cardIdText.text = cardId.ToString();
            _cardNameText.text = cardName;
            
            GetComponent<Button>().onClick.AddListener(() => spawner.SpawnCard(cardId));
        }
    }
}