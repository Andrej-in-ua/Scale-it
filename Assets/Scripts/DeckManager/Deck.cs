using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Common.Collection;

namespace DeckManager
{
    [System.Serializable]
    public record RawDeck
    {
        public DeckMeta meta;
        public List<CardRaw> cards;
    }

    [System.Serializable]
    public record Deck
    {
        public DeckMeta meta;
        public Dictionary<int, Card> cards;

        public static Deck Instance { get; private set; }
        
        public static void SetAsGlobal(Deck deck)
        {
            Instance = deck;
        }

        public Deck(RawDeck raw)
        {
            meta = raw.meta;
            cards = raw.cards.ToDictionary(card => card.cardID, card => new Card(card));
        }
    }

    [System.Serializable]
    public record DeckMeta
    {
        public string name;
        public string version;
    }

    [System.Serializable]
    public record CardRaw
    {
        public int cardID;
        public string name;
        public List<string> tags;
        public int stackSize;
        public List<RecipeRaw> recipes;
    }

    [System.Serializable]
    public record Card
    {
        public int cardID;
        public string name;
        public TagDictionary tags;
        public int stackSize;

        public ImmutableDictionary<int, Recipe> recipes;
        public int modifierLinkCount;
        public int inputLinkCount;
        public int outputLinkCount;

        public Card(CardRaw raw)
        {
            cardID = raw.cardID;
            name = raw.name;
            tags = new TagDictionary(raw.tags);
            stackSize = raw.stackSize;

            recipes = raw.recipes?.ToImmutableDictionary(recipe => recipe.recipeID, recipe => new Recipe(recipe))
                      ?? ImmutableDictionary<int, Recipe>.Empty;

            modifierLinkCount = raw.recipes?.Max(recipe => recipe.modifiers?.Count() ?? 0) ?? 0;
            inputLinkCount = raw.recipes?.Max(recipe => recipe.inputs?.Count() ?? 0) ?? 0;
            outputLinkCount = raw.recipes?.Max(recipe => recipe.outputs?.Count() ?? 0) ?? 0;
        }
    }

    [System.Serializable]
    public record RecipeRaw
    {
        public int recipeID;
        public string name;
        public int ticks = 1;
        public List<LinkRaw> modifiers;
        public List<LinkRaw> inputs;
        public List<LinkRaw> outputs;
    }

    [System.Serializable]
    public record Recipe
    {
        public int recipeID;
        public string name;
        public int ticks = 1;
        public ImmutableList<Modifier> modifiers;
        public ImmutableList<Input> inputs;
        public ImmutableList<Output> outputs;

        public Recipe(RecipeRaw raw)
        {
            recipeID = raw.recipeID;
            name = raw.name;
            ticks = raw.ticks;

            modifiers = raw.modifiers?.Select(linkRaw => new Modifier(linkRaw)).ToImmutableList<Modifier>()
                        ?? ImmutableList<Modifier>.Empty;

            inputs = raw.inputs?.Select(linkRaw => new Input(linkRaw)).ToImmutableList<Input>()
                     ?? ImmutableList<Input>.Empty;

            outputs = raw.outputs?.Select(linkRaw => new Output(linkRaw)).ToImmutableList<Output>()
                      ?? ImmutableList<Output>.Empty;
        }
    }

    [System.Serializable]
    public record LinkRaw
    {
        public int link;
        public int resourceID;
        public List<string> resourceTags;
        public float ratio = 1f;
        public int storage;
        public bool required;
        public List<Affect> affects;
    }

    [System.Serializable]
    public record Input
    {
        public int link;
        public int resourceID;
        public TagDictionary resourceTags;
        public float ratio = 1f;
        public int storage;

        public Input(LinkRaw raw)
        {
            link = raw.link;
            resourceID = raw.resourceID;
            resourceTags = new TagDictionary(raw.resourceTags);
            ratio = raw.ratio;
            storage = raw.storage;
        }
    }

    [System.Serializable]
    public record Output
    {
        public int link;
        public int resourceID;
        public TagDictionary resourceTags;
        public float ratio = 1f;
        public int storage;

        public Output(LinkRaw raw)
        {
            link = raw.link;
            resourceID = raw.resourceID;
            resourceTags = new TagDictionary(raw.resourceTags);
            ratio = raw.ratio;
            storage = raw.storage;
        }
    }

    [System.Serializable]
    public record Modifier
    {
        public int link;
        public int resourceID;
        public TagDictionary resourceTags;
        public bool required;
        public ImmutableList<Affect> affects;

        public Modifier(LinkRaw raw)
        {
            link = raw.link;
            resourceID = raw.resourceID;
            resourceTags = new TagDictionary(raw.resourceTags);
            required = raw.required;
            affects = raw.affects.ToImmutableList();
        }
    }

    [System.Serializable]
    public record Affect
    {
        public int output;
        public float ratio;
    }
}