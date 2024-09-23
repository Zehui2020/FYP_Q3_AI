//----------------------------------------------------------------------
// ItemShopManager
//
// アイテムを管理するクラス
//
// Data: 8/28/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEditorInternal.Profiling.Memory.Experimental;
using UnityEditor.Rendering;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.Collections;

public class ItemShopManager : MonoBehaviour
{
    [Header("アイテムの初期配置座標")]
    public Vector2 m_itemStartPosition;

    [Header("Item Spacing / アイテム間隔")]
    public Vector2 m_itemSpacing;

    [Header("Number of Columns / 列の数")]
    public int m_numberOfColumns = 4; // Defaul: 4

    [Header("アイテムのスケール")]
    public float m_itemScale = 1.0f; // Defaul: 1.0

    // アイテムショップUI管理クラス
    ItemShopUIHandler m_itemShopUIHandler;

    // アイテムの単位
    private string m_itemUnit = " G";

    // アイテムデータ配列
    private ItemData[] m_itemDataArray;

    // アイテム読み込みクラス
    public ItemLoader m_itemLoader;


    private IEnumerator Start()
    {
        // アイテムショップUI管理クラスが設定されていない場合の処理
        if (m_itemShopUIHandler == null)
        {
            // アイテムショップUI管理クラスを設定する
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // アイテム読み込みクラスが設定されていない場合の処理
        if (m_itemLoader == null)
        {
            // アイテム読み込みクラスを設定する
            m_itemLoader = FindAnyObjectByType<ItemLoader>();
        }

        // 出口ボタンが設定されている場合の処理
        if (m_itemShopUIHandler.m_exitTextButton != null)
        {
            // ボタンがクリックされた時にSwitchShopDisplayを呼び出す
            m_itemShopUIHandler.m_exitTextButton.onClick.AddListener(SwitchShopDisplay);
        }

        // アイテム読み込みクラスが設定されている場合の処理
        if (m_itemLoader != null)
        {
            // アイテム読み込みが完了するまで待つ
            yield return new WaitUntil(() => m_itemLoader.IsDataLoaded());

            // アイテムを表示する
            ShowShop();
        }

        // デバッグボタン
        if (m_itemShopUIHandler.m_debugButton != null)
        {
            m_itemShopUIHandler.m_debugButton.onClick.AddListener(SwitchShopDisplay);
        }
    }


    // ショップの表示切り替え
    public void SwitchShopDisplay()
    {
        // ショップが表示されている場合の処理
        if (m_itemShopUIHandler.m_itemShopBackground.activeSelf)
        {
            // ショップを非表示する
            HideShop();
        }
        // ショップが非表示になっている場合の処理
        else
        {
            // ショップを表示にする
            ShowShop();
        }
    }

    // ショップを表示
    private void ShowShop()
    {
        // アイテムを再設定する
        SetItem();
        // ショップを表示する
        m_itemShopUIHandler.m_itemShopBackground.SetActive(true);
    }

    // ショップを非表示
    private void HideShop()
    {
        // ショップを非表示にする
        m_itemShopUIHandler.m_itemShopBackground.SetActive(false);
        // アイテムを削除する
        ClearItems();
    }

    // アイテムの設定
    private void SetItem()
    {
        // ショップのRectTransformを取得
        RectTransform parentRectTransform = m_itemShopUIHandler.m_itemShopBackground.GetComponent<RectTransform>();
        float parentWidth = parentRectTransform.rect.width;
        float parentHeight = parentRectTransform.rect.height;

        // アイテムデータ配列を取得
        m_itemDataArray = m_itemLoader.GetItemDataArray();

        if (m_itemDataArray != null && m_itemDataArray.Length > 0)
        {
            // アイテムを配置する
            ArrangeItems(parentWidth, parentHeight);
        }
    }

    // アイテムの配置
    private void ArrangeItems(float parentWidth, float parentHeight)
    {
        int row = 0;
        int column = 0;
        float itemStartXPosition = m_itemStartPosition.x;
        float itemStartYPosition = m_itemStartPosition.y;

        foreach (ItemData itemData in m_itemDataArray)
        {
            // アイテムの生成と設定する
            RectTransform rectTransform = CreateItem(itemData);

            // アイテムを配置する
            PlaceItem(rectTransform, ref itemStartXPosition, ref itemStartYPosition, row, column);

            // 列と行の更新をする
            UpdateRowAndColumn(ref row, ref column, rectTransform, ref itemStartXPosition, ref itemStartYPosition);
        }
    }

    // アイテムの生成
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

    // アイテムの配置
    private void PlaceItem(RectTransform rectTransform, ref float itemStartXPosition, ref float itemStartYPosition, int row, int column)
    {
        rectTransform.anchoredPosition = new Vector2(
            itemStartXPosition + rectTransform.sizeDelta.x * rectTransform.pivot.x,
            -itemStartYPosition - rectTransform.sizeDelta.y * rectTransform.pivot.y
        );
    }

    // 行と列の更新
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

    // アイテムの削除
    private void ClearItems()
    {
        // ショップのアイテムを取得する(アイテムを設定している親がItemShopObject)
        Transform shopCanvas = m_itemShopUIHandler.m_itemShopBackground.transform.Find("ItemShopObject");
        // アイテムリストの親オブジェクト内の全ての子オブジェクトを削除する
        foreach (Transform child in shopCanvas)
        {
            Destroy(child.gameObject);
        }
    }

    // アイテム単位の取得
    public string GetItemUnit()
    {
        return m_itemUnit;
    }

    // アイテムデータリストを返すメソッド
    public ItemDataList GetItemDataList()
    {
        // ItemDataListを新しく作成し、現在のアイテムデータを設定する
        ItemDataList itemDataList = new ItemDataList();
        itemDataList.items = m_itemDataArray;
        return itemDataList;
    }
}