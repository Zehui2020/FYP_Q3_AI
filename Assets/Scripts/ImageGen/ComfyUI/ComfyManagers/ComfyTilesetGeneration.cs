using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComfyTilesetGeneration : ComfyManager
{
    [SerializeField] private ComfyUIManager uiManager;

    [System.Serializable]
    public struct PromptChecker
    {
        public string foundPrompts;
        public string endPrompt;
    }
    public List<PromptChecker> promptCheckers = new();

    private void Start()
    {
        InitManager();
    }

    public void QueueTilesetPrompt()
    {
        string finalPrompt = string.Empty;
        string playerPrompt = uiManager.GetPrompt();

        foreach (PromptChecker promptChecker in promptCheckers)
        {
            if (playerPrompt.Contains(promptChecker.foundPrompts))
                finalPrompt = promptChecker.endPrompt;
        }

        if (finalPrompt == string.Empty)
            finalPrompt = "dirt themed";

        Debug.Log(finalPrompt);

        promptCtr.QueuePrompt(finalPrompt);
    }
}