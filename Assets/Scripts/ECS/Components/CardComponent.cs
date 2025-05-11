using Common.Collection;
using DeckManager;
using ECS.Enums;
using Unity.Collections;
using Unity.Entities;

namespace ECS.Components
{
    public struct CardComponent : IComponentData
    {
        public int CardID;
        public int RecipeID;

        public float ProductionState;

        [NativeDisableParallelForRestriction] public NativeHashMap<int, int> ResourcesStored;

        [NativeDisableParallelForRestriction] public NativeHashMap<int, int> ResourcesLock;

        public Card Card => Deck.Instance.cards[CardID];
        public TagDictionary Tags => Card.tags;
        public Recipe ActiveRecipe => Card.recipes[RecipeID];

        public CardComponent(int cardID) : this(Deck.Instance.cards[cardID])
        {
        }

        public CardComponent(Card cardConfig)
        {
            CardID = cardConfig.cardID;
            RecipeID = -1;

            ProductionState = -1f;

            var initialCapacity = (cardConfig.inputLinkCount + cardConfig.outputLinkCount) * 2;

            ResourcesStored = new NativeHashMap<int, int>(initialCapacity, Allocator.Persistent);
            ResourcesLock = new NativeHashMap<int, int>(cardConfig.outputLinkCount * 2,Allocator.Persistent
            );
        }
    }
}