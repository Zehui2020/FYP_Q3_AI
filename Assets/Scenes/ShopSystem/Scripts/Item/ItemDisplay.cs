//----------------------------------------------------------------------
// ItemDisplay
//
// Class for displaying items
//
// Date: 8/28/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemDisplay : MonoBehaviour
{
    [Header("UI Image for Item Sprite")]
    public Image m_itemImage;
    [Header("TextMeshProUGUI for Item Name")]
    public TextMeshProUGUI m_itemNameText;
    [Header("TextMeshProUGUI for Item Price")]
    public TextMeshProUGUI m_itemPriceText;

    // Current item data
    private ItemData m_currentItemData;
    // Current sprite information
    private Dictionary<string, Sprite> m_currentSpriteDictionary;

    // Setup the item
    public void Setup(ItemData itemData, Dictionary<string, Sprite> spriteDictionary, string itemUnit)
    {
        // Set the current item data
        m_currentItemData = itemData;
        // Set the current sprite information
        m_currentSpriteDictionary = spriteDictionary;

        // If the item image is set and the sprite for the item data is found in the dictionary
        if (m_itemImage != null && spriteDictionary.TryGetValue(m_currentItemData.itemSprite, out Sprite sprite))
        {
            // Set the sprite for the item
            m_itemImage.sprite = sprite;
        }

        // If the item name text is set
        if (m_itemNameText != null)
        {
            // Set the item name
            m_itemNameText.text = m_currentItemData.itemName;
        }

        // If the item price text is set
        if (m_itemPriceText != null)
        {
            // Set the item price
            m_itemPriceText.text = m_currentItemData.itemPrice.ToString() + itemUnit;
        }
    }

    // Get the current item data
    public ItemData GetCurrentItemData()
    {
        return m_currentItemData;
    }

    // Set the item price
    public void SetItemPrice(int price)
    {
        m_currentItemData.itemPrice = price;
    }

    // Get the current sprite information
    public Dictionary<string, Sprite> GetCurrentSpriteDictionary()
    {
        return m_currentSpriteDictionary;
    }
}
