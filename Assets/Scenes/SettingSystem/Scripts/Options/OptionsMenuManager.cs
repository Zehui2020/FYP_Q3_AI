//----------------------------------------------------------------------
// OptionsMenuManager
//
// Class that manages the menus opened in the options menu
//
// Date: 27/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class OptionsMenuManager : MonoBehaviour
{
    [Header("Class that manages the UI of the settings screen")]
    public SettingUIHandler m_settingUIHandler;

    [Header("Class that manages the master volume")]
    private MasterVolumeController m_masterVolumeController;

    [Header("Class that manages the screen brightness")]
    private BrightnessController m_brightnessController;

    private void Start()
    {
        // Handle the case where the class that manages the UI of the settings screen is not assigned
        if (m_settingUIHandler == null)
        {
            // Assign the class that manages the UI of the settings screen
            m_settingUIHandler = FindAnyObjectByType<SettingUIHandler>();
        }

        // Handle the case where the class that manages the master volume is not assigned
        if (m_masterVolumeController == null)
        {
            // Assign the class that manages the master volume
            m_masterVolumeController = GetComponent<MasterVolumeController>();
        }

        // Handle the case where the class that manages the screen brightness is not assigned
        if (m_brightnessController == null)
        {
            // Assign the class that manages the screen brightness
            m_brightnessController = GetComponent<BrightnessController>();
        }

        // Adjust the screen brightness
        m_settingUIHandler.m_brightnessSlider.onValueChanged.AddListener(m_brightnessController.OnBrightnessChange);
    }
}
