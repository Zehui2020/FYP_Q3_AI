using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComfyBGManager : ComfyManager
{
    [SerializeField] private WorldSpaceButtonController buttonController;
    [SerializeField] protected ComfyTilesetGeneration tilesetGeneration;
    [SerializeField] private ComfyUIManager uiManager;

    [SerializeField] private PromptData promptData;
    private bool startGenerating;

    private string totalStringPrompt;

    private PromptData.BGPrompt.Type currentBGType;

    private void Start()
    {
        InitManager();

        buttonController.InitController(promptData);
        buttonController.SpawnButtons(currentBGType);

        tilesetGeneration.InitTilesetGeneration(promptData);

        uiManager.SetStartingPrompt(currentBGType);
    }

    private void Update()
    {
        if (promptID == comfyWebsocket.promptID)
            uiManager.SetLoadingBar(comfyWebsocket.currentProgress, comfyWebsocket.maxProgress);
    }

    public void StartBGGeneration()
    {
        if (startGenerating)
            return;

        if (uiManager.GetPrompt() == uiManager.setPrompts + ", " + currentBGType.ToString())
            return;

        if (currentBGType > PromptData.BGPrompt.Type.TotalTypes)
            return;

        startGenerating = true;
        QueueBGPrompt();
    }

    public void QueueBGPrompt()
    {
        totalStringPrompt += uiManager.GetPrompt();

        PromptData.BGPrompt bgPrompt = promptData.GetBGPrompt(currentBGType, uiManager.GetPrompt());
        promptCtr.QueuePromptWithControlNet(promptData.GetPromptJSON(bgPrompt.bgType), bgPrompt.prompt, bgPrompt.referenceImage);
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        fileName = currentBGType.ToString();

        if (base.OnRecieveImage(promptID, texture))
        {
            startGenerating = false;
            uiManager.ResetPrompt();

            if (currentBGType == PromptData.BGPrompt.Type.TotalTypes)
            {
                tilesetGeneration.QueueTilesetPrompt(totalStringPrompt);
                return false;
            }

            currentBGType++;
            uiManager.SetStartingPrompt(currentBGType);
            buttonController.SpawnButtons(currentBGType);

            return true;
        }

        return false;
    }
}