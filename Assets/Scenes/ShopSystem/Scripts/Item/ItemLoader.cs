//----------------------------------------------------------------------
// ItemLoader
//
// Class to load items
//
// Date: 9/13/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class ItemData
{
    [Header("Item Sprite Path")]
    public string itemSprite;
    [Header("Item Name")]
    public string itemName;
    [Header("Item Price")]
    public int itemPrice;
    [Header("Item Quantity")]
    public int itemQuantity;
}

[System.Serializable]
public class ItemDataList
{
    [Header("List of Items")]
    public ItemData[] items;
}

// Class to load items
public class ItemLoader : MonoBehaviour
{
    private ItemData[] m_itemDataArray;
    public ItemData m_itemData;
    public ItemDataList m_itemDataList;
    private Dictionary<string, Sprite> m_spriteDictionary;

    private bool isFirstRun = true; // First run flag

    private void Start()
    {
        // Load all sprites and store them in the dictionary
        LoadAllSprites();

        // Determine the path of the JSON file based on whether the game is being run for the first time
        string path = Path.Combine(Application.dataPath, IsFirstPlay()
            ? "Scenes/ShopSystem/Json/Load/Items/items.json"
            : "Scenes/ShopSystem/Json/Save/Items/items.json");

        // Load item data from the JSON file
        m_itemDataList = LoadItemData(path);
        if (m_itemDataList != null)
        {
            m_itemDataArray = m_itemDataList.items;
        }
    }

    // Method to check if this is the first playthrough
    private bool IsFirstPlay()
    {
        string loadPath = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Load/Items/items.json");
        return !File.Exists(loadPath);
    }

    // Method to load item data from a JSON file
    private ItemDataList LoadItemData(string path)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<ItemDataList>(json);
        }
        else
        {
            Debug.LogError($"JSON file not found: {path}");
            return null;
        }
    }

    // Load all sprites from the "items" folder
    private void LoadAllSprites()
    {
        // Set the path to the "items" folder
        string spriteFolderPath = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Resources/items");
        string[] spriteFiles = Directory.GetFiles(spriteFolderPath, "*.png");
        m_spriteDictionary = new Dictionary<string, Sprite>();

        foreach (string file in spriteFiles)
        {
            string fileName = Path.GetFileNameWithoutExtension(file);
            byte[] fileData = File.ReadAllBytes(file);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            if (sprite != null)
            {
                m_spriteDictionary[fileName] = sprite;
            }
        }
    }

    // Method to load item data based on whether it's the first run
    private ItemDataList LoadItemData()
    {
        string path;

        // Change path based on whether it's the first run
        if (isFirstRun)
        {
            path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Load/Items/items.json");
        }
        else
        {
            path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Save/Items/items.json");
        }

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            return JsonUtility.FromJson<ItemDataList>(json);
        }
        else
        {
            Debug.LogError($"JSON file not found: {path}");
            return null;
        }
    }

    // Method to save item data
    public void SaveItemData()
    {
        // Create an item data list
        ItemDataList itemDataList = new ItemDataList { items = m_itemDataArray };

        // Convert data to JSON format
        string json = JsonUtility.ToJson(itemDataList, true);

        // Determine the path to save the data
        string path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Save/Items/items.json");

        // Write the JSON data to a file
        File.WriteAllText(path, json);
    }

    // Check if data is loaded
    public bool IsDataLoaded()
    {
        return m_itemDataArray != null && m_itemDataArray.Length > 0 && m_spriteDictionary != null && m_spriteDictionary.Count > 0;
    }

    public ItemData GetItemData()
    {
        return m_itemData;
    }

    public ItemData[] GetItemDataArray()
    {
        return m_itemDataArray;
    }

    public Dictionary<string, Sprite> GetSpriteDictionary()
    {
        return m_spriteDictionary;
    }

    // Method called when restarting the game
    public void OnGameRestart()
    {
        isFirstRun = false; // Update the flag
        m_itemDataList = LoadItemData(); // Reload the item data
    }
}
