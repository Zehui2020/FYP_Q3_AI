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

    public void SetLoadingText(string text)
    {
        loadingText.text = text;
    }

    public void SetLoadingBar(int currentValue, int maxValue, string currentGeneration)
    {
        if (currentValue < maxValue)
        {
            loadingBar.maxValue = maxValue;
            loadingBar.value = currentValue;
            loadingText.text = "Generating " + currentGeneration + " (" + currentValue + " / " + maxValue + ")";
        }
        else
        {
            loadingBar.maxValue = maxValue;
            loadingBar.value = maxValue;
            loadingText.text = "Processing " + currentGeneration + "...";
        }
    }

    public bool CheckAdditionalPrompts()
    {
        return promptText.text == setPrompts;
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

    public void SetStartingPrompt(int levelNumber)
    {
        title.text = "Choose Your Adventure!\n" + "Stage " + levelNumber + " Theme: ";
        promptText.text = setPrompts;
    }
}