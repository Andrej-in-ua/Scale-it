using DefaultNamespace;
using GameTable;
using Services;
using UnityEngine;

namespace UI.Game
{
    public class UICardFactory : IUICardFactory
    {
        private readonly IAssetProviderService _assetProviderService;
        private DragCard _lastSpawnedCard;

        public UICardFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }

        public DragCard CreateUICard(Transform parent)
        {
            Vector3 worldPos = parent.position;
            Vector2 localPos;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parent.GetComponent<RectTransform>(),
                RectTransformUtility.WorldToScreenPoint(null, worldPos),
                null,
                out localPos
            );

            var cardPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.CardPath);
            
            GameObject card = Object.Instantiate(cardPrefab, parent);

            /*card.transform.SetParent(parent, false);
            card.transform.SetAsLastSibling();

            card.GetComponent<RectTransform>().anchoredPosition = localPos;

            card.GetComponent<DragCard>().FollowCursorWithoutClick();

            _lastSpawnedCard = card.GetComponent<DragCard>();*/

            return _lastSpawnedCard;
        }

        public Transform CreateInventory()
        {
            return null;
        }
    }
}