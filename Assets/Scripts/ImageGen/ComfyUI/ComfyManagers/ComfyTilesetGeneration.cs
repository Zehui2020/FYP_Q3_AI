using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComfyTilesetGeneration : ComfyManager
{
    private bool recievedTileset = false;

    [SerializeField] private string setPrompts;
    private PromptData promptData;

    public void InitTilesetGeneration(PromptData promptData)
    {
        this.promptData = promptData;
        InitManager();
    }

    public void QueueTilesetPrompt(string playerPrompt)
    {
        if (recievedTileset)
            return;

        string finalPrompt = string.Empty;
        string controlnetImage = string.Empty;

        foreach (PromptData.PromptChecker promptChecker in promptData.promptCheckers)
        {
            if (playerPrompt.Contains(promptChecker.foundPrompts))
            {
                finalPrompt = setPrompts + ", " + promptChecker.endPrompt;
                controlnetImage = promptChecker.controlnetImage;
            }
        }

        if (finalPrompt == string.Empty)
            finalPrompt = setPrompts + ", dirt themed";

        Debug.Log("Tileset Theme: " + finalPrompt);

        promptCtr.QueuePromptWithControlNet(setPrompts + finalPrompt, controlnetImage);
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