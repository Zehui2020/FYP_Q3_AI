//----------------------------------------------------------------------
// ItemLoader
//
// アイテムを読み込むクラス
//
// Data: 9/13/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class ItemData
{
    // jsonファイルで読み込むため名前はjsonファイルと同じ名前を使用する
    [Header("Item Sprite Path / アイテムスプライトのパス")]
    public string itemSprite;
    [Header("Item Name / アイテム名")]
    public string itemName;
    [Header("Item Price / アイテムの値段")]
    public int itemPrice;
    [Header("アイテムの個数")]
    public int itemQuantity;
}

[System.Serializable]
public class ItemDataList
{
    // jsonファイルで読み込むため名前はjsonファイルと同じ名前を使用する
    [Header("List of Items / アイテムのリスト")]
    public ItemData[] items;
}

public class ItemLoader : MonoBehaviour
{
    [Header("Path to JSON File / JSONファイルのパス")]
    public string m_jsonFilePath = "items";

    // アイテムデータの配列
    private ItemData[] m_itemDataArray;
    // アイテムデータ
    public ItemData m_itemData;
    // アイテムデータリスト
    public ItemDataList m_itemDataList;
    // スプライトを名前で管理する辞書
    private Dictionary<string, Sprite> m_spriteDictionary;

    private void Start()
    {
        // スプライトを一括でロードして辞書に格納する
        LoadAllSprites();

        // JSONファイルからアイテムデータを読み込む
        m_itemDataList = LoadItemData();
        // アイテムデータリストが設定されている場合の処理
        if (m_itemDataList != null)
        {
            // アイテムを設定する
            m_itemDataArray = m_itemDataList.items;
        }

    }

    // すべてのスクリプトを読み込む
    private void LoadAllSprites()
    {
        // "items"フォルダから全てのスプライトを読み込む
        Sprite[] sprites = Resources.LoadAll<Sprite>("items");

        // スプライトの名前とスプライト本体を格納する辞書を初期化する
        m_spriteDictionary = new Dictionary<string, Sprite>();

        // 読み込んだスプライトを辞書に追加する
        foreach (Sprite sprite in sprites)
        {
            m_spriteDictionary[sprite.name] = sprite;
        }
    }

    // アイテムを読み込む
    private ItemDataList LoadItemData()
    {
        // ResourcesフォルダからJSONファイルを読み込む
        TextAsset jsonText = Resources.Load<TextAsset>(m_jsonFilePath);

        // JSONデータをItemDataListに変換
        ItemDataList itemDataList = JsonUtility.FromJson<ItemDataList>(jsonText.text);
        return itemDataList;
    }

    // データがロードされているかを確認
    public bool IsDataLoaded()
    {
        // アイテムデータとスプライト辞書がロードされているかどうかを確認する
        return m_itemDataArray != null && m_itemDataArray.Length > 0 && m_spriteDictionary != null && m_spriteDictionary.Count > 0;
    }

    // アイテムデータ取得
    public ItemData GetItemData()
    {
        return m_itemData;
    }
    // アイテムデータ配列取得
    public ItemData[] GetItemDataArray()
    {
        return m_itemDataArray;
    }
    // スプライト取得
    public Dictionary<string, Sprite> GetSpriteDictionary()
    {
        return m_spriteDictionary;
    }
}