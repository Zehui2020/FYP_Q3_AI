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

    private PromptData.BGPrompt.Type currentBGType;
    private int bgRecievedCounter;

    [SerializeField] private List<string> bgPrompts = new();

    private void Start()
    {
        InitManager();

        buttonController.InitController(promptData);

        if (playerPrefs.experiencedTutorial)
            buttonController.SpawnButtons(currentBGType);

        tilesetGeneration.InitTilesetGeneration(promptData);

        uiManager.SetStartingPrompt(currentBGType);
    }

    private void Update()
    {
        if (promptID == comfyWebsocket.promptID)
            uiManager.SetLoadingBar(comfyWebsocket.currentProgress, comfyWebsocket.maxProgress);
    }

    public bool StartBGGeneration()
    {
        if (startGenerating)
            return false;

        if (uiManager.GetPrompt() == uiManager.setPrompts + ", " + currentBGType.ToString())
            return false;

        if (currentBGType > PromptData.BGPrompt.Type.TotalTypes)
            return false;

        bgPrompts.Add(uiManager.GetPrompt());

        if (bgPrompts.Count >= 3)
        {
            startGenerating = true;
            QueueBGPrompt();
        }
        else
        {
            currentBGType++;

            uiManager.ResetPrompt();
            uiManager.SetStartingPrompt(currentBGType);
            buttonController.SpawnButtons(currentBGType);
        }

        return true;
    }

    public void QueueBGPrompt()
    {
        totalStringPrompt += bgPrompts[bgRecievedCounter];

        PromptData.BGPrompt bgPrompt = promptData.GetBGPrompt((PromptData.BGPrompt.Type)bgRecievedCounter, bgPrompts[bgRecievedCounter]);
        promptCtr.QueuePromptWithControlNet(promptData.GetPromptJSON(bgPrompt.bgType), bgPrompt.prompt, bgPrompt.referenceImage);
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        fileName = ((PromptData.BGPrompt.Type)bgRecievedCounter).ToString();

        if (base.OnRecieveImage(promptID, texture))
        {
            if ((PromptData.BGPrompt.Type)bgRecievedCounter >= PromptData.BGPrompt.Type.Background)
            {
                Debug.Log("QUEUED TIELSET");
                tilesetGeneration.QueueTilesetPrompt(totalStringPrompt);
                return true;
            }

            bgRecievedCounter++;
            QueueBGPrompt();

            return true;
        }

        return false;
    }
}