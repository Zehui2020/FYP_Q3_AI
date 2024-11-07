using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class ComfyUIManager : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI promptText;

    public string setPrompts;

    public UnityEvent OnRecievePrompt;

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
        title.text = "Choose Your Themes!\n Theme " + (levelNumber - 1).ToString() + ":";
        promptText.text = setPrompts;
    }
}