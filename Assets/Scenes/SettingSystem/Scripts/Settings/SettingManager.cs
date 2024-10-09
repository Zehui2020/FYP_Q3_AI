//----------------------------------------------------------------------
// SettingManager
//
// Class that manages the settings screen
//
// Date: 27/9/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using UnityEngine;

public class SettingManager : MonoBehaviour
{
    [Header("Class that manages the UI of the settings screen")]
    public SettingUIManager m_settingUIManager;

    [Header("Class that handles the overall UI of the settings screen")]
    public SettingUIHandler m_settingUIHandler;

    [Header("Class that applies effects to the UI of the settings screen")]
    public OptionsMenuEffect m_optionsMenuEffect;

    [Header("Class that manages the master volume")]
    public MasterVolumeController m_masterVolumeController;

    // Class that manages game termination
    private GameEndSystem m_gameEndSystem;

    private void Start()
    {
        // Handle the case where the class that manages the UI of the settings screen is not assigned
        if (m_settingUIManager == null)
        {
            // Assign the class that manages the UI of the settings screen
            m_settingUIManager = FindAnyObjectByType<SettingUIManager>();
        }

        // Handle the case where the class that handles the overall UI of the settings screen is not assigned
        if (m_settingUIHandler == null)
        {
            m_settingUIHandler = FindAnyObjectByType<SettingUIHandler>();
        }

        // Handle the case where the class that applies effects to the UI of the settings screen is not assigned
        if (m_optionsMenuEffect == null)
        {
            // Assign the class that applies effects to the UI of the settings screen
            m_optionsMenuEffect = FindAnyObjectByType<OptionsMenuEffect>();
        }

        // Handle the case where the class that manages the master volume is not assigned
        if (m_masterVolumeController == null)
        {
            // Assign the class that manages the master volume
            m_masterVolumeController = FindAnyObjectByType<MasterVolumeController>();
        }

        // Handle the case where the class that manages game termination is not assigned
        if (m_gameEndSystem == null)
        {
            // Assign the class that manages game termination
            m_gameEndSystem = GetComponent<GameEndSystem>();
        }

        // Play the BGM
        m_masterVolumeController.m_BGMVolumeController.PlaySampleBGM();

        // Set the initial state of the UI of the settings screen
        m_settingUIManager.SettingInitialState();

        // Handle when the button to close the settings screen is clicked
        m_settingUIHandler.m_settingEndTextButton.onClick.AddListener(() =>
        {
            // Restore the size of the UI on the settings screen
            m_optionsMenuEffect.RestoreOriginalSize();

            // Hide the settings screen
            m_settingUIManager.CloseSettings();

            // Play the return sound effect
            m_masterVolumeController.m_SFXVolumeController.PlaySampleReturnSFX();
        });

        // Handle when the options button is pressed
        m_settingUIHandler.m_optionsButton.onClick.AddListener(() =>
        {
            // Handle the case where the settings screen is already visible
            if (m_settingUIManager.GetIsSettings() == true)
            {
                // Hide the settings screen
                m_settingUIManager.CloseSettingsUI();
            }
            // Show the options menu
            m_settingUIManager.OpenOptionsMenuUI();

            // Play the decision sound effect
            m_masterVolumeController.m_SFXVolumeController.PlaySampleDecisionSFX();
        });

        // Handle when the Save & Exit button is pressed
        m_settingUIHandler.m_saveAndExitButton.onClick.AddListener(() =>
        {
            // End the game
            m_gameEndSystem.GameEnd();
        });

    }

    private void Update()
    {
        // Handle when the ESC key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // Handle closing the settings screen
            if (m_settingUIManager.GetIsSettings() == false)
            {
                m_settingUIManager.OpenSettings();
                m_masterVolumeController.m_SFXVolumeController.PlaySampleDecisionSFX();
            }
            else
            {
                // Restore the size of the UI on the settings screen
                m_optionsMenuEffect.RestoreOriginalSize();

                // If a coroutine is active, stop it and restore the original size
                StopAllCoroutines();

                // Hide all settings screens
                m_settingUIManager.CloseAllSettings();
                //// Hide the settings screen
                //m_settingUIManager.CloseSettings();
                m_masterVolumeController.m_SFXVolumeController.PlaySampleReturnSFX();
            }
        }
    }
}
