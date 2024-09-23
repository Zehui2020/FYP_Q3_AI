//----------------------------------------------------------------------
// TextFadeAndMove
//
// 購入時の金額を表示するクラス
//
// Data: 17/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------
using System.Collections;
using TMPro;
using UnityEngine;

public class TextFadeAndMove : MonoBehaviour
{
    [Header("Fade and Move Duration")]
    public float m_duration = 2.0f;

    [Header("Move Distance")]
    public Vector3 m_moveDistance = new Vector3(0, -50, 0);

    // アイテムショップUI管理クラス
    ItemShopUIHandler m_itemShopUIHandler;

    private Vector3 m_initialPosition;

    // 購入金額表示テキストのコピー
    private GameObject m_itemPurchaseDisplayPrefab;

    // 現在の購入金額表示テキストのコピー
    private GameObject m_currentitemPurchaseDisplayPrefab;

    void Start()
    {
        // アイテムショップUI管理管理クラスが設定されていない場合の処理
        if(m_itemShopUIHandler == null)
        {
            // アイテムショップUI管理クラスを設定する
            m_itemShopUIHandler = FindAnyObjectByType<ItemShopUIHandler>();
        }

        // 購入金額表示テキストののコピーを作る
        m_itemPurchaseDisplayPrefab = m_itemShopUIHandler.m_itemPurchaseDisplay.gameObject;
        // 初期の色と位置を保存
        m_initialPosition = m_itemShopUIHandler.m_itemPurchaseDisplay.rectTransform.anchoredPosition;

        // アイテム購入額の表示を非表示にする
        m_itemShopUIHandler.m_itemPurchaseDisplay.gameObject.SetActive(false);
    }

    // テキストのフェード
    public IEnumerator FadeMoveAndResetText(string text, float price, string itemUnit)
    {
        // 新しいテキストオブジェクトを生成
        GameObject newPurchaseDisplay = Instantiate(m_itemPurchaseDisplayPrefab, m_itemShopUIHandler.m_itemPurchaseDisplay.transform.parent);
        var textComponent = newPurchaseDisplay.GetComponent<TextMeshProUGUI>();
        textComponent.text = text + price + itemUnit;
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f);
        newPurchaseDisplay.SetActive(true);

        // フェードアウトしながら移動
        yield return StartCoroutine(FadeAndMoveText(1, 0, newPurchaseDisplay.transform.position, newPurchaseDisplay.transform.position + m_moveDistance, newPurchaseDisplay));

        // 少し待つ
        yield return new WaitForSeconds(0.5f);

        // テキストを削除
        Destroy(newPurchaseDisplay); // 生成したテキストを削除
    }

    private IEnumerator FadeAndMoveText(float startAlpha, float endAlpha, Vector3 startPosition, Vector3 endPosition, GameObject textObject)
    {
        float elapsedTime = 0f;

        while (elapsedTime < m_duration)
        {
            float t = elapsedTime / m_duration;

            // テキストの位置を補間して移動
            if (textObject != null)
            {
                textObject.transform.position = Vector3.Lerp(startPosition, endPosition, t);

                // テキストの透明度を変更
                Color newColor = textObject.GetComponent<TextMeshProUGUI>().color;
                newColor.a = Mathf.Lerp(startAlpha, endAlpha, t);
                textObject.GetComponent<TextMeshProUGUI>().color = newColor;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最終的な位置と透明度を設定
        if (textObject != null)
        {
            textObject.transform.position = endPosition;
            Color finalColor = textObject.GetComponent<TextMeshProUGUI>().color;
            finalColor.a = endAlpha;
            textObject.GetComponent<TextMeshProUGUI>().color = finalColor;
        }
    }
    // 透明度と位置をリセットする
    private void ResetTextPositionAndAlpha()
    {
        // 位置を元に戻す
        m_itemShopUIHandler.m_itemPurchaseDisplay.rectTransform.anchoredPosition = m_initialPosition;

        // 透明度を元に戻す
        Color resetColor = m_itemShopUIHandler.m_itemPurchaseDisplay.color;
        resetColor.a = 1f;
        m_itemShopUIHandler.m_itemPurchaseDisplay.color = resetColor;

        // 購入時の金額を非表示にする
        m_itemShopUIHandler.m_itemPurchaseDisplay.gameObject.SetActive(false);
    }
}