using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneExitHandler : MonoBehaviour
{
    // アイテムショップUI
    ItemShopUIHandler m_itemShopUIHandler;

    // ボタンが押された時に呼ばれるメソッド
    public void OnExitButtonClicked()
    {
        // 現在のシーンを終了する
        Application.Quit();

        // エディタでテストする場合は、以下の行を追加します
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
