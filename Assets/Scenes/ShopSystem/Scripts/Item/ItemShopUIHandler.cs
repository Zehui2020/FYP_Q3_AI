//----------------------------------------------------------------------
// ItemShopUIHandler
//
// Class to manage the item shop UI
//
// Date: 19/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopUIHandler : MonoBehaviour
{
    // GameObject section start -------------------------------------------

    [Header("Item Prefab")]
    public GameObject m_itemPrefab;

    [Header("Background panel for purchase screen")]
    public GameObject m_purchaseScreenBackground;

    [Header("Item shop")]
    public GameObject m_itemShop;

    [Header("Background of the item shop")]
    public GameObject m_itemShopBackground;

    [Header("Object to display conversation with AI")]
    public GameObject m_AIConversationDisplay;

    // GameObject section end ---------------------------------------------




    // Button section start -----------------------------------------------

    [Header("Exit button")]
    public Button m_exitTextButton;

    [Header("Debug button")]
    public Button m_debugButton;

    [Header("Purchase button")]
    public Button m_purchaseButton;

    [Header("Cancel button")]
    public Button m_cancelButton;

    [Header("Button to end conversation with AI")]
    public Button m_AIConversationEndBotton;

    [Header("Debug Good button")]
    public Button m_debugGoodBotton;

    [Header("Debug Normal button")]
    public Button m_debugNormalBotton;

    [Header("Debug Bad button")]
    public Button m_debugBadBotton;

    // Button section end -------------------------------------------------




    // Image section start ------------------------------------------------

    [Header("Image display for item")]
    public Image m_forDisplayingItemImages;

    [Header("Image for AI system")]
    public Image m_AISystemImage;

    [Header("Background for conversation with AI")]
    public Image m_AIConversationBackground;

    // Image section end --------------------------------------------------




    // TextMeshProUGUI section start --------------------------------------

    [Header("Item name")]
    public TextMeshProUGUI m_itemName;

    [Header("Unit price of item")]
    public TextMeshProUGUI m_itemPerPrice;

    [Header("Total price of item")]
    public TextMeshProUGUI m_itemTotalPrice;

    [Header("Item quantity")]
    public TextMeshProUGUI m_itemQuantity;

    [Header("Message when item is purchased")]
    public TextMeshProUGUI m_itemPurchaseMessage;

    [Header("Display for item purchase amount")]
    public TextMeshProUGUI m_itemPurchaseDisplay;

    // TextMeshProUGUI section end ----------------------------------------

}
