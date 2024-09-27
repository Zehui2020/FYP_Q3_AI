//----------------------------------------------------------------------
// SceneExitHandler
//
// Class to manage scene exit
//
// Date: 23/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class SceneExitHandler : MonoBehaviour
{
    // Item shop UI handler class
    ItemShopUIHandler m_itemShopUIHandler;

    void Start()
    {
        // Set the item shop UI handler class
        m_itemShopUIHandler = FindObjectOfType<ItemShopUIHandler>();

        // Add an event listener to the exit button
        if (m_itemShopUIHandler != null && m_itemShopUIHandler.m_exitTextButton != null)
        {
            m_itemShopUIHandler.m_exitTextButton.onClick.AddListener(OnExitButtonClicked);
        }
    }

    // Handle the button click event
    public void OnExitButtonClicked()
    {
        // Exit the current scene
        Application.Quit();

        // Debug check in the Unity editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
