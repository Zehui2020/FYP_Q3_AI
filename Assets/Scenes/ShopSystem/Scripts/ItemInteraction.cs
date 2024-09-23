//----------------------------------------------------------------------
// ItemInteraction
//
// アイテムとユーザーのやり取りを管理するクラス
//
// Data: 28/8/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine.EventSystems;
using UnityEngine;

public class ItemInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // アイテムエフェクト
    private ItemEffect m_itemEffect;
    // アイテム購入表示クラス
    public ItemPurchaseDisplay m_itemPurchaseDisplay;

    void Start()
    {
        // アイテムエフェクトが設定されていない場合の処理
        if (m_itemEffect == null)
        {
            // アイテムエフェクトを取得する
            m_itemEffect = this.GetComponent<ItemEffect>();
        }
        // アイテム購入表示クラスが設定されていない場合の処理
        if(m_itemPurchaseDisplay == null)
        {
            // アイテム購入表示クラスを設定する
            m_itemPurchaseDisplay = FindAnyObjectByType<ItemPurchaseDisplay>();
        }
    }

    // ポインターがアイテムに触れた時の処理
    public void OnPointerEnter(PointerEventData eventData)
    {
        // スケールエフェクトを発動させる
        m_itemEffect.OnMouseEnter();

        // アイテム購入画面が表示されている場合の処理
        if(m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // エフェクトをしない
            m_itemEffect.StopEffect();
        }
    }

    // ポインターがアイテムから離れた時の処理
    public void OnPointerExit(PointerEventData eventData)
    {
        // スケールエフェクトを発動させる
        m_itemEffect.OnMouseExit();
        // アイテム購入画面が表示されている場合の処理
        if (m_itemPurchaseDisplay.GetPopupFlag() == true)
        {
            // エフェクトをしない
            m_itemEffect.StopEffect();
        }
    }

    // ポインターがアイテムと触れている時にクリックを押した処理
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // クリックされたアイテムのItemDisplayを取得する
            ItemDisplay clickedItemDisplay = eventData.pointerPress.GetComponent<ItemDisplay>();

            if (clickedItemDisplay != null)
            {
                // アイテムエフェクトを開始する
                m_itemEffect.OnMouseEnter();

                // クリックされたアイテムのデータでポップアップ表示する
                m_itemPurchaseDisplay.OnItemClicked(clickedItemDisplay.GetCurrentItemData(), clickedItemDisplay.GetCurrentSpriteDictionary());
                // アイテム購入画面が表示されている場合の処理
                if (m_itemPurchaseDisplay.GetPopupFlag() == true)
                {
                    // エフェクトをしない
                    m_itemEffect.StopEffect();
                }

            }
        }
        else
        {
            // アイテムエフェクトを終了する
            m_itemEffect.OnMouseExit();
        }
    }
}
