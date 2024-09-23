//----------------------------------------------------------------------
// ItemEffect
//
// アイテムに関連する効果を管理するクラス
//
// Data: 8/28/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.Collections;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{
    [Header("Item to apply the effect to / エフェクトを適用するアイテム")]
    public Transform m_itemTransform;
    [Header("Scale Amount / スケール量")]
    public float m_scaleAmount = 1.2f;
    [Header("Time it takes to change / 変化するのにかかる時間")]
    public float m_changeTime = 0.2f;
    [Header("Original size / 元の大きさ")]
    private Vector3 m_originalScale;

    void Start()
    {
        // 元の大きさを保存する
        m_originalScale = m_itemTransform.localScale;
    }

    // マウスカーソルがオブジェクトに入ったときの処理
    public void OnMouseEnter()
    {
        // 進行中の大きさ変更を停止する
        StopAllCoroutines();
        // アイテムを拡大する
        StartCoroutine(ScaleEffect(m_originalScale, m_originalScale * m_scaleAmount));
    }

    // マウスがアイテムから離れた場合の処理
    public void OnMouseExit()
    {
        // 進行中の大きさ変更を停止する
        StopAllCoroutines();
        // アイテムを元の大きさに戻す
        StartCoroutine(ScaleEffect(m_itemTransform.localScale, m_originalScale));
    }

    // アイテムのエフェクトを止める処理
    public void StopEffect()
    {
        // アイテムを元の大きさに戻す
        m_itemTransform.localScale = m_originalScale;

        // 進行中の大きさ変更を停止する
        StopAllCoroutines();
    }


    // アイテムの大きさを変更する処理
    private IEnumerator ScaleEffect(Vector3 fromScale, Vector3 toScale)
    {
        float elapsedTime = 0f;
        while (elapsedTime < m_changeTime)
        {
            // 徐々にスケールを変化させる
            m_itemTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / m_changeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        // アイテムの大きさ変更を指定した値にする
        m_itemTransform.localScale = toScale;
    }
}

