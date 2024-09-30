//----------------------------------------------------------------------
// ItemPriceFluctuations
//
// Class to handle item price fluctuations
//
// Date: 25/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPriceFluctuations : MonoBehaviour
{
    // Percentage of price fluctuation (10%)
    public float m_priceChangeRate = 0.1f;

    // Item data
    private ItemData m_itemData;

    // Dictionaries to manage initial and current item prices
    private Dictionary<string, int> m_initialItemPrices = new Dictionary<string, int>();
    private Dictionary<string, float> m_currentItemPrices = new Dictionary<string, float>();

    // Upper and lower bounds for fluctuating prices
    private float m_minPriceFactor = 0.5f;  // 50%
    private float m_maxPriceFactor = 1.0f;  // 100%

    // Mood states (Bad: -1, Neutral: 0, Good: 1)
    public int m_mood = 0;

    // Handle price changes based on mood
    public void ChangePriceBasedOnMood(ItemData[] itemDataArray, float modifier)
    {
        // Set initial prices (only if not already set)
        foreach (var itemData in itemDataArray)
        {
            if (!m_initialItemPrices.ContainsKey(itemData.itemName))
            {
                // Initial price is set once and not changed afterward
                m_initialItemPrices[itemData.itemName] = itemData.itemPrice;
            }

            if (!m_currentItemPrices.ContainsKey(itemData.itemName))
            {
                m_currentItemPrices[itemData.itemName] = itemData.itemPrice;
            }

            // Get the current price
            float currentPrice = m_currentItemPrices[itemData.itemName];
            float initialPrice = m_initialItemPrices[itemData.itemName];

            // Adjust price based on mood
            float newPrice = initialPrice - (initialPrice * m_priceChangeRate * modifier);
            float priceDiff = currentPrice - newPrice;
            currentPrice -= priceDiff;

            /*
            if (isMood >= 1)
            {
                currentPrice -= initialPrice * m_priceChangeRate;
            }
            else if (isMood < 0)
            {
                currentPrice += initialPrice * m_priceChangeRate;
            }
            */

            // Clamp the price between the minimum and maximum factors
            currentPrice = Mathf.Clamp(currentPrice, initialPrice * m_minPriceFactor, initialPrice * m_maxPriceFactor);

            // Save the updated price in the dictionary
            m_currentItemPrices[itemData.itemName] = currentPrice;
            Debug.Log(itemData.itemName + ": " + currentPrice);
        }

        // Set the mood
        m_mood = 1;
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

    // Set the item data
    public void SetItemData(ItemData itemData)
    {
        m_itemData = itemData;
    }

    // Set the mood
    public void SetMood(int mood)
    {
        m_mood = mood;
    }

    // Get the mood
    public int GetMood()
    {
        return m_mood;
    }
}
