//----------------------------------------------------------------------
// ItemShopUIHandler
//
// Class for managing the item shop UI
//
// Data: 19/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopUIHandler : MonoBehaviour
{
    // GameObject start here ----------------------------------------------

    [Header("Item Prefab")]
    public GameObject m_itemPrefab;

    [Header("Purchase Screen Background Panel")]
    public GameObject m_purchaseScreenBackground;

    [Header("Item Shop")]
    public GameObject m_itemShop;

    [Header("Item Shop Background")]
    public GameObject m_itemShopBackground;

    // GameObject end here ------------------------------------------------




    // Button start here --------------------------------------------------

    [Header("Exit Button")]
    public Button m_exitTextButton;

    [Header("Debug Button")]
    public Button m_debugButton;

    [Header("Increase Item Quantity Button")]
    public Button m_plusButton;

    [Header("Decrease Item Quantity Button")]
    public Button m_minusButton;

    [Header("Purchase Button")]
    public Button m_purchaseButton;

    [Header("Cancel Button")]
    public Button m_cancelButton;

    // Button end here ----------------------------------------------------




    // Image start here ---------------------------------------------------

    [Header("Image Display for Items")]
    public Image m_forDisplayingItemImages;

    // Image end here -----------------------------------------------------




    // TextMeshProUGUI start here -----------------------------------------

    [Header("Item Name")]
    public TextMeshProUGUI m_itemName;

    [Header("Item Unit Price")]
    public TextMeshProUGUI m_itemPerPrice;

    [Header("Item Total Price")]
    public TextMeshProUGUI m_itemTotalPrice;

    [Header("Item Quantity")]
    public TextMeshProUGUI m_itemQuantity;

    [Header("Item Purchase Message")]
    public TextMeshProUGUI m_itemPurchaseMessage;

    [Header("Item Purchase Amount Display")]
    public TextMeshProUGUI m_itemPurchaseDisplay;

    // TextMeshProUGUI end here -------------------------------------------
}
