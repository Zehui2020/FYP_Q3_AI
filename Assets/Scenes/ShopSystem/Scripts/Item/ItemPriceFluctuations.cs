//----------------------------------------------------------------------
// ItemPriceFluctuations
//
// Class to handle fluctuations in item prices
//
// Date: 25/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine;

public class ItemPriceFluctuations : MonoBehaviour
{
    // Price fluctuation rate for items (10%)
    public float m_priceChangeRate = 0.1f;

    // Dictionaries to manage the initial and current prices of items
    private Dictionary<string, float> m_initialItemPrices = new Dictionary<string, float>();
    private Dictionary<string, float> m_currentItemPrices = new Dictionary<string, float>();

    // Upper and lower limits for fluctuating prices
    private float m_minPriceFactor = 0.5f;  // 50%
    private float m_maxPriceFactor = 1.0f;  // 100%

    // Mood state (Bad: -1, Neutral: 0, Good: 1)
    public int m_mood = 0;

    // Process to change prices based on mood
    public void ChangePriceBasedOnMood(string itemName, ItemData[] itemDataArray, Dictionary<string, float> initialItemPrice, Dictionary<string, float> currentItemPrice, int isMood)
    {
        m_currentItemPrices = currentItemPrice;

        // Set initial prices
        foreach (var itemData in itemDataArray)
        {
            // Set initial item prices (if not already initialized)
            if (!initialItemPrice.ContainsKey(itemData.itemName))
            {
                initialItemPrice[itemData.itemName] = itemData.itemPrice;
            }

            if (!m_currentItemPrices.ContainsKey(itemData.itemName))
            {
                m_currentItemPrices[itemData.itemName] = itemData.itemPrice;
            }

            Debug.Log(itemData.itemName + " initial price: " + itemData.itemPrice);
        }

        // Check if the item name exists in the dictionary
        if (!m_currentItemPrices.ContainsKey(itemName))
        {
            Debug.LogError("Item name not found: " + itemName);
            return;
        }

        // Get the current and initial prices
        float currentPrice = m_currentItemPrices[itemName];
        float initialPrice = initialItemPrice[itemName];

        // Price change based on mood
        if (isMood >= 1)
        {
            // Good mood: decrease price by 10%
            currentPrice -= initialPrice * m_priceChangeRate;
        }
        else if (isMood < 0)
        {
            // Bad mood: increase price by 10%
            currentPrice += initialPrice * m_priceChangeRate;
        }

        // Clamp the price between the minimum and maximum factors
        currentPrice = Mathf.Clamp(currentPrice, initialPrice * m_minPriceFactor, initialPrice * m_maxPriceFactor);

        // Save the updated price in the dictionary
        m_currentItemPrices[itemName] = currentPrice;

        Debug.Log("Price of " + itemName + " changed due to mood " + isMood + ": " + currentPrice);
    }

    // Get the current price of the specified item
    public int GetCurrentPrice(string itemName)
    {
        if (m_currentItemPrices.ContainsKey(itemName))
        {
            return (int)m_currentItemPrices[itemName];
        }
        else
        {
            return 0;
        }
    }

    // Get the current mood
    public int GetMood()
    {
        return m_mood;
    }
}
