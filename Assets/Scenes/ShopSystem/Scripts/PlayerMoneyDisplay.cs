//----------------------------------------------------------------------
// PlayerMoneyDisplay
//
// Class for displaying the player's money
//
// Data: 30/8/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;

public class PlayerMoneyDisplay : MonoBehaviour
{
    [Header("TextMeshProUGUI for displaying player's money")]
    public TextMeshProUGUI m_moneyText;

    // Item manager class
    public ItemShopManager m_itemManager;

    private void Start()
    {
        // If the item manager class is not set
        if (m_itemManager == null)
        {
            // Set the item manager class
            m_itemManager = FindAnyObjectByType<ItemShopManager>();
        }
    }

    void Update()
    {
        // If the money text is set and the player's wallet instance is valid
        if (m_moneyText != null && PlayerWallet.Instance != null)
        {
            // Set the player's money
            m_moneyText.text = "Money : " + PlayerWallet.Instance.GetMoney() + m_itemManager.GetItemUnit();
        }
    }
}
