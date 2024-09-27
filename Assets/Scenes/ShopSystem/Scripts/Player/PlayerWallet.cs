//----------------------------------------------------------------------
// PlayerWallet
//
// Class to manage the player's money
//
// Date: 2024/08/30
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.IO;
using UnityEngine;

public class PlayerWallet : MonoBehaviour
{
    // Player's money
    public static PlayerWallet Instance { get; private set; }

    // Load from the JSON file in Resources only when the game starts
    [Header("JSON File Name in Resources")]
    public string m_jsonFileName = "money";

    // Player's current money
    private int m_currentMoney;

    void Start()
    {
        // Flag to determine if it's the first playthrough
        if (IsFirstPlay())
        {
            // On the first playthrough, load from the Load folder
            LoadMoney();
        }
        else
        {
            // On game continuation, load from the Save folder
            LoadMoneyForContinue();
        }
    }

    // Method to determine if it's the first playthrough
    private bool IsFirstPlay()
    {
        string path = Path.Combine(Application.dataPath, "Scenes/ShopSystem/Json/Load/Money/money.json");
        return !File.Exists(path); // If no file exists in the Load folder, it's the first playthrough
    }

    void Awake()
    {
        // Singleton pattern: handle case where the instance is not set
        if (Instance == null)
        {
            // Set the object as the instance
            Instance = this;
            // Prevent the object from being destroyed when switching scenes
            DontDestroyOnLoad(gameObject);
            // Load money from the JSON file at startup
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
        // Return the player's current money
        return m_currentMoney;
    }

    // Method to add money to the player's wallet
    public void AddMoney(int amount)
    {
        // Add money to the player's wallet and then save it
        m_currentMoney += amount;
        // Save the player's money
        SaveMoney();
    }

    // Method to spend money from the player's wallet
    public bool SpendMoney(int amount)
    {
        // If the player's money is greater than or equal to the amount
        if (m_currentMoney >= amount)
        {
            // Subtract the specified amount from the player's wallet
            m_currentMoney -= amount;
            // Save the player's money
            SaveMoney();
            // Purchase successful
            return true;
        }
        else
        {
            // Purchase failed
            return false;
        }
    }

    // Method to save the player's money
    private void SaveMoney()
    {
        // Create an instance of the data class to store the player's money and set the current amount
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
        // On the first playthrough, load from the Load folder
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
            // Set the default money amount (10000G)
            m_currentMoney = 10000;
        }
    }

    // Method to load the player's money on game continuation
    public void LoadMoneyForContinue()
    {
        // On game continuation, load from the Save folder
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
            // Set the default money amount (10000G)
            m_currentMoney = 10000;
        }
    }
}

// Data class to store the player's money
[System.Serializable]
public class PlayerWalletData
{
    public int money;
}
