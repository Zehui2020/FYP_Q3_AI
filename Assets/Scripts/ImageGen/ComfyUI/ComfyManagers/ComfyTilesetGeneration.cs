using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComfyTilesetGeneration : ComfyManager
{
    [SerializeField] private ComfyUIManager uiManager;
    public UnityEvent OnRecieveTileset;

    private bool recievedTileset;

    [System.Serializable]
    public struct PromptChecker
    {
        public string foundPrompts;
        public string endPrompt;
    }
    public List<PromptChecker> promptCheckers = new();
    [SerializeField] private string setPrompts;

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
                finalPrompt = setPrompts + ", " + promptChecker.endPrompt;
        }

        if (finalPrompt == string.Empty)
            finalPrompt = setPrompts + ", dirt themed";

        Debug.Log("Tileset Theme: " + finalPrompt);

        promptCtr.QueuePrompt(setPrompts + finalPrompt);
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        if (base.OnRecieveImage(promptID, texture) && !recievedTileset)
        {
            OnRecieveTileset?.Invoke();
            recievedTileset = true;
            return true;
        }

        return false;
    }

    private void OnDisable()
    {
        OnRecieveTileset = null;
    }
}