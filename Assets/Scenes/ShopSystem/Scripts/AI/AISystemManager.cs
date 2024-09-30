//----------------------------------------------------------------------
// AISystemManager
//
// Class that manages the AI system
//
// Date: 25/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public class AISystemManager : MonoBehaviour
{
    // Item shop UI handler class
    public ItemShopUIHandler m_itemShopUIHandler;

    // Texture collision detection class
    private TextureCollision m_textureCollision;

    // Item price fluctuation class
    public ItemPriceFluctuations m_itemPriceFluctuations;

    // Current item data
    private ItemData m_itemData;

    // Array of item data
    private ItemData[] m_itemDataArray;

    // Dictionary to manage the price for each item
    private Dictionary<string, float> m_currentItemPrice = new Dictionary<string, float>();

    void Start()
    {
        // Handle when the item shop UI handler class is not set
        if (m_itemShopUIHandler == null)
        {
            // Set the item shop UI handler class
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // Handle when the texture collision detection class is not set
        if (m_textureCollision == null)
        {
            // Set the texture collision detection class
            m_textureCollision = GetComponent<TextureCollision>();
        }

        // Handle when the item price fluctuation class is not set
        if (m_itemPriceFluctuations == null)
        {
            // Set the item price fluctuation class
            m_itemPriceFluctuations = FindAnyObjectByType<ItemPriceFluctuations>();
        }
        // By default, display the AI system
        m_itemShopUIHandler.m_AISystemImage.gameObject.SetActive(true);

        // Initially, hide the object that displays the conversation with AI
        m_itemShopUIHandler.m_AIConversationDisplay.SetActive(false);
        /*
        // Debug setup
        m_itemShopUIHandler.m_debugBadBotton.onClick.AddListener(TheNumberMinus1);
        m_itemShopUIHandler.m_debugNormalBotton.onClick.AddListener(TheNumber0);
        m_itemShopUIHandler.m_debugGoodBotton.onClick.AddListener(TheNumberPlus1);
        */
    }

    public void BargainItemPrice(float modifier)
    {
        m_itemPriceFluctuations.ChangePriceBasedOnMood(m_itemDataArray, modifier);
    }

    // Debug methods
    private void TheNumberMinus1()
    {
        m_itemPriceFluctuations.ChangePriceBasedOnMood(m_itemDataArray, -1);
    }

    private void TheNumber0()
    {
        m_itemPriceFluctuations.ChangePriceBasedOnMood(m_itemDataArray, 0);
    }

    private void TheNumberPlus1()
    {
        m_itemPriceFluctuations.ChangePriceBasedOnMood(m_itemDataArray, 1);
    }

    // Display the AI system
    public void ShowAISystem()
    {
        // Display the AI system
        m_itemShopUIHandler.m_AISystemImage.gameObject.SetActive(true);
    }

    // Hide the AI system
    public void HideAISystem()
    {
        // Hide the AI system
        m_itemShopUIHandler.m_AISystemImage.gameObject.SetActive(false);
    }

    // Show the background for conversations with AI
    public void ShowAIConversationBackground()
    {
        m_itemShopUIHandler.m_AIConversationBackground.gameObject.SetActive(true);
    }

    // Show the object that displays the conversation with AI
    public void ShowAIConversationDisplay()
    {
        m_itemShopUIHandler.m_AIConversationDisplay.SetActive(true);
    }

    // Hide the object that displays the conversation with AI
    public void HideAIConversationDisplay()
    {
        m_itemShopUIHandler.m_AIConversationDisplay.SetActive(false);
    }

    // Get the fluctuation of item prices
    public int GetItemPriceFluctuations(string itemName)
    {
        return m_itemPriceFluctuations.GetCurrentPrice(itemName);
    }

    // Set item data
    public void SetItemData(ItemData itemData)
    {
        m_itemPriceFluctuations.SetItemData(itemData);
    }

    // Set the array of item data
    public void SetItemDataArray(ItemData[] itemDataArray)
    {
        m_itemDataArray = itemDataArray;
    }

    // Set the dictionary that manages the current price for each item
    public void SetCurrentItemPriceDictionary(Dictionary<string, float> currentItemPrice)
    {
        m_currentItemPrice = currentItemPrice;
    }

    // Initialize mood
    public void SetMood(int mood)
    {
        m_itemPriceFluctuations.SetMood(mood);
    }

    // Get mood
    public int GetMood()
    {
        return m_itemPriceFluctuations.GetMood();
    }
}
