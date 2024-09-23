//----------------------------------------------------------------------
// ItemDisplay
//
// アイテムを表示するためのクラス
//
// Data: 8/28/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemDisplay : MonoBehaviour
{
    //[Header("UI Image for Item Sprite / アイテムスプライト表示用")]
    public Image m_itemImage;
    //[Header("TextMeshProUGUI for Item Name / アイテム名表示用")]
    public TextMeshProUGUI m_itemNameText;
    //[Header("TextMeshProUGUI for Item Price / アイテム価格表示用")]
    public TextMeshProUGUI m_itemPriceText;

    // 現在のアイテムデータ
    private ItemData m_currentItemData;
    // 現在のスプライト情報
    private Dictionary<string, Sprite> m_currentSpriteDictionary;

    // アイテムの設定
    public void Setup(ItemData itemData, Dictionary<string, Sprite> spriteDictionary, string itemUnit)
    {
        // 現在のアイテムデータを設定する
        m_currentItemData = itemData;
        // 現在のスプライト情報を設定する
        m_currentSpriteDictionary = spriteDictionary;

        // アイテムのイメージが設定されている場合、アイテムデータからスプライトを辞書から取得している場合の処理
        if (m_itemImage != null && spriteDictionary.TryGetValue(m_currentItemData.itemSprite, out Sprite sprite))
        {
            // アイテムのスプライトを設定する
            m_itemImage.sprite = sprite;
        }

        // アイテムの名前が設定されている場合の処理
        if (m_itemNameText != null)
        {
            // アイテムの名前を設定する
            m_itemNameText.text = m_currentItemData.itemName;
        }

        // アイテムの価格が設定されている場合の処理
        if (m_itemPriceText != null)
        {
            // アイテムの価格を設定する
            m_itemPriceText.text = m_currentItemData.itemPrice.ToString() + itemUnit;
        }
    }

    // 現在のアイテムデータを取得
    public ItemData GetCurrentItemData()
    {
        return m_currentItemData;
    }

    // 現在のスプライトの情報を取得
    public Dictionary<string, Sprite> GetCurrentSpriteDictionary()
    {
        return m_currentSpriteDictionary;
    }
}
