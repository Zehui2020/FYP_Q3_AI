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
    [Header("Coordinates to render the item's image")]
    public Vector2 m_ItemImageTexturePosition;

    // Item data
    private ItemData m_itemData;
    // Current item object
    private GameObject m_currentItemObject;
    // Flag to determine if the popup is active
    private bool m_isPopupActive = false;
    // Flag to check if a purchase message is being processed
    private bool isProcessingPurchaseMessage = false;
    // Item quantity
    private int m_itemQuantity = 1;
    // Current item quantity
    private int m_currentItemMQuantity;
    // Initial item quantity
    private const int ITEMQUANTITY_INITIAL = 1;
    // Total item price
    private int m_itemTotalPrice;

    // Prefab for purchase messages
    private GameObject m_itemPurchaseMessagePrefab;

    // Class to load items
    private ItemLoader m_itemLoader;

    // Item manager class
    private ItemShopManager m_itemManager;

    // Class responsible for displaying the purchase amount
    private TextFadeAndMove m_textFadeAndMove;

    // Class to manage the item shop UI
    private ItemShopUIHandler m_itemShopUIHandler;

    void Start()
    {
        // Handle case where the item loader class is not set
        if (m_itemLoader == null)
        {
            // Set the item loader class
            m_itemLoader = FindAnyObjectByType<ItemLoader>();
        }

        // Handle case where the item shop UI handler is not set
        if (m_itemShopUIHandler == null)
        {
            // Set the item shop UI handler class
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // Handle case where the item manager class is not set
        if (m_itemManager == null)
        {
            // Set the item manager class
            m_itemManager = FindAnyObjectByType<ItemShopManager>();
        }

        // Handle case where the purchase display class is not set
        if (m_textFadeAndMove == null)
        {
            // Set the purchase display class
            m_textFadeAndMove = FindAnyObjectByType<TextFadeAndMove>();
        }

        // Set the prefab for purchase messages
        m_itemPurchaseMessagePrefab = m_itemShopUIHandler.m_itemPurchaseMessage.gameObject;

        // Initially hide the popup panel
        m_itemShopUIHandler.m_itemShopBackground.SetActive(false);

        // Handle purchase button click
        m_itemShopUIHandler.m_purchaseButton.onClick.AddListener(PurchaseButtonClicked);

        // Handle cancel button click
        m_itemShopUIHandler.m_cancelButton.onClick.AddListener(CancelButtonClicked);

        // Initially hide the purchase screen
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);
        // Initially hide the purchase message
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);
    }

    public void OnItemClicked(ItemData itemdata, Dictionary<string, Sprite> spriteDictionary)
    {
        // Set the item data
        m_itemData = itemdata;

        // Prevent clicking other items while the popup is active
        if (m_isPopupActive)
        {
            return;
        }

        // Handle case where the item quantity is 0
        if (m_itemData.itemQuantity <= 0)
        {
            // Display a message
            // If the message is already active and has text, clear the message
            if (m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.activeSelf == true && !string.IsNullOrEmpty(m_itemShopUIHandler.m_itemPurchaseMessage.text))
            {
                // Clear the message
                m_itemShopUIHandler.m_itemPurchaseMessage.text = "";
            }

            // Set a new message
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Unable to purchase item";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.red;
            m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(true);

            // Set the flag indicating that a purchase message is being processed
            isProcessingPurchaseMessage = true;

            // Hide the message after a delay
            StartCoroutine(HideMessageAfterDelay(2.0f));
            return;
        }

        // Handle case where the previous item is still present
        if (m_currentItemObject != null)
        {
            // Destroy the previous item
            Destroy(m_currentItemObject);
        }

        // Get the RectTransform of the item image for display
        RectTransform rectTransform = m_itemShopUIHandler.m_forDisplayingItemImages.GetComponent<RectTransform>();

        // Set the anchor and pivot to the center
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Move the object based on the parent object's coordinates
        rectTransform.anchoredPosition += new Vector2(m_ItemImageTexturePosition.x, -m_ItemImageTexturePosition.y);

        // If the item image is set and the sprite can be retrieved from the dictionary, assign the sprite from the item data
        if (m_itemShopUIHandler.m_forDisplayingItemImages != null && spriteDictionary.TryGetValue(m_itemData.itemSprite, out Sprite sprite))
        {
            // Set the item image
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = sprite;
        }

        // Set the item name
        m_itemShopUIHandler.m_itemName.text = itemdata.itemName;
        // Set the price for a single item
        m_itemShopUIHandler.m_itemPerPrice.text = itemdata.itemPrice.ToString() + m_itemManager.GetItemUnit();
        // Set the item quantity
        m_currentItemMQuantity = itemdata.itemQuantity;
        // Update the item quantity
        UpdateItemQuantity();
        // Update the total price
        UpdateTotalPrice();
        // Show the popup panel
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(true);
        // Set the popup as active
        m_isPopupActive = true;
    }

    // Update the item quantity
    private void UpdateItemQuantity()
    {
        // If the text for item quantity is set, update it
        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            // Set the item quantity
            m_itemShopUIHandler.m_itemQuantity.text = "x" + m_itemQuantity.ToString();
        }
    }

    // Update the total price
    private void UpdateTotalPrice()
    {
        // If the item data is set, update the total price
        if (m_itemData != null)
        {
            // Set the total price
            m_itemTotalPrice = m_itemData.itemPrice * m_itemQuantity;
            m_itemShopUIHandler.m_itemTotalPrice.text = m_itemTotalPrice.ToString() + m_itemManager.GetItemUnit();

            // If the player's money is less than the total purchase amount, set the text color to red
            if (PlayerWallet.Instance.GetMoney() < m_itemTotalPrice)
            {
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.red;
            }
            // If the player's money is greater than or equal to the purchase amount, set the text color to white
            else
            {
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.white;
            }
        }
    }

    // Handle purchase button click
    public void PurchaseButtonClicked()
    {
        // If the player's money is less than the total price and the item quantity is greater than 0
        if (m_itemTotalPrice > PlayerWallet.Instance.GetMoney() && m_itemData.itemQuantity >= 1)
        {
            // Purchase failed
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase failed";
            // Set the text color to red
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.red;
        }
        else
        {
            // Purchase the item
            PurchaseItem(m_itemQuantity);
            // Update the stock
            m_itemData.itemQuantity -= m_itemQuantity;
            // Purchase successful
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase successful";
            // Set the text color to green
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.green;
        }

        // Display the purchase result message
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(true);

        // Hide the message after a certain time
        StartCoroutine(HideMessageAfterDelay(2.0f));
    }

    // Process to purchase the item
    private void PurchaseItem(int quantity)
    {
        // If item data is set, proceed with the purchase
        if (m_itemData != null)
        {
            // Deduct the amount from the player's money
            PlayerWallet.Instance.SpendMoney(m_itemData.itemPrice * quantity);

            // Update the item quantity
            m_itemData.itemQuantity -= quantity;

            // Save the item data
            m_itemLoader.SaveItemData();
        }

        // Destroy the generated object
        DestroyObject();
        // Reset the item quantity
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // Handle cancel button click
    public void CancelButtonClicked()
    {
        // Cancel the item purchase
        CancelItem();
    }

    // Process to cancel the item purchase
    void CancelItem()
    {
        // Destroy the generated object
        DestroyObject();
        // Reset the item quantity to the initial value
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // Process to hide the message after a delay
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        // Instantiate a new text object
        GameObject newPurchaseDisplay = Instantiate(m_itemPurchaseMessagePrefab, m_itemShopUIHandler.m_itemPurchaseMessage.transform.parent);
        var textComponent = newPurchaseDisplay.GetComponent<TextMeshProUGUI>();

        // Wait for the specified time
        yield return new WaitForSeconds(delay);

        // Hide the message
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);

        // Destroy the generated text
        Destroy(newPurchaseDisplay);

        // Turn off the flag indicating message processing
        isProcessingPurchaseMessage = false;
    }

    // Destroy the object
    void DestroyObject()
    {
        // Hide the popup
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);

        // If the item image is set, clear it
        if (m_itemShopUIHandler.m_forDisplayingItemImages != null)
        {
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = null;
        }

        // If the item name is set, clear it
        if (m_itemShopUIHandler.m_itemName != null)
        {
            m_itemShopUIHandler.m_itemName.text = string.Empty;
        }

        // If the price per item is set, clear it
        if (m_itemShopUIHandler.m_itemPerPrice != null)
        {
            m_itemShopUIHandler.m_itemPerPrice.text = string.Empty;
        }

        // If the total price is set, clear it
        if (m_itemShopUIHandler.m_itemTotalPrice != null)
        {
            m_itemShopUIHandler.m_itemTotalPrice.text = string.Empty;
        }

        // If the item quantity is set, clear it
        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            m_itemShopUIHandler.m_itemQuantity.text = string.Empty;
        }

        // If the current item object is set, destroy it
        if (m_currentItemObject != null)
        {
            Destroy(m_currentItemObject);
            m_currentItemObject = null;
        }

        // Deactivate the popup
        m_isPopupActive = false;

        // Reset the item quantity to the initial value
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // Get the popup flag
    public bool GetPopupFlag()
    {
        // Return the popup flag
        return m_isPopupActive;
    }
}
