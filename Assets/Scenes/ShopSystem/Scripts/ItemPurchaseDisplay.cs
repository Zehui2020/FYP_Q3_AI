//----------------------------------------------------------------------
// ItemPurchaseDisplay
//
// アイテムの購入表示を管理するクラス
//
// Data: 2/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

public class ItemPurchaseDisplay : MonoBehaviour
{
    [Header("アイテムのイメージ画像を描画する座標")]
    public Vector2 m_ItemImageTexturePosition;

    // アイテムデータ
    private ItemData m_itemData;
    // 現在のアイテムオブジェクト
    private GameObject m_currentItemObject;
    // ポップアップしているかどうかのフラグ
    private bool m_isPopupActive = false;
    // アイテムの個数
    private int m_itemQuantity = 1;
    // 現在のアイテムの個数
    private int m_currentItemMQuantity;
    // アイテムの個数の最小値
    private const int ITEMQUANTITY_MIN = 1;
    // アイテムの個数の初期値
    private const int ITEMQUANTITY_INITIAL = 1;
    // アイテムの合計金額
    private int m_itemTotalPrice;


    // ボタンが長押しされているかのフラグ
    private bool m_isPlusButtonHeld = false;
    private bool m_isMinusButtonHeld = false;
    private float m_holdDuration = 0.0f;
    private const float HOLD_THRESHOLD = 0.5f;  // 長押しの閾値
    private const float HOLD_REPEAT_RATE = 0.1f;  // 長押し後の繰り返し速度

    // アイテムマネージャークラス
    public ItemShopManager m_itemManager;

    // 購入額表示用クラス
    TextFadeAndMove m_textFadeAndMove;

    // アイテムショップUI管理クラス
    ItemShopUIHandler m_itemShopUIHandler;

