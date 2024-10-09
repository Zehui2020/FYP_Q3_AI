//----------------------------------------------------------------------
// BrightnessController
//
// Class for managing the brightness of the screen
//
// Date: 27/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class BrightnessController : MonoBehaviour
{
    [Header("Class that manages the UI of the settings screen")]
    public SettingUIHandler m_settingUIHandler;

    // Initial brightness value
    private float m_initialBrightnessValue = 1.0f;

    private void Start()
    {
        // Handle the case where the class that manages the UI of the settings screen is not assigned
        if (m_settingUIHandler == null)
        {
            // Assign the class that manages the UI of the settings screen
            m_settingUIHandler = FindAnyObjectByType<SettingUIHandler>();
        }

        // Set the initial brightness value
        m_settingUIHandler.m_brightnessSlider.value = m_initialBrightnessValue;
    }

    // Change the brightness
    public void OnBrightnessChange(float value)
    {
        // Get the color of the entire screen
        Color screenColor = m_settingUIHandler.m_entireScreen.color;

        // Adjust the brightness
        screenColor.a = m_initialBrightnessValue - value;

        // Apply the adjusted color to the entire screen
        m_settingUIHandler.m_entireScreen.color = screenColor;
    }
}
