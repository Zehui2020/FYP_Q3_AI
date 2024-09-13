using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ComfyUIManager : MonoBehaviour
{
    [Header("Loading")]
    [SerializeField] private Slider loadingBar;
    [SerializeField] private TextMeshProUGUI loadingText;

    [Header("Prompt")]
    [SerializeField] private TextMeshProUGUI promptText;

    [SerializeField] private string setPrompts;

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
    }

    public void ResetPrompt()
    {
        promptText.text = setPrompts;
    }
}