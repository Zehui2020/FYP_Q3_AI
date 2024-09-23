//----------------------------------------------------------------------
// ItemLoader
//
// Class that loads items
//
// Data: 9/13/2024
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

// Class that loads items
public class ItemLoader : MonoBehaviour
{
    // Item data array
    private ItemData[] m_itemDataArray;
    // Item data
    public ItemData m_itemData;
    // Item data list
    public ItemDataList m_itemDataList;
    // Sprites
    private Dictionary<string, Sprite> m_spriteDictionary;

    // Flag to check if it's the first run
    private bool m_isFirstRun = true;

    private void Start()
    {
        // Load all sprites and store them in the dictionary
        LoadAllSprites();

        // Load item data from JSON file
        m_itemDataList = LoadItemData();
        if (m_itemDataList != null)
        {
            m_itemDataArray = m_itemDataList.items;
        }
    }

    private void LoadAllSprites()
    {
        // Set the path for the "Items/items" folder
        string spriteFolderPath = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Load/Items/items");
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

    // Process to load item data
    private ItemDataList LoadItemData()
    {
        string path;

        // Check if it's the first run
        if (m_isFirstRun)
        {
            // Load the JSON file only on the first run
            path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Load/Items/items.json");
        }
        else
        {
            // Load the JSON file on subsequent runs
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

    // Process to save item data
    public void SaveItemData()
    {
        // Create item data list
        ItemDataList itemDataList = new ItemDataList { items = m_itemDataArray };

        // Convert data to JSON format
        string json = JsonUtility.ToJson(itemDataList, true);

        // Determine save path
        string path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Save/Items/items.json");

        // Write JSON data to file
        File.WriteAllText(path, json);
    }

    // Check if data has been loaded
    public bool IsDataLoaded()
    {
        // Return item data array
        return m_itemDataArray != null && m_itemDataArray.Length > 0 && m_spriteDictionary != null && m_spriteDictionary.Count > 0;
    }

    // Get item data
    public ItemData GetItemData()
    {
        // Return item data
        return m_itemData;
    }

    // Get item data array
    public ItemData[] GetItemDataArray()
    {
        return m_itemDataArray;
    }

    // Get sprites
    public Dictionary<string, Sprite> GetSpriteDictionary()
    {
        // Return sprites
        return m_spriteDictionary;
    }

    // Process called on game restart
    public void OnGameRestart()
    {
        // Update flag
        m_isFirstRun = false;
        // Reload item data
        m_itemDataList = LoadItemData();
    }
}
