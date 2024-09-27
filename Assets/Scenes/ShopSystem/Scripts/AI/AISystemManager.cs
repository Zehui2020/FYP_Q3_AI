//----------------------------------------------------------------------
// AISystemManager
//
// Class to manage the AI system
//
// Date: 25/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISystemManager : MonoBehaviour
{
    // Item shop UI management class
    public ItemShopUIHandler m_itemShopUIHandler;

    // Texture collision detection class
    private TextureCollision m_textureCollision;

    // Item price fluctuation class
    public ItemPriceFluctuations m_itemPriceFluctuations;

    // Current item data
    private ItemData m_itemData;

    // Array of item data
    private ItemData[] m_itemDataArray;

    // Dictionaries to manage item prices
    private Dictionary<string, float> m_initialItemPrice = new Dictionary<string, float>();
    private Dictionary<string, float> m_currentItemPrice = new Dictionary<string, float>();

    void Start()
    {
        // Handle case where the item shop UI management class is not set
        if (m_itemShopUIHandler == null)
        {
            // Set the item shop UI management class
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // Handle case where the texture collision class is not set
        if (m_textureCollision == null)
        {
            // Set the texture collision detection class
            m_textureCollision = GetComponent<TextureCollision>();
        }

        // Handle case where the item price fluctuation class is not set
        if (m_itemPriceFluctuations == null)
        {
            // Set the item price fluctuation class
            m_itemPriceFluctuations = FindAnyObjectByType<ItemPriceFluctuations>();
        }

        // Initially display the AI system
        m_itemShopUIHandler.m_AISystemImage.gameObject.SetActive(true);

        // Initially hide the object that displays AI conversations
        m_itemShopUIHandler.m_AIConversationDisplay.SetActive(false);

        // For debugging
        m_itemShopUIHandler.m_debugBadBotton.onClick.AddListener(TheNumberMinus1);
        m_itemShopUIHandler.m_debugNormalBotton.onClick.AddListener(TheNumber0);
        m_itemShopUIHandler.m_debugGoodBotton.onClick.AddListener(TheNumberPlus1);
    }

    // For debugging
    private void TheNumberMinus1()
    {
        m_itemPriceFluctuations.ChangePriceBasedOnMood(m_itemData.itemName, m_itemDataArray, m_initialItemPrice, m_currentItemPrice, -1);
    }
    private void TheNumber0()
    {
        m_itemPriceFluctuations.ChangePriceBasedOnMood(m_itemData.itemName, m_itemDataArray, m_initialItemPrice, m_currentItemPrice, 0);
    }
    private void TheNumberPlus1()
    {
        m_itemPriceFluctuations.ChangePriceBasedOnMood(m_itemData.itemName, m_itemDataArray, m_initialItemPrice, m_currentItemPrice, 1);
    }

    // Show the AI system
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

    // Show background for AI conversation
    public void ShowAIConversationBackground()
    {
        m_itemShopUIHandler.m_AIConversationBackground.gameObject.SetActive(true);
    }

    // Show the object that displays AI conversations
    public void ShowAIConversationDisplay()
    {
        m_itemShopUIHandler.m_AIConversationDisplay.SetActive(true);
    }

    // Hide the object that displays AI conversations
    public void HideAIConversationDisplay()
    {
        m_itemShopUIHandler.m_AIConversationDisplay.SetActive(false);
    }

    // Get item price fluctuations
    public int GetItemPriceFluctuations(string itemName)
    {
        return m_itemPriceFluctuations.GetCurrentPrice(itemName);
    }

    // Set item data
    public void SetItemData(ItemData itemData)
    {
        m_itemData = itemData;
    }

    // Set item data array
    public void SetItemDataArray(ItemData[] itemDataArray)
    {
        m_itemDataArray = itemDataArray;
    }

    // Set the dictionary for managing initial item prices
    public void SetInitialItemPriceDictionary(Dictionary<string, float> initialItemPrice)
    {
        m_initialItemPrice = initialItemPrice;
    }

    // Set the dictionary for managing current item prices
    public void SetCurrentItemPriceDictionary(Dictionary<string, float> currentItemPrice)
    {
        m_currentItemPrice = currentItemPrice;
    }

    // Get mood
    public int GetMood()
    {
        return m_itemPriceFluctuations.GetMood();
    }
}
