using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class ComfyUIManager : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Prompt")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI promptText;

    public string setPrompts;

    public UnityEvent OnRecievePrompt;

    public void SetLoadingBar(int currentValue, int maxValue)
    {
        if (currentValue < maxValue)
        {
            loadingBar.maxValue = maxValue;
            loadingBar.value = currentValue;
            loadingText.text = "Generating Image (" + currentValue + " / " + maxValue + ")";
        }
        else
        {
            loadingBar.maxValue = maxValue;
            loadingBar.value = maxValue;
            loadingText.text = "Processing Image...";
        }
    }

    public string GetPrompt()
    {
        return promptText.text;
    }

    public void AddPrompt(string text)
    {
        promptText.text += ", " + text;
        OnRecievePrompt?.Invoke();
    }

    public void ResetPrompt()
    {
        promptText.text = setPrompts;
    }

    public void SetStartingPrompt(PromptData.BGPrompt.Type promptData)
    {
        title.text = promptData.ToString();
        promptText.text = setPrompts + ", " + promptData.ToString();
    }
}