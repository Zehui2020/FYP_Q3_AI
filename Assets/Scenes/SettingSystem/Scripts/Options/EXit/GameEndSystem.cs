//----------------------------------------------------------------------
// GameEndSystem
//
// ゲーム終了を管理するクラス
//
// Data: 2/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class GameEndSystem : MonoBehaviour
{
    // ゲームを終了する
    public void GameEnd()
    {
        // ゲームを終了する
        Application.Quit();

        // デバッグで確認する用
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
