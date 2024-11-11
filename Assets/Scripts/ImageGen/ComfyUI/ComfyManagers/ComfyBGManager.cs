using System.Collections.Generic;
using UnityEngine;
using static PromptData;

public class ComfyBGManager : ComfyManager
{
    [SerializeField] private WorldSpaceButtonController buttonController;
    [SerializeField] protected ComfyTilesetGeneration tilesetGeneration;
    [SerializeField] private ComfyUIManager uiManager;
    [SerializeField] private PlayerPrefs playerPrefs;

    private bool startGenerating;

    [SerializeField] private int bgRecievedCounter;
    [SerializeField] private int currentLevelPrompt = 0;
    [SerializeField] private int queueLevelData = 0;

    [SerializeField] private List<PromptData> allPromptDatas = new();
    [SerializeField] private List<string> bgPrompts = new();

    private void Start()
    {
        InitManager();

        buttonController.InitController(allPromptDatas[queueLevelData]);
        if (playerPrefs.experiencedTutorial)
        {
            buttonController.SpawnButtons();
            queueLevelData++;
        }

        uiManager.SetStartingPrompt(2);
    }

    public bool StartBGGeneration()
    {
        if (startGenerating)
            return false;

        if (uiManager.GetPrompt() == uiManager.setPrompts)
            return false;

        foreach (string prompt in allPromptDatas[0].themePrompts)
        {
            if (Utility.StringExistsInString(prompt, uiManager.GetPrompt()))
            {
                bgPrompts.Add(prompt);
                GameData.Instance.levelThemes += ", " + prompt;
                break;
            }
        }

        if (queueLevelData != allPromptDatas.Count)
        {
            buttonController.InitController(allPromptDatas[queueLevelData]);
            buttonController.SpawnButtons();
            queueLevelData++;
            uiManager.SetStartingPrompt(queueLevelData + 1);
            return false;
        }

        startGenerating = true;
        QueueBGPrompt();

        return true;
    }

    public void QueueBGPrompt()
    {
        PromptData.BGPrompt bgPrompt = allPromptDatas[currentLevelPrompt].GetBGPrompt((PromptData.BGPrompt.Type)bgRecievedCounter, bgPrompts[currentLevelPrompt]);
        GameData.Instance.EnqueueLoading(bgPrompt.keywords + "_" + bgPrompt.type.ToString());
        promptCtr.QueuePromptWithControlNet(allPromptDatas[currentLevelPrompt].GetPromptJSON(bgPrompt.bgType), bgPrompt.prompt, bgPrompt.referenceImage);
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        if (currentLevelPrompt >= allPromptDatas.Count)
            return false;

        fileName = ((BGPrompt.Type)bgRecievedCounter).ToString() + "_" + bgPrompts[currentLevelPrompt];

        if (base.OnRecieveImage(promptID, texture))
        {
            if (bgRecievedCounter >= (int)PromptData.BGPrompt.Type.Middleground)
            {
                if (startGenerating)
                {
                    startGenerating = false;
                    currentLevelPrompt++;

                    // If all level BGs done
                    if (currentLevelPrompt >= allPromptDatas.Count)
                    {
                        tilesetGeneration.InitTilesetGeneration(allPromptDatas, bgPrompts);
                        tilesetGeneration.QueueTilesetPrompt();
                        Destroy(gameObject);
                        return true;
                    }

                    startGenerating = true;
                    bgRecievedCounter = 0;
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