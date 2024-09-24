using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComfyTilesetGeneration : ComfyManager
{
    private bool recievedTileset;

    [SerializeField] private string setPrompts;
    private PromptData promptData;

    public void InitTilesetGeneration(PromptData promptData)
    {
        this.promptData = promptData;
        InitManager();
    }

    public void QueueTilesetPrompt(string playerPrompt)
    {
        string finalPrompt = string.Empty;

        foreach (PromptData.PromptChecker promptChecker in promptData.promptCheckers)
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
            recievedTileset = true;
            return true;
        }

        return false;
    }
}