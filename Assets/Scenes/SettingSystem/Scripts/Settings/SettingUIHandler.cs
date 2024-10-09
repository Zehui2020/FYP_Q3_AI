//----------------------------------------------------------------------
// SettingUIHandler
//
// Class that handles the UI elements used in the settings screen
//
// Date: 30/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingUIHandler : MonoBehaviour
{
    // Image start here ---------------------------------------------------

    [Header("Background image for the settings screen")]
    public Image m_settingsBackgroundImage;

    [Header("Image for the entire screen")]
    public Image m_entireScreen;

    [Header("Image for the master volume control")]
    public Image m_masterVolumeImage;

    [Header("Image for the music volume control")]
    public Image m_musicVolumeImage;

    [Header("Image for the sound effect volume control slider")]
    public Image m_soundEffectVolumeImage;

    [Header("Image for the brightness control")]
    public Image m_brightnessImage;

    // Image end here -----------------------------------------------------




    // Button start here ---------------------------------------------------

    [Header("Button to close the settings screen")]
    public Button m_settingEndTextButton;

    [Header("Options button")]
    public Button m_optionsButton;

    [Header("Save button")]
    public Button m_saveButton;

    [Header("Save and Exit button")]
    public Button m_saveAndExitButton;

    // Button end here -----------------------------------------------------




    // Slider start here --------------------------------------------------

    [Header("Slider for adjusting the master volume")]
    public Slider m_masterVolumeSlider;

    [Header("Slider for adjusting the music volume")]
    public Slider m_musicVolumeSlider;

    [Header("Slider for adjusting the sound effect volume")]
    public Slider m_soundEffectVolumeSlider;

    [Header("Slider for adjusting the brightness")]
    public Slider m_brightnessSlider;

    // Slider end here ----------------------------------------------------
}