    void Start()
    {
        // アイテムショップUI管理クラスが設定されていない場合の処理
        if (m_itemShopUIHandler == null)
        {
            // アイテムショップUI管理クラスを設定する
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // アイテムマネージャークラスが設定されていない場合の処理
        if (m_itemManager == null)
        {
            // アイテムマネージャークラスを設定する
            m_itemManager = FindAnyObjectByType<ItemShopManager>();
        }

        // アイテム購入額表示用クラスが設定されていない場合の処理
        if(m_textFadeAndMove == null)
        {
            // アイテム購入額表示用クラスを設定する
            m_textFadeAndMove = FindAnyObjectByType<TextFadeAndMove>();
        }

        // ポップアップパネルを非表示にする
        m_itemShopUIHandler.m_itemShopBackground.SetActive(false);
        // 購入ボタンが押された時の処理
        m_itemShopUIHandler.m_purchaseButton.onClick.AddListener(PurchaseButtonClicked);
        // キャンセルボタンが押された時の処理
        m_itemShopUIHandler.m_cancelButton.onClick.AddListener(CancelButtonClicked);

        // アイテムの個数を増やすボタンが押された時の処理
        m_itemShopUIHandler.m_plusButton.onClick.AddListener(IncreaseQuantity);

        // +ボタンにEventTriggerを追加して、長押し時の処理を設定
        var plusButtonTrigger = m_itemShopUIHandler.m_plusButton.gameObject.AddComponent<EventTrigger>();

        // PointerDownイベント（ボタンが押された時）の処理
        var plusDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        plusDownEntry.callback.AddListener((data) => { StartButtonHold(true); }); // 長押し開始
        plusButtonTrigger.triggers.Add(plusDownEntry);

        // PointerUpイベント（ボタンが離された時）の処理
        var plusUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        plusUpEntry.callback.AddListener((data) => { StopButtonHold(); }); // 長押し停止
        plusButtonTrigger.triggers.Add(plusUpEntry);

        // アイテムの個数を減らすボタンが押された時の処理
        m_itemShopUIHandler.m_minusButton.onClick.AddListener(DecreaseQuantity);

        // -ボタンにEventTriggerを追加して、長押し時の処理を設定
        var minusButtonTrigger = m_itemShopUIHandler.m_minusButton.gameObject.AddComponent<EventTrigger>();

        // PointerDownイベント（ボタンが押された時）の処理
        var minusDownEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerDown };
        minusDownEntry.callback.AddListener((data) => { StartButtonHold(false); }); // 長押し開始
        minusButtonTrigger.triggers.Add(minusDownEntry);

        // PointerUpイベント（ボタンが離された時）の処理
        var minusUpEntry = new EventTrigger.Entry { eventID = EventTriggerType.PointerUp };
        minusUpEntry.callback.AddListener((data) => { StopButtonHold(); }); // 長押し停止
        minusButtonTrigger.triggers.Add(minusUpEntry);


        // 初期状態で購入画面を非表示にする
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);
        // 初期状態で購入結果のメッセージを非表示にする
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);


    }

    // アイテムをクリックした時の処理
    public void OnItemClicked(ItemData itemdata, Dictionary<string, Sprite> spriteDictionary)
    {
        // ポップアップが表示されている間は他のアイテムをクリックできないようにする
        if (m_isPopupActive)
        {
            return;
        }

        // 前回のアイテムが残っている場合の処理
        if (m_currentItemObject != null)
        {
            // 前回のアイテムを削除する
            Destroy(m_currentItemObject);
        }

        // アイテムのイメージ画像のRectTransformを取得する
        RectTransform rectTransform = m_itemShopUIHandler.m_forDisplayingItemImages.GetComponent<RectTransform>();

        // アンカーとピボットの設定を中央に変更する
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // 親オブジェクトを基準とし、指定した座標分移動させる
        rectTransform.anchoredPosition += new Vector2(m_ItemImageTexturePosition.x, -m_ItemImageTexturePosition.y);

        // アイテムデータを設定する
        m_itemData = itemdata;

        // アイテムのイメージ画像が設定されている場合、アイテムデータからスプライトを辞書から取得できる場合の処理
        if (m_itemShopUIHandler.m_forDisplayingItemImages != null && spriteDictionary.TryGetValue(m_itemData.itemSprite, out Sprite sprite))
        {
            // アイテムのイメージ画像を設定する
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = sprite;
        }
        // アイテムの名前を設定する
        m_itemShopUIHandler.m_itemName.text = itemdata.itemName;
        // アイテム1つの値段を設定する
        m_itemShopUIHandler.m_itemPerPrice.text = itemdata.itemPrice.ToString() + m_itemManager.GetItemUnit();
        // アイテムの個数を設定する
        m_currentItemMQuantity = itemdata.itemQuantity;
        // アイテムの個数を更新する
        UpdateItemQuantity();
        // 合計金額を更新する
        UpdateTotalPrice();
        // ポップアップパネルを表示する
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(true);
        // ポップアップをアクティブに設定する
        m_isPopupActive = true;
    }

    // 購入するアイテムの個数を増やす
    private void IncreaseQuantity()
    {
        // アイテムの個数の上限値を決める
        if (m_itemQuantity < m_currentItemMQuantity)
        {
            // アイテムの個数を増やす
            m_itemQuantity++;
            UpdateItemQuantity();
            // 合計金額を更新する
            UpdateTotalPrice();
        }

    }

    // 購入するアイテムの個数を減らす
    private void DecreaseQuantity()
    {
        // アイテムの個数が最小値未満になった場合の処理
        if (m_itemQuantity > ITEMQUANTITY_MIN)
        {
            // アイテムの個数を減らす
            m_itemQuantity--;
            UpdateItemQuantity();
            // 合計金額を更新する
            UpdateTotalPrice();
        }
    }

    // アイテムの個数を更新する
    private void UpdateItemQuantity()
    {
        // アイテムの個数を表示するテキストが設定されている場合の処理
        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            // アイテムの個数を設定する
            m_itemShopUIHandler.m_itemQuantity.text = "x" + m_itemQuantity.ToString();
        }
    }

    // 合計金額を更新する
    private void UpdateTotalPrice()
    {
        // アイテムデータが設定されている場合の処理
        if (m_itemData != null)
        {
            // 合計金額を設定する
            m_itemTotalPrice = m_itemData.itemPrice * m_itemQuantity;
            m_itemShopUIHandler.m_itemTotalPrice.text = m_itemTotalPrice.ToString() + m_itemManager.GetItemUnit(); ;

            // プレイヤーの所持金よりも購入金額が高かった場合の処理
            if (PlayerWallet.Instance.GetMoney() < m_itemTotalPrice)
            {
                // テキストの色を赤色にする
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.red;
            }
            // プレイヤーの所持金よりも購入金額が低かった場合の処理
            else
            {
                // テキストの色を白色にする
                m_itemShopUIHandler.m_itemTotalPrice.color = Color.white;
            }
        }
    }

    // 購入ボタンの処理
    public void PurchaseButtonClicked()
    {
        // プレイヤーの所持金が購入金額より少ない場合の処理
        if (m_itemTotalPrice > PlayerWallet.Instance.GetMoney())
        {
            // 購入に失敗しました
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase failed";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.red;
        }
        else
        {
            // アイテムを購入する
            PurchaseItem(m_itemQuantity);
            // 購入に成功しました
            m_itemShopUIHandler.m_itemPurchaseMessage.text = "Purchase successful";
            m_itemShopUIHandler.m_itemPurchaseMessage.color = Color.green;


            // 購入金額をテキストとして表示
            StartCoroutine(m_textFadeAndMove.FadeMoveAndResetText("-", m_itemTotalPrice, m_itemManager.GetItemUnit()));

        }

        // 購入結果のメッセージを表示する
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(true);

        // メッセージを一定時間後(秒)に非表示にする
        StartCoroutine(HideMessageAfterDelay(2.0f));
    }

    // ボタンが長押しされ始めた時の処理
    private void StartButtonHold(bool isPlusButton)
    {
        // 長押しフラグを設定する
        if (isPlusButton)
        {
            m_isPlusButtonHeld = true;
        }
        else
        {
            m_isMinusButtonHeld = true;
        }

        // コルーチンを開始する
        StartCoroutine(HoldButtonCoroutine(isPlusButton));
    }

    // ボタンが長押しされた時の処理
    private IEnumerator HoldButtonCoroutine(bool isPlusButton)
    {
        // 一定時間長押しされるまで待機
        yield return new WaitForSeconds(HOLD_THRESHOLD);

        // ボタンが押され続けている間の処理
        while (isPlusButton ? m_isPlusButtonHeld : m_isMinusButtonHeld)
        {
            // アイテムの個数を増減する
            if (isPlusButton)
            {
                IncreaseQuantity();
            }
            else
            {
                DecreaseQuantity();
            }

            // 繰り返し速度を制御
            yield return new WaitForSeconds(HOLD_REPEAT_RATE);
        }
    }

    // ボタンが離された時の処理
    private void StopButtonHold()
    {
        // 長押しフラグを解除する
        m_isPlusButtonHeld = false;
        m_isMinusButtonHeld = false;
    }


    // 購入するアイテムの処理
    private void PurchaseItem(int quantity)
    {
        // アイテムデータが設定されている場合の処理
        if (m_itemData != null)
        {
            // プレイヤーの所持金から減らす
            PlayerWallet.Instance.SpendMoney(m_itemData.itemPrice * quantity);
        }
        // 生成したオブジェクトを削除する
        DestroyObject();
        // アイテムの個数を初期化する
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // キャンセルボタンの処理
    public void CancelButtonClicked()
    {
        // アイテムを購入することをやめる
        CancelItem();
    }

    // キャンセルするアイテムの処理
    void CancelItem()
    {
        // 生成したオブジェクトを削除する
        DestroyObject();
        // アイテムの個数を初期化する
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }

    // 一定時間経過後にメッセージを非表示にする処理
    private IEnumerator HideMessageAfterDelay(float delay)
    {
        // 指定した時間待機する
        yield return new WaitForSeconds(delay);

        // メッセージを非表示にする
        m_itemShopUIHandler.m_itemPurchaseMessage.gameObject.SetActive(false);
    }

    // オブジェクトを消去する
    void DestroyObject()
    {
        // ポップアップを非表示にする
        m_itemShopUIHandler.m_purchaseScreenBackground.SetActive(false);

        // アイテム画像が設定されている場合の処理
        if (m_itemShopUIHandler.m_forDisplayingItemImages != null)
        {
            // アイテム画像のスプライトを削除する
            m_itemShopUIHandler.m_forDisplayingItemImages.sprite = null;
        }

        // アイテムの名前が設定されている場合の処理
        if (m_itemShopUIHandler.m_itemName != null)
        {
            // アイテムの名前を削除する
            m_itemShopUIHandler.m_itemName.text = string.Empty;
        }

        // アイテムの単価が設定されている場合の処理
        if (m_itemShopUIHandler.m_itemPerPrice != null)
        {
            // アイテムの単価を削除する
            m_itemShopUIHandler.m_itemPerPrice.text = string.Empty;
        }

        // アイテムの合計金額が設定されている場合の処理
        if (m_itemShopUIHandler.m_itemTotalPrice != null)
        {
            // アイテムの合計金額を削除する
            m_itemShopUIHandler.m_itemTotalPrice.text = string.Empty;
        }

        // アイテムの個数が設定されている場合の処理
        if (m_itemShopUIHandler.m_itemQuantity != null)
        {
            // アイテムの個数を削除する
            m_itemShopUIHandler.m_itemQuantity.text = string.Empty; // アイテムの個数をクリア
        }

        // 現在のアイテムオブジェクトが設定されている場合の処理
        if (m_currentItemObject != null)
        {
            Destroy(m_currentItemObject);
            m_currentItemObject = null;
        }

        // ポップアップを非アクティブにする
        m_isPopupActive = false;

        // アイテムの個数を初期値にする
        m_itemQuantity = ITEMQUANTITY_INITIAL;
    }


    // ポップアップしているかどうかのフラグを取得する
    public bool GetPopupFlag()
    {
        // ポップアップフラグを返す
        return m_isPopupActive;
    }
}