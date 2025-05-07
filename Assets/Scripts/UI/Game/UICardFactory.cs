using Services;
using UI.Game.CardPreviews;
using UnityEngine;

namespace UI.Game
{
    public class UICardFactory : IUICardFactory
    {
        private readonly IAssetProviderService _assetProviderService;

        public UICardFactory(IAssetProviderService assetProviderService)
        {
            _assetProviderService = assetProviderService;
        }

        public UICardPreview CreateUICard(Transform parent, string name)
        {
            var cardPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(Constants.CardPath);
            UICardPreview card = Object.Instantiate(cardPrefab, parent).GetComponent<UICardPreview>();
            
            card.name = name;
            card.Name.text = name;

           // ConstructLinePortsForCard(card.transform, 4, 4, 2);

            return card;
        }

        // TODO: MB move it to self factory?
      

        public Transform CreateLinePort(Transform parent, string inputPath)
        {
            GameObject linePortPrefab = _assetProviderService.LoadAssetFromResources<GameObject>(inputPath).gameObject;
            GameObject linePort = Object.Instantiate(linePortPrefab, parent);

            return linePort.GetComponent<Transform>();
        }

        private void ConstructLinePortsForCard(Transform card, int inputsCount, int outputsCount, int modifiersCount)
        {
            (int count, string path)[] portConfigs =
            {
                (inputsCount, Constants.InputPath),
                (outputsCount, Constants.OutputPath),
                (modifiersCount, Constants.ModifierPath)
            };

            for (int i = 0; i < portConfigs.Length; i++)
            {
                Transform parent = card.GetChild(i);

                for (int j = 0; j < portConfigs[i].count; j++)
                {
                    CreateLinePort(parent, portConfigs[i].path);
                }
            }
        }
    }
}