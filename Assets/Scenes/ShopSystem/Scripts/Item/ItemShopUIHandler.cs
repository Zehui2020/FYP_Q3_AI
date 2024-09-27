//----------------------------------------------------------------------
// ItemShopUIHandler
//
// Item Shop UI management class
//
// Date: 19/9/2024
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

    [Header("Background panel for the purchase screen")]
    public GameObject m_purchaseScreenBackground;

    [Header("Item Shop")]
    public GameObject m_itemShop;

    [Header("Background for the item shop")]
    public GameObject m_itemShopBackground;

    [Header("Object to display the AI conversation")]
    public GameObject m_AIConversationDisplay;

    // GameObject end here ------------------------------------------------





    // Button start here --------------------------------------------------

    [Header("Exit Button")]
    public Button m_exitTextButton;

    [Header("Debug Button")]
    public Button m_debugButton;

    [Header("Purchase Button")]
    public Button m_purchaseButton;

    [Header("Cancel Button")]
    public Button m_cancelButton;

    [Header("Button to end the AI conversation")]
    public Button m_AIConversationEndBotton;

    [Header("Debug Good Button")]
    public Button m_debugGoodBotton;

    [Header("Debug Normal Button")]
    public Button m_debugNormalBotton;

    [Header("Debug Bad Button")]
    public Button m_debugBadBotton;

    // Button end here ----------------------------------------------------





    // Image start here ---------------------------------------------------

    [Header("Image for displaying item images")]
    public Image m_forDisplayingItemImages;

    [Header("Image for the AI system")]
    public Image m_AISystemImage;

    [Header("Background for AI conversation")]
    public Image m_AIConversationBackground;

    // Image end here -----------------------------------------------------





    // TextMeshProUGUI start here -----------------------------------------

    [Header("Item Name")]
    public TextMeshProUGUI m_itemName;

    [Header("Item Price per Unit")]
    public TextMeshProUGUI m_itemPerPrice;

    [Header("Total Price of the Item")]
    public TextMeshProUGUI m_itemTotalPrice;

    [Header("Item Quantity")]
    public TextMeshProUGUI m_itemQuantity;

    [Header("Message when purchasing an item")]
    public TextMeshProUGUI m_itemPurchaseMessage;

    [Header("Display for item purchase amount")]
    public TextMeshProUGUI m_itemPurchaseDisplay;

    // TextMeshProUGUI end here -------------------------------------------

}
