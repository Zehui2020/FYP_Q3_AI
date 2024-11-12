using System.Collections.Generic;
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

    [SerializeField] private int bgRecievedCounter;
    [SerializeField] private int currentLevelPrompt = 0;
    [SerializeField] private int queueLevelData = 0;

    [SerializeField] private List<PromptData> allPromptDatas = new();
    [SerializeField] private List<string> bgPrompts = new();

    public UnityEvent OnStartBGGen;
    public UnityEvent OnFinishBGGen;

    private void Start()
    {
        queueLevelData = 0;

        InitManager();

        uiManager.SetStartingPrompt(queueLevelData + 1);
        buttonController.InitController(allPromptDatas[queueLevelData]);
        if (playerPrefs.experiencedTutorial)
        {
            buttonController.SpawnButtons();
        }
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

        if (queueLevelData != allPromptDatas.Count - 1)
        {
            buttonController.InitController(allPromptDatas[queueLevelData]);
            buttonController.SpawnButtons();
            queueLevelData++;
            uiManager.SetStartingPrompt(queueLevelData + 1);
            return false;
        }

        OnStartBGGen?.Invoke();

        return true;
    }

    public void QueueBGPrompt()
    {
        startGenerating = true;
        GameData.Instance.DequeueLoading();
        PromptData.BGPrompt bgPrompt = allPromptDatas[currentLevelPrompt].GetBGPrompt((PromptData.BGPrompt.Type)bgRecievedCounter, bgPrompts[currentLevelPrompt]);
        GameData.Instance.EnqueueLoading(bgPrompt.keywords + "_" + bgPrompt.type.ToString(), true);
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
                        OnFinishBGGen?.Invoke();
                        tilesetGeneration.InitTilesetGeneration(allPromptDatas, bgPrompts);
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