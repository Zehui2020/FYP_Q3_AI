//----------------------------------------------------------------------
// OptionsMenuInteraction
//
// Class that handles interactions between the options menu and the user
//
// Date: 3/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsMenuInteraction : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Effect class
    private OptionsMenuEffect m_optionsMenuEffect;
    // Class that manages the UI of the settings screen
    public SettingUIHandler m_settingUIHandler;

    void Start()
    {
        // Handle the case where the effect class is not assigned
        if (m_optionsMenuEffect == null)
        {
            // Get the effect class
            m_optionsMenuEffect = this.GetComponent<OptionsMenuEffect>();
        }

        // Handle the case where the class that manages the UI of the settings screen is not assigned
        if (m_settingUIHandler == null)
        {
            // Assign the class that manages the UI of the settings screen
            m_settingUIHandler = FindAnyObjectByType<SettingUIHandler>();
        }
    }

    // Handle when the pointer enters the button
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Get the button's parent object
        Button button = eventData.pointerEnter.GetComponentInParent<Button>();

        // Options button
        if (button == m_settingUIHandler.m_optionsButton)
        {
            m_optionsMenuEffect.StartOptionsButtonEffect();
        }
        // Save button
        else if (button == m_settingUIHandler.m_saveButton)
        {
            m_optionsMenuEffect.StartSaveButtonEffect();
        }
        // Save & Exit button
        else if (button == m_settingUIHandler.m_saveAndExitButton)
        {
            m_optionsMenuEffect.StartSaveAndExitButtonEffect();
        }
        // Button to close the settings screen
        else if (button == m_settingUIHandler.m_settingEndTextButton)
        {
            m_optionsMenuEffect.StartSettingEndTextButtonEffect();
        }
    }

    // Handle when the pointer exits the button
    public void OnPointerExit(PointerEventData eventData)
    {
        // Get the button's parent object
        Button button = eventData.pointerEnter.GetComponentInParent<Button>();

        // Options button
        if (button == m_settingUIHandler.m_optionsButton)
        {
            m_optionsMenuEffect.EndOptionsButtonEffect();
        }
        // Save button
        else if (button == m_settingUIHandler.m_saveButton)
        {
            m_optionsMenuEffect.EndSaveButtonEffect();
        }
        // Save & Exit button
        else if (button == m_settingUIHandler.m_saveAndExitButton)
        {
            m_optionsMenuEffect.EndSaveAndExitButtonEffect();
        }
        // Button to close the settings screen
        else if (button == m_settingUIHandler.m_settingEndTextButton)
        {
            m_optionsMenuEffect.EndSettingEndTextButtonEffect();
        }
    }
}
