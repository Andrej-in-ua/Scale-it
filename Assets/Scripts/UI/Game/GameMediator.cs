using System.Collections.Generic;
using GameTable;
using UnityEngine;

namespace UI.Game
{
    public class GameMediator : IGameMediator
    {
        private readonly IUICardFactory _factory;
        private Transform _inventory;
        
        private List<DragCard> _dragCards;

        public GameMediator(IUICardFactory factory)
        {
            _factory = factory;
        }

        public void ConstructUI()
        {
           _inventory = _factory.CreateInventory();

           for (int i = 0; i < 5; i++)
           {
               Transform inventoryPanel = _inventory.GetChild(0).transform;
               var card = _factory.CreateUICard(inventoryPanel);
               
              ConstructLinePortsForCard(card.transform, 4,4,2);

              _factory.CreateCardName(card.transform);
           }
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
                    _factory.CreateLinePort(parent, portConfigs[i].path);
                }
            }
        }
    }

    public interface IGameMediator
    {
        public void ConstructUI();
    }
}