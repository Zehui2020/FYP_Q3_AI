//----------------------------------------------------------------------
// SettingUIManager
//
// Class that manages the UI of the settings screen
//
// Date: 1/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class SettingUIManager : MonoBehaviour
{
    [Header("Class that handles the UI of the settings screen")]
    public SettingUIHandler m_settingUIHandler;

    // Flag to indicate whether the settings screen is shown or hidden
    private bool m_isSettings = false;

    private void Start()
    {
        // Handle the case where the class that handles the UI of the settings screen is not assigned
        if (m_settingUIHandler == null)
        {
            // Assign the class that handles the UI of the settings screen
            m_settingUIHandler = FindAnyObjectByType<SettingUIHandler>();
        }
    }

    // Set the initial state of the settings screen
    public void SettingInitialState()
    {
        // Hide the background of the settings screen
        m_settingUIHandler.m_settingsBackgroundImage.gameObject.SetActive(false);
        // Hide the button to close the settings screen
        m_settingUIHandler.m_settingEndTextButton.gameObject.SetActive(false);
        // Hide the options button
        m_settingUIHandler.m_optionsButton.gameObject.SetActive(false);
        // Hide the save button
        m_settingUIHandler.m_saveButton.gameObject.SetActive(false);
        // Hide the save & exit button
        m_settingUIHandler.m_saveAndExitButton.gameObject.SetActive(false);
        // Hide the master volume image
        m_settingUIHandler.m_masterVolumeImage.gameObject.SetActive(false);
        // Hide the music volume image
        m_settingUIHandler.m_musicVolumeImage.gameObject.SetActive(false);
        // Hide the brightness image
        m_settingUIHandler.m_brightnessImage.gameObject.SetActive(false);
        // Hide the sound effect volume image
        m_settingUIHandler.m_soundEffectVolumeImage.gameObject.SetActive(false);
        // Hide the master volume slider
        m_settingUIHandler.m_masterVolumeSlider.gameObject.SetActive(false);
        // Hide the music volume slider
        m_settingUIHandler.m_musicVolumeSlider.gameObject.SetActive(false);
        // Hide the sound effect volume slider
        m_settingUIHandler.m_soundEffectVolumeSlider.gameObject.SetActive(false);
        // Hide the brightness slider
        m_settingUIHandler.m_brightnessSlider.gameObject.SetActive(false);
    }

    // Show the settings screen
    public void OpenSettings()
    {
        // Show the background of the settings screen
        OpenSettingsBackgroundImage();
        // Show the UI of the settings screen
        OpenSettingsUI();

        // Set the settings screen to be in the visible state
        m_isSettings = true;
    }

    // Hide the settings screen
    public void CloseSettings()
    {
        // If the settings screen is currently visible
        if (m_settingUIHandler.m_optionsButton.gameObject.activeSelf &&   // Options button
            m_settingUIHandler.m_saveButton.gameObject.activeSelf &&      // Save button
            m_settingUIHandler.m_saveAndExitButton.gameObject.activeSelf  // Save & Exit button
            )
        {
            // Hide the UI of the settings screen
            CloseSettingsUI();
            // Hide the background of the settings screen
            CloseSettingsBackgroundImage();

            // Set the settings screen to be in the hidden state
            m_isSettings = false;
        }
        // If the options menu is currently visible
        else if (m_settingUIHandler.m_masterVolumeImage.gameObject.activeSelf &&       // Master volume image
                 m_settingUIHandler.m_musicVolumeImage.gameObject.activeSelf &&        // Music volume image
                 m_settingUIHandler.m_soundEffectVolumeImage.gameObject.activeSelf &&  // Sound effect volume image
                 m_settingUIHandler.m_brightnessImage.gameObject.activeSelf            // Brightness image
                 )
        {
            // Hide the UI of the options menu
            CloseOptionsMenuUI();
            // Show the settings screen
            OpenSettingsUI();

            // Set the settings screen to be in the visible state
            m_isSettings = true;
        }
    }

    // Close all settings screens
    public void CloseAllSettings()
    {
        // Hide the UI of the settings screen
        CloseSettingsUI();
        // Hide the UI of the options menu
        CloseOptionsMenuUI();
        // Hide the background of the settings screen
        CloseSettingsBackgroundImage();

        // Set the settings screen to be in the hidden state
        m_isSettings = false;
    }

    // Show the UI of the settings screen
    private void OpenSettingsUI()
    {
        // Options button
        m_settingUIHandler.m_optionsButton.gameObject.SetActive(true);
        // Save button
        m_settingUIHandler.m_saveButton.gameObject.SetActive(true);
        // Save & Exit button
        m_settingUIHandler.m_saveAndExitButton.gameObject.SetActive(true);
    }

    // Hide the UI of the settings screen
    public void CloseSettingsUI()
    {
        // Options button
        m_settingUIHandler.m_optionsButton.gameObject.SetActive(false);
        // Save button
        m_settingUIHandler.m_saveButton.gameObject.SetActive(false);
        // Save & Exit button
        m_settingUIHandler.m_saveAndExitButton.gameObject.SetActive(false);
    }

    // Show the UI of the options menu
    public void OpenOptionsMenuUI()
    {
        // Master volume image
        m_settingUIHandler.m_masterVolumeImage.gameObject.SetActive(true);
        // Music volume image
        m_settingUIHandler.m_musicVolumeImage.gameObject.SetActive(true);
        // Sound effect volume image
        m_settingUIHandler.m_soundEffectVolumeImage.gameObject.SetActive(true);
        // Brightness image
        m_settingUIHandler.m_brightnessImage.gameObject.SetActive(true);

        // Master volume slider
        m_settingUIHandler.m_masterVolumeSlider.gameObject.SetActive(true);
        // Music volume slider
        m_settingUIHandler.m_musicVolumeSlider.gameObject.SetActive(true);
        // Sound effect volume slider
        m_settingUIHandler.m_soundEffectVolumeSlider.gameObject.SetActive(true);
        // Brightness slider
        m_settingUIHandler.m_brightnessSlider.gameObject.SetActive(true);
    }

    // Hide the UI of the options menu
    private void CloseOptionsMenuUI()
    {
        // Master volume image
        m_settingUIHandler.m_masterVolumeImage.gameObject.SetActive(false);
        // Music volume image
        m_settingUIHandler.m_musicVolumeImage.gameObject.SetActive(false);
        // Sound effect volume image
        m_settingUIHandler.m_soundEffectVolumeImage.gameObject.SetActive(false);
        // Brightness image
        m_settingUIHandler.m_brightnessImage.gameObject.SetActive(false);

        // Master volume slider
        m_settingUIHandler.m_masterVolumeSlider.gameObject.SetActive(false);
        // Music volume slider
        m_settingUIHandler.m_musicVolumeSlider.gameObject.SetActive(false);
        // Sound effect volume slider
        m_settingUIHandler.m_soundEffectVolumeSlider.gameObject.SetActive(false);
        // Brightness slider
        m_settingUIHandler.m_brightnessSlider.gameObject.SetActive(false);
    }

    // Show the background of the settings screen
    private void OpenSettingsBackgroundImage()
    {
        // Button to close the settings screen
        m_settingUIHandler.m_settingEndTextButton.gameObject.SetActive(true);
        // Background of the settings screen
        m_settingUIHandler.m_settingsBackgroundImage.gameObject.SetActive(true);
    }

    // Hide the background of the settings screen
    private void CloseSettingsBackgroundImage()
    {
        // Button to close the settings screen
        m_settingUIHandler.m_settingEndTextButton.gameObject.SetActive(false);
        // Background of the settings screen
        m_settingUIHandler.m_settingsBackgroundImage.gameObject.SetActive(false);
    }

    // Return whether the settings screen is shown or hidden
    public bool GetIsSettings()
    {
        return m_isSettings;
    }
}
