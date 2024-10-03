using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComfyBGManager : ComfyManager
{
    [SerializeField] private WorldSpaceButtonController buttonController;
    [SerializeField] protected ComfyTilesetGeneration tilesetGeneration;
    [SerializeField] private ComfyUIManager uiManager;
    [SerializeField] private PlayerPrefs playerPrefs;

    [SerializeField] private PromptData promptData;
    private bool startGenerating;

    private string totalStringPrompt;

    private int bgRecievedCounter;

    [SerializeField] private string bgPrompts;

    private void Start()
    {
        InitManager();

        buttonController.InitController(promptData);

        if (playerPrefs.experiencedTutorial)
            buttonController.SpawnButtons();

        tilesetGeneration.InitTilesetGeneration(promptData);

        uiManager.SetStartingPrompt();
    }

    private void Update()
    {
        if (comfyWebsocket.currentProgress != -1 && comfyWebsocket.maxProgress != -1)
            uiManager.SetLoadingBar(comfyWebsocket.currentProgress, comfyWebsocket.maxProgress, "Background");
        else
            uiManager.SetLoadingText("Waiting for queue...");
    }

    public bool StartBGGeneration()
    {
        if (startGenerating)
            return false;

        if (uiManager.GetPrompt() == uiManager.setPrompts)
            return false;

        bgPrompts = uiManager.GetPrompt();
        totalStringPrompt += bgPrompts;

        startGenerating = true;
        QueueBGPrompt();

        return true;
    }

    public void QueueBGPrompt()
    {
        PromptData.BGPrompt bgPrompt = promptData.GetBGPrompt((PromptData.BGPrompt.Type)bgRecievedCounter, bgPrompts);
        totalStringPrompt += bgPrompt.prompt;
        promptCtr.QueuePromptWithControlNet(promptData.GetPromptJSON(bgPrompt.bgType), bgPrompt.prompt, bgPrompt.referenceImage);
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        fileName = ((PromptData.BGPrompt.Type)bgRecievedCounter).ToString();

        if (base.OnRecieveImage(promptID, texture))
        {
            if (bgRecievedCounter >= (int)PromptData.BGPrompt.Type.Background)
            {
                if (startGenerating)
                {
                    Debug.Log("QUEUED TIELSET");
                    tilesetGeneration.QueueTilesetPrompt(totalStringPrompt);
                    startGenerating = false;
                }

                return true;
            }

            bgRecievedCounter++;
            QueueBGPrompt();

            return true;
        }

        return false;
    }
}