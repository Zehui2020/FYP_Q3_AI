//----------------------------------------------------------------------
// GameEndSystem
//
// A class to manage game termination
//
// Date: 2/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class GameEndSystem : MonoBehaviour
{
    // Terminates the game
    public void GameEnd()
    {
        // Terminates the game
        Application.Quit();

        // For debug confirmation
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
