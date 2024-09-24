//----------------------------------------------------------------------
// ItemPurchaseDisplay
//
// Class that manages the display for item purchases
//
// Data: 2/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

public class ItemPurchaseDisplay : MonoBehaviour
{
    [Header("Coordinates for drawing the item image")]
    public Vector2 m_ItemImageTexturePosition;

    // Item data
    private ItemData m_itemData;
    // Current item object
    private GameObject m_currentItemObject;
    // Flag indicating whether the popup is active
    private bool m_isPopupActive = false;
    // Quantity of the item
    private int m_itemQuantity = 1;
    // Current quantity of the item
    private int m_currentItemMQuantity;
    // Minimum value for item quantity
    private const int ITEMQUANTITY_MIN = 0;
    // Initial value for item quantity
    private const int ITEMQUANTITY_INITIAL = 0;
    // Total price of the item
    private int m_itemTotalPrice;

    // Flag for whether the + button is held down
    private bool m_isPlusButtonHeld = false;
    // Flag for whether the - button is held down
    private bool m_isMinusButtonHeld = false;
    // Threshold for enabling long press
    private const float HOLD_THRESHOLD = 0.5f;
    // Repeat rate after long press
    private const float HOLD_REPEAT_RATE = 0.1f;

    // Prefab for the purchase message
    private GameObject m_itemPurchaseMessagePrefab;

    // Class for loading items
    private ItemLoader m_itemLoader;

    // Class for managing items
    private ItemShopManager m_itemManager;

    // Class responsible for displaying purchase amounts
    private TextFadeAndMove m_textFadeAndMove;

    // Class managing the item shop UI
    private ItemShopUIHandler m_itemShopUIHandler;

