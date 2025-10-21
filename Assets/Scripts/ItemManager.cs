using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public static class ItemManager
{
    [System.Serializable]
    public class ItemData
    {
        public int id;
        public string name;
        public string category;
        public string iconPathSmall;
        public string iconPathLarge;
        public int price;
        public int rarity;
        public string description;

        public override string ToString()
        {
            return $"ID: {id}, Name: {name}, Category: {category}, Price: {price}, Rarity: {rarity}, Description: {description}";
        }
    }

    [System.Serializable]
    public class ItemDatabase
    {
        public List<ItemData> items;
        public IEnumerable<string> GetCategories() =>
        items.Select(i => i.category).Distinct();

        public IEnumerable<ItemData> GetByCategory(string cat) =>
        items.Where(i => i.category == cat);
    }
    public static ItemDatabase Load()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "items.json");
        if (!File.Exists(path))
        {
            Debug.LogError("items.json not found at " + path);
            return null;
        }

        string json = File.ReadAllText(path);
        ItemDatabase db = JsonUtility.FromJson<ItemDatabase>(json);
        Debug.Log($"Loaded {db.items.Count} items.");
        return db;
    }
}