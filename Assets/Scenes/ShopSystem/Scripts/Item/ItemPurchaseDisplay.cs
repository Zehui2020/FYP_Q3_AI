//----------------------------------------------------------------------
// ItemPurchaseDisplay
//
// Class to manage the display of item purchases
//
// Date: 2/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class ItemPurchaseDisplay : MonoBehaviour
{
    [Header("Coordinates to display the item image")]
    public Vector2 m_ItemImageTexturePosition;

    // Item data
    private ItemData m_itemData;
    // Current item object
    private GameObject m_currentItemObject;
    // Flag indicating if the popup is active
    private bool m_isPopupActive = false;
    // Flag to check if the purchase message is being processed
    private bool isProcessingPurchaseMessage = false;
    // Item quantity
    private int m_itemQuantity = 1;
    // Current item quantity
    private int m_currentItemMQuantity;
    // Minimum item quantity
    private const int ITEMQUANTITY_MIN = 1;
    // Initial item quantity
    private const int ITEMQUANTITY_INITIAL = 1;
    // Total item price
    private int m_itemTotalPrice;

    // Flag for holding the "+" button
    private bool m_isPlusButtonHeld = false;
    // Flag for holding the "-" button
    private bool m_isMinusButtonHeld = false;
    // Threshold for hold activation
    private const float HOLD_THRESHOLD = 0.5f;
    // Repeat rate after holding the button
    private const float HOLD_REPEAT_RATE = 0.1f;

    // Prefab for the item purchase message
    private GameObject m_itemPurchaseMessagePrefab;

    // Class to load items
    private ItemLoader m_itemLoader;

    // Item manager class
    private ItemShopManager m_itemManager;

    // Class to handle purchase amount display
    private TextFadeAndMove m_textFadeAndMove;

    // Class to manage the item shop UI
    private ItemShopUIHandler m_itemShopUIHandler;

    void Start()
    {
        // Set the item loader class if not already set
        if (m_itemLoader == null)
        {
            m_itemLoader = FindAnyObjectByType<ItemLoader>();
        }

        // Set the item shop UI handler class if not already set
        if (m_itemShopUIHandler == null)
        {
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // Set the item manager class if not already set
        if (m_itemManager == null)
        {
            m_itemManager = FindAnyObjectByType<ItemShopManager>();
        }

        // Set the class for displaying the purchase amount if not already set
        if (m_textFadeAndMove == null)
        {
            m_textFadeAndMove = FindAnyObjectByType<TextFadeAndMove>();
        }

        // Set the prefab for the purchase message
        m_itemPurchaseMessagePrefab = m_itemShopUIHandler.m_itemPurchaseMessage.gameObject;

        // Initially hide the popup panel
        m_itemShopUIHandler.m_itemShopBackground.SetActive(false);

        // Add listener to the purchase button
        m_itemShopUIHandler.m_purchaseButton.onClick.AddListener(PurchaseButtonClicked);

        // Add listener to the cancel button
        m_itemShopUIHandler.m_cancelButton.onClick.AddListener(CancelButtonClicked);

        // Initially hide the purchase screen and purchase result message
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);
    }

    public void OnItemClicked(ItemData itemdata, Dictionary<string, Sprite> spriteDictionary)
    {
        // Set the item data
        m_itemData = itemdata;

        // Prevent other items from being clicked while the popup is active
        if (m_isPopupActive)
        {
            return;
        }

        // Handle case where item quantity is 0
        if (m_itemData.itemQuantity <= 0)
        {
            // Display the message
            if (m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.activeSelf == true && !string.IsNullOrEmpty(m_itemShopUIHandler.m_itemPurchaseMessage.text))
            {
                m_itemShopUIHandler.m_itemPurchaseMessage.text = "";
            }

            // Set a new message
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Unable to purchase item";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.red;
            m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(true);

            // Set the flag to indicate the message is being processed
            isProcessingPurchaseMessage = true;

            // Hide the message after a delay
            StartCoroutine(HideMessageAfterDelay(2.0f));
            return;
        }

        // Handle the case where a previous item still exists
        if (m_currentItemObject != null)
        {
            Destroy(m_currentItemObject);
        }

        // Get the RectTransform of the item image
        RectTransform rectTransform = m_itemShopUIHandler.m_forDisplayingItemImages.GetComponent<RectTransform>();

        // Set anchor and pivot to center
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Adjust the position relative to the parent object
        rectTransform.anchoredPosition += new Vector2(m_ItemImageTexturePosition.x, -m_ItemImageTexturePosition.y);

        // Set the item image if available and if the sprite can be retrieved from the dictionary
        if (m_itemShopUIHandler.m_forDisplayingItemImages != null && spriteDictionary.TryGetValue(m_itemData.itemSprite, out Sprite sprite))
        {
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = sprite;
        }

        // Set item name, price, quantity, and update total price
        m_itemShopUIHandler.m_itemName.text = itemdata.itemName;
        m_itemShopUIHandler.m_itemPerPrice.text = itemdata.itemPrice.ToString() + m_itemManager.GetItemUnit();
        m_currentItemMQuantity = itemdata.itemQuantity;
        UpdateItemQuantity();
        UpdateTotalPrice();

        // Show the purchase screen background and set the popup active
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(true);
        m_isPopupActive = true;
    }

    // Update the displayed item quantity
    private void UpdateItemQuantity()
    {
        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            m_itemShopUIHandler.m_itemQuantity.text = "x" + m_itemQuantity.ToString();
        }
    }

    // Update the total price
    private void UpdateTotalPrice()
    {
        if (m_itemData != null)
        {
            m_itemTotalPrice = m_itemData.itemPrice * m_itemQuantity;
            m_itemShopUIHandler.m_itemTotalPrice.text = m_itemTotalPrice.ToString() + m_itemManager.GetItemUnit();

            // Change text color based on whether the player has enough money
            if (PlayerWallet.Instance.GetMoney() < m_itemTotalPrice)
            {
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.red;
            }
            else
            {
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.white;
            }
        }
    }

    // Handle the purchase button click
    public void PurchaseButtonClicked()
    {
        if (m_itemTotalPrice > PlayerWallet.Instance.GetMoney() && m_itemData.itemQuantity >= 1)
        {
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase failed";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.red;
        }
        else
        {
            PurchaseItem(m_itemQuantity);
            m_itemData.itemQuantity -= m_itemQuantity;
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase successful";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.green;
        }

        // Show the purchase result message
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(true);

        // Hide the message after a delay
        StartCoroutine(HideMessageAfterDelay(2.0f));
    }

    // Handle the purchase of the item
    private void PurchaseItem(int quantity)
    {
        if (m_itemData != null)
        {
            PlayerWallet.Instance.SpendMoney(m_itemData.itemPrice * quantity);
            m_itemData.itemQuantity -= quantity;

            // Save item data
            //m_itemLoader.SaveItemData();
        }

        DestroyObject();
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // Handle the cancel button click
    public void CancelButtonClicked()
    {
        CancelItem();
    }

    // Handle the canceling of an item
    void CancelItem()
    {
        DestroyObject();
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // Hide the message after a delay
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        GameObject newPurchaseDisplay = Instantiate(m_itemPurchaseMessagePrefab, m_itemShopUIHandler.m_itemPurchaseMessage.transform.parent);
        var textComponent = newPurchaseDisplay.GetComponent<TextMeshProUGUI>();

        yield return new WaitForSeconds(delay);

        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);
        Destroy(newPurchaseDisplay);

        isProcessingPurchaseMessage = false;
    }

    // Destroy the object and hide the popup
    void DestroyObject()
    {
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);

        if (m_itemShopUIHandler.m_forDisplayingItemImages != null)
        {
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = null;
        }

        if (m_itemShopUIHandler.m_itemName != null)
        {
            m_itemShopUIHandler.m_itemName.text = string.Empty;
        }

        if (m_itemShopUIHandler.m_itemPerPrice != null)
        {
            m_itemShopUIHandler.m_itemPerPrice.text = string.Empty;
        }

        if (m_itemShopUIHandler.m_itemTotalPrice != null)
        {
            m_itemShopUIHandler.m_itemTotalPrice.text = string.Empty;
        }

        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            m_itemShopUIHandler.m_itemQuantity.text = string.Empty;
        }

        if (m_currentItemObject != null)
        {
            Destroy(m_currentItemObject);
            m_currentItemObject = null;
        }

        m_isPopupActive = false;
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // Return the popup flag
    public bool GetPopupFlag()
    {
        return m_isPopupActive;
    }
}