    void Start()
    {
        // Handling if the item loader class is not set
        if (m_itemLoader == null)
        {
            // Setting the item loader class
            m_itemLoader = FindAnyObjectByType<ItemLoader>();
        }

        // Handling if the item shop UI manager class is not set
        if (m_itemShopUIHandler == null)
        {
            // Setting the item shop UI manager class
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // Handling if the item manager class is not set
        if (m_itemManager == null)
        {
            // Setting the item manager class
            m_itemManager = FindAnyObjectByType<ItemShopManager>();
        }

        // Handling if the purchase amount display class is not set
        if (m_textFadeAndMove == null)
        {
            // Setting the purchase amount display class
            m_textFadeAndMove = FindAnyObjectByType<TextFadeAndMove>();
        }

        // Setting the prefab for the purchase message
        m_itemPurchaseMessagePrefab = m_itemShopUIHandler.m_itemPurchaseMessage.gameObject;

        // Hiding the popup panel
        m_itemShopUIHandler.m_itemShopBackground.SetActive(false);
        // Handling when the purchase button is clicked
        m_itemShopUIHandler.m_purchaseButton.onClick.AddListener(PurchaseButtonClicked);
        // Handling when the cancel button is clicked
        m_itemShopUIHandler.m_cancelButton.onClick.AddListener(CancelButtonClicked);

        // Handling when the increase quantity button is clicked
        m_itemShopUIHandler.m_plusButton.onClick.AddListener(IncreaseQuantity);

        // Setting up click listener for the + button
        var plusButtonTrigger = m_itemShopUIHandler.m_plusButton.gameObject.AddComponent<EventTrigger>();

        // Handling when the + button is pressed
        var plusDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        plusDownEntry.callback.AddListener((data) => { StartButtonHold(true); }); // Start long press
        plusButtonTrigger.triggers.Add(plusDownEntry);

        // Handling when the + button is released
        var plusUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        plusUpEntry.callback.AddListener((data) => { StopButtonHold(); }); // Stop long press
        plusButtonTrigger.triggers.Add(plusUpEntry);

        // Handling when the decrease quantity button is clicked
        m_itemShopUIHandler.m_minusButton.onClick.AddListener(DecreaseQuantity);

        // Setting up click listener for the - button
        var minusButtonTrigger = m_itemShopUIHandler.m_minusButton.gameObject.AddComponent<EventTrigger>();

        // Handling when the - button is pressed
        var minusDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        minusDownEntry.callback.AddListener((data) => { StartButtonHold(false); }); // Start long press
        minusButtonTrigger.triggers.Add(minusDownEntry);

        // Handling when the - button is released
        var minusUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        minusUpEntry.callback.AddListener((data) => { StopButtonHold(); }); // Stop long press
        minusButtonTrigger.triggers.Add(minusUpEntry);

        // Hiding the purchase screen initially
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);
        // Hiding the purchase result message initially
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);
    }

    // Handling when an item is clicked
    public void OnItemClicked(ItemData itemdata, Dictionary<string, Sprite> spriteDictionary)
    {
        // Setting the item data
        m_itemData = itemdata;

        // Handling when the item quantity is 0
        if (m_itemData.itemQuantity <= 0)
        {
            // Displaying a message
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Unable to purchase item";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.red;
            m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(true);
            StartCoroutine(HideMessageAfterDelay(2.0f)); // Hide message after a certain time
            return;
        }

        // Preventing clicks on other items while the popup is displayed
        if (m_isPopupActive)
        {
            return;
        }

        // Handling if there is a previous item remaining
        if (m_currentItemObject != null)
        {
            // Destroying the previous item
            Destroy(m_currentItemObject);
        }

        // Getting the RectTransform for the item image display
        RectTransform rectTransform = m_itemShopUIHandler.m_forDisplayingItemImages.GetComponent<RectTransform>();

        // Setting the anchor and pivot to the center
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Moving based on the specified coordinates
        rectTransform.anchoredPosition += new Vector2(m_ItemImageTexturePosition.x, -m_ItemImageTexturePosition.y);

        // Handling when the item image is set and the sprite can be obtained from the dictionary
        if (m_itemShopUIHandler.m_forDisplayingItemImages != null && spriteDictionary.TryGetValue(m_itemData.itemSprite, out Sprite sprite))
        {
            // Setting the item image
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = sprite;
        }
        // Setting the item name
        m_itemShopUIHandler.m_itemName.text = itemdata.itemName;
        // Setting the price for one item
        m_itemShopUIHandler.m_itemPerPrice.text = itemdata.itemPrice.ToString() + m_itemManager.GetItemUnit();
        // Setting the current item quantity
        m_currentItemMQuantity = itemdata.itemQuantity;
        // Updating the item quantity
        UpdateItemQuantity();
        // Updating the total price
        UpdateTotalPrice();
        // Displaying the popup panel
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(true);
        // Setting the popup to active
        m_isPopupActive = true;
    }

    // Increase the quantity of the item to purchase
    private void IncreaseQuantity()
    {
        // Setting the upper limit for item quantity
        if (m_itemQuantity < m_currentItemMQuantity)
        {
            // Increasing the item quantity
            m_itemQuantity++;
            UpdateItemQuantity();
            // Updating the total price
            UpdateTotalPrice();
        }
    }

    // Decrease the quantity of the item to purchase
    private void DecreaseQuantity()
    {
        // Handling if the item quantity is below the minimum
        if (m_itemQuantity > ITEMQUANTITY_MIN)
        {
            // Decreasing the item quantity
            m_itemQuantity--;
            UpdateItemQuantity();
            // Updating the total price
            UpdateTotalPrice();
        }
    }

    // Update the item quantity
    private void UpdateItemQuantity()
    {
        // Handling if the item quantity display text is set
        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            // Setting the item quantity
            m_itemShopUIHandler.m_itemQuantity.text = "x" + m_itemQuantity.ToString();
        }
    }

    // Update the total price
    private void UpdateTotalPrice()
    {
        // Handling if the item data is set
        if (m_itemData != null)
        {
            // Setting the total price
            m_itemTotalPrice = m_itemData.itemPrice * m_itemQuantity;
            m_itemShopUIHandler.m_itemTotalPrice.text = m_itemTotalPrice.ToString() + m_itemManager.GetItemUnit();

            // Handling if the purchase price is higher than the player's money
            if (PlayerWallet.Instance.GetMoney() < m_itemTotalPrice)
            {
                // Changing the text color to red
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.red;
            }
            // Handling if the purchase price is lower than the player's money
            else
            {
                // Changing the text color to white
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.white;
            }
        }
    }

    // Process when the purchase button is clicked
    public void PurchaseButtonClicked()
    {
        // Calculating the total price
        int totalPrice = m_itemData.itemPrice * m_itemQuantity;

        // Handling if the player's money is less than the purchase price
        if (m_itemTotalPrice > PlayerWallet.Instance.GetMoney())
        {
            // Purchase failed
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase failed";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.red;
        }
        else
        {
            // Purchasing the item
            PurchaseItem(m_itemQuantity);
            // Updating the stock
            m_itemData.itemQuantity -= m_itemQuantity;
            // Purchase successful
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase successful";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.green;

            // Saving item data
            m_itemLoader.SaveItemData();

            // Displaying the purchase amount as text
            StartCoroutine(m_textFadeAndMove.FadeMoveAndResetText("-", m_itemTotalPrice, m_itemManager.GetItemUnit()));
        }

        // Displaying the purchase result message
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(true);

        // Hiding the message after a certain time (seconds)
        StartCoroutine(HideMessageAfterDelay(2.0f));
    }

    // Handling when the button is held down
    private void StartButtonHold(bool isPlusButton)
    {
        // Setting the long press flag
        if (isPlusButton)
        {
            m_isPlusButtonHeld = true;
        }
        else
        {
            m_isMinusButtonHeld = true;
        }

        // Starting the coroutine
        StartCoroutine(HoldButtonCoroutine(isPlusButton));
    }

    // Processing when the button is held down
    private IEnumerator HoldButtonCoroutine(bool isPlusButton)
    {
        // Waiting until a certain time has passed
        yield return new WaitForSeconds(HOLD_THRESHOLD);

        // Processing while the button is held down
        while (isPlusButton ? m_isPlusButtonHeld : m_isMinusButtonHeld)
        {
            // Increasing or decreasing the item quantity
            if (isPlusButton)
            {
                IncreaseQuantity();
            }
            else
            {
                DecreaseQuantity();
            }

            // Controlling the repeat rate
            yield return new WaitForSeconds(HOLD_REPEAT_RATE);
        }
    }

    // Handling when the button is released
    private void StopButtonHold()
    {
        // Resetting the long press flags
        m_isPlusButtonHeld = false;
        m_isMinusButtonHeld = false;
    }

    // Processing for purchasing the item
    private void PurchaseItem(int quantity)
    {
        // Handling if the item data is set
        if (m_itemData != null)
        {
            // Deducting from the player's money
            PlayerWallet.Instance.SpendMoney(m_itemData.itemPrice * quantity);

            // Updating the item quantity
            m_itemData.itemQuantity -= quantity;

            // Saving the item data
            m_itemLoader.SaveItemData();
        }

        // Destroying the created object
        DestroyObject();
        // Resetting the item quantity
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // Processing when the cancel button is clicked
    public void CancelButtonClicked()
    {
        // Cancelling the item purchase
        CancelItem();
    }

    // Processing for cancelling the item
    void CancelItem()
    {
        // Destroying the created object
        DestroyObject();
        // Resetting the item quantity
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // Processing to hide the message after a certain time
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        // Creating a new text object
        GameObject newPurchaseDisplay = Instantiate(m_itemPurchaseMessagePrefab, m_itemShopUIHandler.m_itemPurchaseMessage.transform.parent);
        var textComponent = newPurchaseDisplay.GetComponent<TextMeshProUGUI>();

        // Waiting for the specified time
        yield return new WaitForSeconds(delay);

        // Hiding the message
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);

        // Destroying the generated text
        Destroy(newPurchaseDisplay);
    }

    // Destroy the object
    void DestroyObject()
    {
        // Hiding the popup
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);

        // Handling if the item image is set
        if (m_itemShopUIHandler.m_forDisplayingItemImages != null)
        {
            // Clearing the item image sprite
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = null;
        }

        // Handling if the item name is set
        if (m_itemShopUIHandler.m_itemName != null)
        {
            // Clearing the item name
            m_itemShopUIHandler.m_itemName.text = string.Empty;
        }

        // Handling if the item price is set
        if (m_itemShopUIHandler.m_itemPerPrice != null)
        {
            // Clearing the item price
            m_itemShopUIHandler.m_itemPerPrice.text = string.Empty;
        }

        // Handling if the total price is set
        if (m_itemShopUIHandler.m_itemTotalPrice != null)
        {
            // Clearing the total price
            m_itemShopUIHandler.m_itemTotalPrice.text = string.Empty;
        }

        // Handling if the item quantity is set
        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            // Clearing the item quantity
            m_itemShopUIHandler.m_itemQuantity.text = string.Empty; // Clear item quantity
        }

        // Handling if the current item object is set
        if (m_currentItemObject != null)
        {
            Destroy(m_currentItemObject);
            m_currentItemObject = null;
        }

        // Deactivating the popup
        m_isPopupActive = false;

        // Resetting the item quantity to the initial value
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // Getting the popup active flag
    public bool GetPopupFlag()
    {
        // Returning the popup flag
        return m_isPopupActive;
    }
}
