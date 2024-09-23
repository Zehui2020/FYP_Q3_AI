//----------------------------------------------------------------------
// PlayerMoneyDisplay
//
// プレイヤーの所持金を表示するクラス
//
// Data: 30/8/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;

public class PlayerMoneyDisplay : MonoBehaviour
{
    [Header("TextMeshProUGUI for displaying player's money / プレイヤーの所持金表示用")]
    public TextMeshProUGUI m_moneyText;

    // アイテムマネージャークラス
    public ItemShopManager m_itemManager;
    private void Start()
    {
        // アイテムマネージャークラスが設定されていない場合の処理
        if(m_itemManager == null)
        {
            // アイテムマネージャークラスを設定する
            m_itemManager = FindAnyObjectByType<ItemShopManager>();
        }
    }

    void Update()
    {
        // プレイヤーの所持金がテキストに設定されている場合、プレイヤーの所持金のインスタンスが有効な場合の処理
        if (m_moneyText != null && PlayerWallet.Instance != null)
        {
            // プレイヤーの所持金を設定する
            m_moneyText.text = "Money : " + PlayerWallet.Instance.GetMoney() + m_itemManager.GetItemUnit();
        }
    }
}
