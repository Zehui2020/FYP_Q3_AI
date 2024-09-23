//----------------------------------------------------------------------
// ItemShopUIHandler
//
// アイテムショップUI管理クラス
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

    [Header("アイテムのPrefab")]
    public GameObject m_itemPrefab;

    [Header("購入画面の背景パネル")]
    public GameObject m_purchaseScreenBackground;

    [Header("アイテムショップ")]
    public GameObject m_itemShop;

    [Header("アイテムショップの背景")]
    public GameObject m_itemShopBackground;

    // GameObject end here ------------------------------------------------





    // Button start here --------------------------------------------------

    [Header("出口ボタン")]
    public Button m_exitTextButton;

    [Header("デバッグ用ボタン")]
    public Button m_debugButton;

    [Header("アイテムの個数を増やすボタン")]
    public Button m_plusButton;

    [Header("アイテムの個数を減らすボタン")]
    public Button m_minusButton;

    [Header("購入ボタン")]
    public Button m_purchaseButton;

    [Header("キャンセルボタン")]
    public Button m_cancelButton;

    // Button end here ----------------------------------------------------





    // Image start here ---------------------------------------------------

    //[Header("アイテムのイメージ画像表示用")]
    public Image m_forDisplayingItemImages;

    // Image end here -----------------------------------------------------





    // TextMeshProUGUI start here -----------------------------------------

    [Header("アイテムの名前")]
    public TextMeshProUGUI m_itemName;

    [Header("アイテムの単価")]
    public TextMeshProUGUI m_itemPerPrice;

    [Header("アイテムの合計金額")]
    public TextMeshProUGUI m_itemTotalPrice;

    [Header("アイテムの個数")]
    public TextMeshProUGUI m_itemQuantity;

    [Header("アイテム購入時のメッセージ")]
    public TextMeshProUGUI m_itemPurchaseMessage;

    //[Header("アイテム購入額の表示")]
    public TextMeshProUGUI m_itemPurchaseDisplay;

    // TextMeshProUGUI end here -------------------------------------------

}
