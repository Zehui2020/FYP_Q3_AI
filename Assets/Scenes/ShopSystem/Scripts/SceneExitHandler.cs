//----------------------------------------------------------------------
// SceneExitHandler
//
// シーン終了を管理するクラス
//
// Data: 23/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class SceneExitHandler : MonoBehaviour
{
    // アイテムショップUI管理クラス
    ItemShopUIHandler m_itemShopUIHandler;

    void Start()
    {
        // アイテムショップUI管理クラスを設定する
        m_itemShopUIHandler = FindObjectOfType<ItemShopUIHandler>();

        // m_exitTextButtonにイベントリスナーを追加
        if (m_itemShopUIHandler != null && m_itemShopUIHandler.m_exitTextButton != null)
        {
            m_itemShopUIHandler.m_exitTextButton.onClick.AddListener(OnExitButtonClicked);
        }
    }

    // ボタンが押された時の処理
    public void OnExitButtonClicked()
    {
        // 現在のシーンを終了する
        Application.Quit();

        // デバッグで確認する用
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
