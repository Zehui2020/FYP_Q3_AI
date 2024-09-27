//----------------------------------------------------------------------
// PlayerWallet
//
// Class to manage player's money
//
// Date: 2024/08/30
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.IO;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    // Player's money instance
    public static PlayerWallet Instance { get; private set; }

    // Load the JSON file from Resources only when the game starts
    [Header("JSON File Name in Resources")]
    public string m_jsonFileName = "money";

    // Current player's money
    private int m_currentMoney;

    void Start()
    {
        // Check if it's the first playthrough
        if (IsFirstPlay())
        {
            // On first playthrough, load from the Load folder
            LoadMoney();
        }
        else
        {
            // On continuation, load from the Save folder
            LoadMoneyForContinue();
        }
    }

    // Method to check if it's the first playthrough
    private bool IsFirstPlay()
    {
        string path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Load/Money/money.json");
        return !File.Exists(path); // If the file doesn't exist in the Load folder, it's the first playthrough
    }

    void Awake()
    {
        // Singleton pattern implementation: if the instance is not set
        if (Instance == null)
        {
            // Set the object as the instance
            Instance = this;
            // Prevent the object from being destroyed when switching scenes
            DontDestroyOnLoad(gameObject);
            // Load money from JSON on startup
            LoadMoney();
        }
        else
        {
            // Destroy the duplicate instance
            Destroy(gameObject);
        }
    }

    // Method to get the player's current money
    public int GetMoney()
    {
        return m_currentMoney;
    }

    // Method to add money to the player's wallet
    public void AddMoney(int amount)
    {
        // Add money to the player's wallet and save it
        m_currentMoney += amount;
        SaveMoney();
    }

    // Method to spend money from the player's wallet
    public bool SpendMoney(int amount)
    {
        // If the player has enough money
        if (m_currentMoney >= amount)
        {
            // Deduct the specified amount from the player's wallet
            m_currentMoney -= amount;
            // Save the updated amount
            SaveMoney();
            // Return success
            return true;
        }
        // If the player doesn't have enough money
        else
        {
            // Return failure
            return false;
        }
    }

    // Method to save the player's money
    private void SaveMoney()
    {
        // Create an instance of the data class and set the current money
        var data = new PlayerWalletData { money = m_currentMoney };

        // Convert the data class to a JSON string
        string json = JsonUtility.ToJson(data);

        // Determine the path to save the JSON data
        string path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Save/Money/money.json");

        // Write the JSON data to the file
        File.WriteAllText(path, json);
    }

    // Method to load the player's money
    private void LoadMoney()
    {
        // On first playthrough, load from the Load folder
        string loadPath = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Load/Money/money.json");

        // Check if the saved data exists
        if (File.Exists(loadPath))
        {
            // Read the JSON data from the file
            string json = File.ReadAllText(loadPath);
            PlayerWalletData data = JsonUtility.FromJson<PlayerWalletData>(json);
            m_currentMoney = data.money;
        }
        else
        {
            // Set the default money (10000G)
            m_currentMoney = 10000;
        }
    }

    // Method to load the player's money when continuing the game
    public void LoadMoneyForContinue()
    {
        // On continuation, load from the Save folder
        string savePath = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Save/Money/money.json");

        // If the file exists, load the data
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerWalletData data = JsonUtility.FromJson<PlayerWalletData>(json);
            m_currentMoney = data.money;
        }
        else
        {
            // Set the default money (10000G)
            m_currentMoney = 10000;
        }
    }
}

// Data class for saving player's money
[System.Serializable]
public class PlayerWalletData
{
    public int money;
}
