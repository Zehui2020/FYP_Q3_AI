//----------------------------------------------------------------------
// SceneExitHandler
//
// Class for managing scene exit
//
// Data: 23/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class SceneExitHandler : MonoBehaviour
{
    // Item shop UI management class
    ItemShopUIHandler m_itemShopUIHandler;

    void Start()
    {
        // Set the item shop UI management class
        m_itemShopUIHandler = FindObjectOfType<ItemShopUIHandler>();

        // Add an event listener to m_exitTextButton
        if (m_itemShopUIHandler != null && m_itemShopUIHandler.m_exitTextButton != null)
        {
            m_itemShopUIHandler.m_exitTextButton.onClick.AddListener(OnExitButtonClicked);
        }
    }

    // Process when the button is clicked
    public void OnExitButtonClicked()
    {
        // Exit the current scene
        Application.Quit();

        // For debugging purposes
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
