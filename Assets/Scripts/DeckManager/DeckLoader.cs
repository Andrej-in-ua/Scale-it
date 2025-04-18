﻿using System;
using System.IO;
using UnityEngine;
using YamlDotNet.Serialization;

namespace DeckManager
{
    public class DeckLoader : MonoBehaviour
    {
        public static DeckLoader Instance { get; private set; }
        public Deck Deck { get; private set; }

        public string deckName = "default";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }

            if (!LoadDeck())
            {
                throw new Exception("Load deck failed");
            }
        }

        private bool LoadDeck()
        {
            var path = Path.Combine(Application.streamingAssetsPath, $"deck_{deckName}.yaml");

            if (!File.Exists(path))
            {
                Debug.LogError($"Deck loading error: {path}");
                return false;
            }

            var yamlText = File.ReadAllText(path);

            var deserializer = new DeserializerBuilder()
#if !DEBUG
                .IgnoreUnmatchedProperties()
#endif
                .Build();

            Deck = new Deck(deserializer.Deserialize<RawDeck>(yamlText));
            Debug.Log($"Deck \"{deckName}\" loaded.");

            return true;
        }
    }
}