using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.Events;
using static PromptData;

public class ComfyBGManager : ComfyManager
{
    [SerializeField] private WorldSpaceButtonController buttonController;
    [SerializeField] protected ComfyTilesetGeneration tilesetGeneration;
    [SerializeField] private ComfyUIManager uiManager;
    [SerializeField] private PlayerPrefs playerPrefs;

    private bool startGenerating;

    private string totalStringPrompt;

    private int bgRecievedCounter;

    [SerializeField] private List<PromptData> allPromptDatas = new();
    private int currentLevelPrompt = 0;
    private int queueLevelData = 0;
    [SerializeField] private List<string> bgPrompts = new();

    private void Start()
    {
        InitManager();

        buttonController.InitController(allPromptDatas[currentLevelPrompt]);

        if (playerPrefs.experiencedTutorial)
            buttonController.SpawnButtons();

        tilesetGeneration.InitTilesetGeneration(allPromptDatas[currentLevelPrompt]);

        uiManager.SetStartingPrompt(2);
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

        bgPrompts.Add(uiManager.GetPrompt());

        if (queueLevelData != allPromptDatas.Count - 1)
        {
            buttonController.InitController(allPromptDatas[queueLevelData]);
            buttonController.SpawnButtons();
            queueLevelData++;
            uiManager.SetStartingPrompt(queueLevelData + 2);
            return false;
        }

        totalStringPrompt += bgPrompts;
        startGenerating = true;
        QueueBGPrompt();

        return true;
    }

    public void QueueBGPrompt()
    {
        PromptData.BGPrompt bgPrompt = allPromptDatas[currentLevelPrompt].GetBGPrompt((PromptData.BGPrompt.Type)bgRecievedCounter, bgPrompts[currentLevelPrompt]);
        totalStringPrompt += bgPrompt.prompt;
        promptCtr.QueuePromptWithControlNet(allPromptDatas[currentLevelPrompt].GetPromptJSON(bgPrompt.bgType), bgPrompt.prompt, bgPrompt.referenceImage);
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        fileName = ((BGPrompt.Type)bgRecievedCounter).ToString() + "_Level" + currentLevelPrompt + 2;

        if (base.OnRecieveImage(promptID, texture))
        {
            if (bgRecievedCounter >= (int)PromptData.BGPrompt.Type.Background)
            {
                if (startGenerating)
                {
                    startGenerating = false;
                    currentLevelPrompt++;

                    // Go to generate the next prompt
                    if (currentLevelPrompt >= allPromptDatas.Count)
                    {
                        tilesetGeneration.QueueTilesetPrompt(totalStringPrompt);
                        return true;
                    }

                    startGenerating = true;
                    bgRecievedCounter = 0;
                    totalStringPrompt = string.Empty;
                    QueueBGPrompt();
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