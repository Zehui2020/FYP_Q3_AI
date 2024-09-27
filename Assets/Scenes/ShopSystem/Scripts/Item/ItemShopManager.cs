//----------------------------------------------------------------------
// ItemShopManager
//
// Class to manage items
//
// Date: 28/8/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using System.Collections;

public class ItemShopManager : MonoBehaviour
{
    [Header("Initial item placement coordinates")]
    public Vector2 m_itemStartPosition;

    [Header("Item Spacing")]
    public Vector2 m_itemSpacing;

    [Header("Number of Columns")]
    public int m_numberOfColumns = 4; // Default: 4

    [Header("Item Scale")]
    public float m_itemScale = 1.0f;  // Default: 1.0

    // Item unit
    private string m_itemUnit = " G";

    // Array of item data
    private ItemData[] m_itemDataArray;

    // Item shop UI handler class
    public ItemShopUIHandler m_itemShopUIHandler;

    // Item loader class
    public ItemLoader m_itemLoader;

    // AI system manager class
    public AISystemManager m_AISystemManager;

    // Texture collision detection class
    public TextureCollision m_textureCollision;

    // Flag indicating if the shop was closed due to collision
    private bool m_isShopClosedByCollision = false;

    private IEnumerator Start()
    {
        // If the item shop UI handler class is not set
        if (m_itemShopUIHandler == null)
        {
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // If the AI system manager class is not set
        if (m_AISystemManager == null)
        {
            m_AISystemManager = FindAnyObjectByType<AISystemManager>();
        }

        // If the texture collision detection class is not set
        if (m_textureCollision == null)
        {
            m_textureCollision = FindAnyObjectByType<TextureCollision>();
        }

        // If the item loader class is not set
        if (m_itemLoader == null)
        {
            m_itemLoader = FindAnyObjectByType<ItemLoader>();
        }

        // If the item loader class is set
        if (m_itemLoader != null)
        {
            yield return new WaitUntil(() => m_itemLoader.IsDataLoaded());
            ShowShop();
            // Set item data for the AI system manager class
            m_AISystemManager.SetItemData(m_itemLoader.m_itemData);
        }

        // Debug button
        if (m_itemShopUIHandler.m_debugButton != null)
        {
            m_itemShopUIHandler.m_debugButton.onClick.AddListener(SwitchShopDisplay);
        }

        // Handle the button to end AI conversation
        m_itemShopUIHandler.m_AIConversationEndBotton.onClick.AddListener(SwitchShopDisplay);

        // Debug buttons
        m_itemShopUIHandler.m_debugBadBotton.onClick.AddListener(SwitchShopDisplay);
        m_itemShopUIHandler.m_debugGoodBotton.onClick.AddListener(SwitchShopDisplay);
        m_itemShopUIHandler.m_debugNormalBotton.onClick.AddListener(SwitchShopDisplay);
    }

    void Update()
    {
        // If there is a collision and the shop has not yet been closed
        if (m_textureCollision != null && m_textureCollision.GetTransparencyHitDetectionFlag() && !m_isShopClosedByCollision)
        {
            // Close the shop
            HideShop();
            // Set the shop as closed
            m_isShopClosedByCollision = true;
            // Display the AI conversation
            m_AISystemManager.ShowAIConversationDisplay();
        }

        // Reset collision detection on click and allow the shop to be closed
        if (Input.GetMouseButtonDown(0))
        {
            // Prevent the shop from closing when an item is clicked
            if (IsItemClicked())
            {
                return;
            }

            if (m_textureCollision != null && m_textureCollision.GetTransparencyHitDetectionFlag() == true)
            {
                // If the shop is displayed
                if (m_itemShopUIHandler.m_itemShopBackground.activeSelf)
                {
                    // Close the shop
                    HideShop();
                    // Set the shop as closed
                    m_isShopClosedByCollision = true;
                }
                // If the shop is not displayed
                else
                {
                    // Reset the flag to allow reopening
                    m_isShopClosedByCollision = false;
                }
            }
        }
    }

    // Check if an item has been clicked
    private bool IsItemClicked()
    {
        // Use raycast to detect if an item was hit
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider != null && hit.collider.gameObject.CompareTag("Item"))
        {
            // If an item was clicked, do not close the shop
            return true;
        }
        return false;
    }

    // Toggle the shop display
    public void SwitchShopDisplay()
    {
        if (m_itemShopUIHandler.m_itemShopBackground.activeSelf)
        {
            // Close the shop
            HideShop();
        }
        else
        {
            // Show the shop
            ShowShop();
        }
    }

