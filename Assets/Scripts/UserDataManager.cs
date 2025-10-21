using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;

public static class UserDataManager
{
    [System.Serializable]
    public class UserData
    {
        public int uid;
        public string name;
        public int money;
        public int level;

        public string language;
        public List<int> ownedItemIds;

        public UserData()
        {
            uid = 0;
            name = "Player";
            money = 0;
            level = 1;
            ownedItemIds = new List<int>();
            language = "en";
        }

        public override string ToString()
        {
            return $"UID: {uid}, Name: {name}, Money: {money}, Level: {level}, Language: {language}, Owned Items: [{string.Join(", ", ownedItemIds)}]";
        }

        public void AddItem(int itemId)
        {
            if (!ownedItemIds.Contains(itemId))
            {
                ownedItemIds.Add(itemId);
            }
        }

        public void RemoveItem(int itemId)
        {
            if (ownedItemIds.Contains(itemId))
            {
                ownedItemIds.Remove(itemId);
            }
        }

        public bool OwnsItem(int itemId)
        {
            return ownedItemIds.Contains(itemId);
        }

        public void AddMoney(int amount)
        {
            money += amount;
        }

        // Not thread-safe
        public bool SpendMoney(int amount)
        {
            if (money >= amount)
            {
                money -= amount;
                return true;
            }
            return false;
        }

        public void LevelUp()
        {
            level += 1;
        }
    }

    private static string FilePath => Path.Combine(Application.persistentDataPath, "userData.json");

    public static void Save(UserData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(FilePath, json);
        Debug.Log($"UserData saved to: {FilePath}\n{json}");
    }

    public static UserData Load()
    {
        if (!File.Exists(FilePath))
        {
            Debug.Log("No save found, creating new data.");
            return new UserData(); // default values
        }

        string json = File.ReadAllText(FilePath);
        return JsonUtility.FromJson<UserData>(json);
    }
}