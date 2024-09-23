//----------------------------------------------------------------------
// PlayerWallet
//
// Class for managing the player's wallet
//
// Data: 2024/08/30
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.IO;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    // Player's current wallet instance
    public static PlayerWallet Instance { get; private set; }

    // Load JSON file name in Resources only when the game starts
    [Header("JSON File Name in Resources")]
    public string m_jsonFileName = "money";

    // Current player's money
    private int m_currentMoney;

    void Start()
    {
        // For checking the save path
        Debug.Log("Persistent Data Path: " + Application.persistentDataPath);

        // Flag to determine if it's the first play
        if (IsFirstPlay())
        {
            // Load from the Load folder on the first play
            LoadMoney();
        }
        else
        {
            // Load from the Save folder on resuming
            LoadMoneyForContinue();
        }
    }

    // Method to check if it's the first play
    private bool IsFirstPlay()
    {
        string path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Load/Money/money.json");
        return !File.Exists(path); // If the file does not exist in the Load folder, it's the first play
    }

    void Awake()
    {
        // Implementing the Singleton pattern: check if the instance is not set
        if (Instance == null)
        {
            // Set the instance as the current object
            Instance = this;
            // Do not destroy the object when switching scenes
            DontDestroyOnLoad(gameObject);
            // Load money from JSON at startup
            LoadMoney();
        }
        else
        {
            // Destroy duplicate instances
            Destroy(gameObject);
        }
    }

    // Method to get the player's money
    public int GetMoney()
    {
        // Get the current player's money
        return m_currentMoney;
    }

    // Method to add money to the player's wallet
    public void AddMoney(int amount)
    {
        // Add the specified amount to the player's money and save it
        m_currentMoney += amount;
        // Save the player's money
        SaveMoney();
    }

    // Method to spend money from the player's wallet
    public bool SpendMoney(int amount)
    {
        // If the player's money is greater than or equal to the purchase amount
        if (m_currentMoney >= amount)
        {
            // Deduct the specified amount from the player's money
            m_currentMoney -= amount;
            // Save the player's money
            SaveMoney();
            // Purchase is successful
            return true;
        }
        // If the player's money is less than the purchase amount
        else
        {
            // Purchase fails
            return false;
        }
    }

    // Method to save the player's money
    private void SaveMoney()
    {
        // Create an instance of the data class to save the current money
        var data = new PlayerWalletData { money = m_currentMoney };

        // Convert the data class to a JSON string
        string json = JsonUtility.ToJson(data);

        // Determine the path for saving JSON data
        string path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Save/Money/money.json");

        // Write the JSON data to the file
        File.WriteAllText(path, json);
    }

    // Method to load the player's money
    private void LoadMoney()
    {
        // Load from the Load folder on the first play
        string loadPath = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Load/Money/money.json");

        // Check if the saved data exists
        if (File.Exists(loadPath))
        {
            // Read JSON data from the file
            string json = File.ReadAllText(loadPath);
            PlayerWalletData data = JsonUtility.FromJson<PlayerWalletData>(json);
            m_currentMoney = data.money;
        }
        else
        {
            // Set default money (10000G)
            m_currentMoney = 10000;
        }
    }

    // Method to load the player's money for resuming
    public void LoadMoneyForContinue()
    {
        // Load from the Save folder when resuming
        string savePath = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Save/Money/money.json");

        // Load data if the file exists
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            PlayerWalletData data = JsonUtility.FromJson<PlayerWalletData>(json);
            m_currentMoney = data.money;
        }
        else
        {
            // Set default money (10000G)
            m_currentMoney = 10000;
        }
    }
}

// Data class for saving the player's wallet
[System.Serializable]
public class PlayerWalletData
{
    public int money;
}