    // Show the shop
    private void ShowShop()
    {
        // Display the shop background
        m_itemShopUIHandler.m_itemShopBackground.SetActive(true);

        // Reset the shop closed flag
        m_isShopClosedByCollision = false;

        // Hide the AI system
        m_AISystemManager.HideAIConversationDisplay();
        // Set items in place
        SetItem();

        // Reset the image collision detection
        if (m_textureCollision != null)
        {
            m_textureCollision.ResetHitDetection();
        }
    }

    // Hide the shop
    private void HideShop()
    {
        // Hide the shop background
        m_itemShopUIHandler.m_itemShopBackground.SetActive(false);
        // Remove items
        ClearItems();

        // Display the AI system
        m_AISystemManager.ShowAIConversationDisplay();
    }

    // Set items in place
    private void SetItem()
    {
        RectTransform parentRectTransform = m_itemShopUIHandler.m_itemShopBackground.GetComponent<RectTransform>();
        float parentWidth = parentRectTransform.rect.width;
        float parentHeight = parentRectTransform.rect.height;

        m_itemDataArray = m_itemLoader.GetItemDataArray();

        if (m_itemDataArray != null && m_itemDataArray.Length > 0)
        {
            ArrangeItems(parentWidth, parentHeight);
        }
    }

    // Arrange items in the shop
    private void ArrangeItems(float parentWidth, float parentHeight)
    {
        int row = 0;
        int column = 0;
        float itemStartXPosition = m_itemStartPosition.x;
        float itemStartYPosition = m_itemStartPosition.y;

        foreach (ItemData itemData in m_itemDataArray)
        {
            RectTransform rectTransform = CreateItem(itemData);
            PlaceItem(rectTransform, ref itemStartXPosition, ref itemStartYPosition, row, column);
            UpdateRowAndColumn(ref row, ref column, rectTransform, ref itemStartXPosition, ref itemStartYPosition);
        }
        // Reset mood to neutral
        m_AISystemManager.SetMood(0);
    }

    // Create an item
    private RectTransform CreateItem(ItemData itemData)
    {
        GameObject itemPrefab = Instantiate(m_itemShopUIHandler.m_itemPrefab, m_itemShopUIHandler.m_itemShop.transform);
        ItemDisplay itemDisplay = itemPrefab.GetComponent<ItemDisplay>();

        if (itemDisplay != null)
        {
            // Set the item
            itemDisplay.Setup(itemData, m_itemLoader.GetSpriteDictionary(), m_itemUnit);

            // Set the item data for the AI system manager
            m_AISystemManager.SetItemDataArray(m_itemDataArray);

            // If the mood is not neutral
            if (m_AISystemManager.GetMood() != 0)
            {
                // Adjust price based on fluctuations
                int fluctuatedPrice = m_AISystemManager.GetItemPriceFluctuations(itemData.itemName);
                itemDisplay.SetItemPrice(fluctuatedPrice);
            }

            // Set the item
            itemDisplay.Setup(itemData, m_itemLoader.GetSpriteDictionary(), m_itemUnit);
        }

        RectTransform rectTransform = itemPrefab.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
        rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale *= m_itemScale;

        return rectTransform;
    }

    // Place the item in the shop
    private void PlaceItem(RectTransform rectTransform, ref float itemStartXPosition, ref float itemStartYPosition, int row, int column)
    {
        rectTransform.anchoredPosition = new Vector2(
            itemStartXPosition + rectTransform.sizeDelta.x * rectTransform.pivot.x,
            -itemStartYPosition - rectTransform.sizeDelta.y * rectTransform.pivot.y
        );
    }

    // Update row and column positions for item placement
    private void UpdateRowAndColumn(ref int row, ref int column, RectTransform rectTransform, ref float itemStartXPosition, ref float itemStartYPosition)
    {
        column++;
        if (column >= m_numberOfColumns)
        {
            column = 0;
            row++;
            itemStartXPosition = m_itemStartPosition.x;
            itemStartYPosition += rectTransform.sizeDelta.y + m_itemSpacing.y;
        }
        else
        {
            itemStartXPosition += rectTransform.sizeDelta.x + m_itemSpacing.x;
        }
    }

    // Clear all items
    private void ClearItems()
    {
        Transform shopCanvas = m_itemShopUIHandler.m_itemShopBackground.transform.Find("ItemShopObject");
        foreach (Transform child in shopCanvas)
        {
            Destroy(child.gameObject);
        }
    }

    // Get the item unit
    public string GetItemUnit()
    {
        return m_itemUnit;
    }

    // Get the array of item data
    public ItemData[] GetItemDataArray()
    {
        return m_itemDataArray;
    }
}
