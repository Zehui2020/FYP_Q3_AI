//----------------------------------------------------------------------
// OptionsMenuEffect
//
// Class that applies effects to the options menu
//
// Date: 3/10/2024
// Author: Shimba Sakai
//----------------------------------------------------------------------

using System.Collections;
using UnityEngine;

public class OptionsMenuEffect : MonoBehaviour
{
    [Header("Class that manages the UI of the settings screen")]
    public SettingUIHandler m_settingUIHandler;
    [Header("Scale amount")]
    public float m_scaleAmount = 1.1f;
    [Header("Time it takes to change")]
    public float m_changeTime = 0.2f;

    private Vector3 m_optionsButtonOriginalScale;
    private Vector3 m_saveButtonOriginalScale;
    private Vector3 m_saveAndExitButtonOriginalScale;
    private Vector3 m_settingEndTextButtonOriginalScale;

    void Start()
    {
        // Handle the case where the class that manages the UI of the settings screen is not assigned
        if (m_settingUIHandler == null)
        {
            // Assign the class that manages the UI of the settings screen
            m_settingUIHandler = FindAnyObjectByType<SettingUIHandler>();
        }

        // Save the original scales
        m_optionsButtonOriginalScale = m_settingUIHandler.m_optionsButton.transform.localScale;                    // Options button
        m_saveButtonOriginalScale = m_settingUIHandler.m_saveButton.transform.localScale;                          // Save button
        m_saveAndExitButtonOriginalScale = m_settingUIHandler.m_saveAndExitButton.transform.localScale;            // Save & Exit button
        m_settingEndTextButtonOriginalScale = m_settingUIHandler.m_settingEndTextButton.transform.localScale;      // Button to close the settings screen
    }

    // Start the effect
    public void StartButtonEffect(Transform buttonTransform, Vector3 originalScale)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleEffect(buttonTransform, originalScale * m_scaleAmount));
    }

    // End the effect
    public void EndButtonEffect(Transform buttonTransform, Vector3 originalScale)
    {
        StopAllCoroutines();
        StartCoroutine(ScaleEffect(buttonTransform, originalScale));
    }

    // Change the scale
    private IEnumerator ScaleEffect(Transform targetTransform, Vector3 toScale)
    {
        Vector3 fromScale = targetTransform.localScale;
        float elapsedTime = 0f;
        while (elapsedTime < m_changeTime)
        {
            targetTransform.localScale = Vector3.Lerp(fromScale, toScale, elapsedTime / m_changeTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        targetTransform.localScale = toScale;
    }

    // Restore the original scale
    public void RestoreOriginalSize()
    {
        // Stop coroutines and restore all buttons to their original scale
        StopAllCoroutines();
        m_settingUIHandler.m_optionsButton.transform.localScale = m_optionsButtonOriginalScale;                   // Options button
        m_settingUIHandler.m_saveButton.transform.localScale = m_saveButtonOriginalScale;                         // Save button
        m_settingUIHandler.m_saveAndExitButton.transform.localScale = m_saveAndExitButtonOriginalScale;           // Save & Exit button
        m_settingUIHandler.m_settingEndTextButton.transform.localScale = m_settingEndTextButtonOriginalScale;     // Button to close the settings screen
    }

    // Effect for the options button
    public void StartOptionsButtonEffect() => StartButtonEffect(m_settingUIHandler.m_optionsButton.transform, m_optionsButtonOriginalScale);
    public void EndOptionsButtonEffect() => EndButtonEffect(m_settingUIHandler.m_optionsButton.transform, m_optionsButtonOriginalScale);

    // Effect for the save button
    public void StartSaveButtonEffect() => StartButtonEffect(m_settingUIHandler.m_saveButton.transform, m_saveButtonOriginalScale);
    public void EndSaveButtonEffect() => EndButtonEffect(m_settingUIHandler.m_saveButton.transform, m_saveButtonOriginalScale);

    // Effect for the save & exit button
    public void StartSaveAndExitButtonEffect() => StartButtonEffect(m_settingUIHandler.m_saveAndExitButton.transform, m_saveAndExitButtonOriginalScale);
    public void EndSaveAndExitButtonEffect() => EndButtonEffect(m_settingUIHandler.m_saveAndExitButton.transform, m_saveAndExitButtonOriginalScale);

    // Effect for the button to close the settings screen
    public void StartSettingEndTextButtonEffect() => StartButtonEffect(m_settingUIHandler.m_settingEndTextButton.transform, m_settingEndTextButtonOriginalScale);
    public void EndSettingEndTextButtonEffect() => EndButtonEffect(m_settingUIHandler.m_settingEndTextButton.transform, m_settingEndTextButtonOriginalScale);
}
