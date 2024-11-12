using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComfyTilesetGeneration : ComfyManager
{
    [SerializeField] private string setPrompts;
    private List<PromptData> promptData;
    [SerializeField] private int tilesetRecieved = 0;

    private List<string> playerBGPrompts;

    public UnityEvent OnFnishTileset;

    public void InitTilesetGeneration(List<PromptData> promptData, List<string> playerBGPrompts)
    {
        this.promptData = promptData;
        this.playerBGPrompts = playerBGPrompts;
        InitManager();
    }

    public void QueueTilesetPrompt()
    {
        if (tilesetRecieved >= promptData.Count)
        {
            OnFnishTileset?.Invoke();
            Destroy(gameObject);
            return;
        }

        string finalPrompt = string.Empty;
        string controlnetImage = string.Empty;

        foreach (PromptData.PromptChecker promptChecker in promptData[tilesetRecieved].promptCheckers)
        {
            if (playerBGPrompts[tilesetRecieved].Contains(promptChecker.foundPrompts))
            {
                finalPrompt = setPrompts + ", " + promptChecker.endPrompt;
                controlnetImage = promptChecker.controlnetImage;
            }
        }

        if (finalPrompt == string.Empty)
            finalPrompt = setPrompts + ", dirt themed";

        GameData.Instance.EnqueueLoading(playerBGPrompts[tilesetRecieved] + "_Tileset", false);

        Debug.Log("Tileset Theme: " + finalPrompt);

        promptCtr.QueuePromptWithControlNet(setPrompts + finalPrompt, controlnetImage);
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        if (tilesetRecieved >= playerBGPrompts.Count)
        {
            Destroy(gameObject);
            return false;
        }

        fileName = "Tileset_" + playerBGPrompts[tilesetRecieved];
        Debug.Log("RECIEVED " + fileName);

        if (base.OnRecieveImage(promptID, texture))
        {
            tilesetRecieved++;
            QueueTilesetPrompt();
            return true;
        }

        return false;
    }
}