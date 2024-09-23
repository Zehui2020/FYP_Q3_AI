//----------------------------------------------------------------------
// ItemShopManager
//
// Class for managing the item shop
//
// Data: 8/28/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class ItemShopManager : MonoBehaviour
{
    [Header("Initial Item Position")]
    public Vector2 m_itemStartPosition;

    [Header("Item Spacing")]
    public Vector2 m_itemSpacing;

    [Header("Number of Columns")]
    public int m_numberOfColumns = 4; // Default: 4

    [Header("Item Scale")]
    public float m_itemScale = 1.0f; // Default: 1.0

    // Item Shop UI Manager
    ItemShopUIHandler m_itemShopUIHandler;

    // Item Unit
    private string m_itemUnit = " G";

    // Item Data Array
    private ItemData[] m_itemDataArray;

    // Item Loader Class
    public ItemLoader m_itemLoader;

    private IEnumerator Start()
    {
        // Currently, when the Exit button is pressed, it is set to exit the scene. If you want to change this behavior, I recommend modifying the SceneExitHandler class.

        // Check if the Item Shop UI Manager is set
        if (m_itemShopUIHandler == null)
        {
            // Set the Item Shop UI Manager
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // Check if the Item Loader is set
        if (m_itemLoader == null)
        {
            // Set the Item Loader
            m_itemLoader = FindAnyObjectByType<ItemLoader>();
        }

        // If the Item Loader is set
        if (m_itemLoader != null)
        {
            // Wait until the item loading is complete
            yield return new WaitUntil(() => m_itemLoader.IsDataLoaded());

            // Show the shop
            ShowShop();
        }

        // Debug button
        if (m_itemShopUIHandler.m_debugButton != null)
        {
            m_itemShopUIHandler.m_debugButton.onClick.AddListener(SwitchShopDisplay);
        }
    }

    // Switch shop display
    public void SwitchShopDisplay()
    {
        // If the shop is currently displayed
        if (m_itemShopUIHandler.m_itemShopBackground.activeSelf)
        {
            // Hide the shop
            HideShop();
        }
        // If the shop is currently hidden
        else
        {
            // Show the shop
            ShowShop();
        }
    }

    // Show the shop
    private void ShowShop()
    {
        // Reset the items
        SetItem();
        // Display the shop
        m_itemShopUIHandler.m_itemShopBackground.SetActive(true);
    }

    // Hide the shop
    private void HideShop()
    {
        // Hide the shop
        m_itemShopUIHandler.m_itemShopBackground.SetActive(false);
        // Clear the items
        ClearItems();
    }

    // Set the items
    private void SetItem()
    {
        // Get the RectTransform of the shop
        RectTransform parentRectTransform = m_itemShopUIHandler.m_itemShopBackground.GetComponent<RectTransform>();
        float parentWidth = parentRectTransform.rect.width;
        float parentHeight = parentRectTransform.rect.height;

        // Get the item data array
        m_itemDataArray = m_itemLoader.GetItemDataArray();

        if (m_itemDataArray != null && m_itemDataArray.Length > 0)
        {
            // Arrange the items
            ArrangeItems(parentWidth, parentHeight);
        }
    }

    // Arrange the items
    private void ArrangeItems(float parentWidth, float parentHeight)
    {
        int row = 0;
        int column = 0;
        float itemStartXPosition = m_itemStartPosition.x;
        float itemStartYPosition = m_itemStartPosition.y;

        foreach (ItemData itemData in m_itemDataArray)
        {
            // Create and set the item
            RectTransform rectTransform = CreateItem(itemData);

            // Place the item
            PlaceItem(rectTransform, ref itemStartXPosition, ref itemStartYPosition, row, column);

            // Update the row and column
            UpdateRowAndColumn(ref row, ref column, rectTransform, ref itemStartXPosition, ref itemStartYPosition);
        }
    }

    // Create the item
    private RectTransform CreateItem(ItemData itemData)
    {
        GameObject itemPrefab = Instantiate(m_itemShopUIHandler.m_itemPrefab, m_itemShopUIHandler.m_itemShop.transform);

        ItemDisplay itemDisplay = itemPrefab.GetComponent<ItemDisplay>();
        if (itemDisplay != null)
        {
            itemDisplay.Setup(itemData, m_itemLoader.GetSpriteDictionary(), m_itemUnit);
        }

        RectTransform rectTransform = itemPrefab.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.0f, 1.0f);
        rectTransform.anchorMax = new Vector2(0.0f, 1.0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localScale *= m_itemScale;

        return rectTransform;
    }

    // Place the item
    private void PlaceItem(RectTransform rectTransform, ref float itemStartXPosition, ref float itemStartYPosition, int row, int column)
    {
        rectTransform.anchoredPosition = new Vector2(
            itemStartXPosition + rectTransform.sizeDelta.x * rectTransform.pivot.x,
            -itemStartYPosition - rectTransform.sizeDelta.y * rectTransform.pivot.y
        );
    }

    // Update the row and column
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

    // Clear the items
    private void ClearItems()
    {
        // Get the shop items (the parent for the items is ItemShopObject)
        Transform shopCanvas = m_itemShopUIHandler.m_itemShopBackground.transform.Find("ItemShopObject");
        // Destroy all child objects within the item list parent object
        foreach (Transform child in shopCanvas)
        {
            Destroy(child.gameObject);
        }
    }

    // Get item unit
    public string GetItemUnit()
    {
        return m_itemUnit;
    }

    // Return the item data list
    public ItemDataList GetItemDataList()
    {
        // Create a new ItemDataList and set the current item data
        ItemDataList itemDataList = new ItemDataList();
        itemDataList.items = m_itemDataArray;
        return itemDataList;
    }
}
