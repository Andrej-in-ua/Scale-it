using System;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;

namespace DeckManager
{
    public static class DeckLoader
    {
        public static void LoadAsGlobal(string deckName)
        {
            var loadedDeck = Load(deckName);

            if (loadedDeck == null)
            {
                // If the deck fails to load, we can't continue. This is a critical error.
                // Call Load(string deckName) directly to handle the error.
                throw new Exception("Could not load deck: " + deckName);
            }

            Deck.SetAsGlobal(Load(deckName));
        }

        public static Deck Load(string deckName)
        {
            var path = Path.Combine(Application.streamingAssetsPath, $"card_decks/{deckName}/deck_{deckName}.yaml");

            if (!File.Exists(path))
            {
                Debug.LogError($"Deck loading error: {path}");
                return null;
            }

            var yamlText = File.ReadAllText(path);

            var deserializer = new DeserializerBuilder()
#if !DEBUG
                .IgnoreUnmatchedProperties()
#endif
                .Build();

            var loadedDeck = new Deck(deserializer.Deserialize<RawDeck>(yamlText));

            Debug.Log($"Deck \"{loadedDeck.meta.name} (v{loadedDeck.meta.version})\" loaded.");

            return loadedDeck;
        }
    }
}