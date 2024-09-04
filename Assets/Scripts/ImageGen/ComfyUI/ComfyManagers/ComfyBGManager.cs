using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComfyBGManager : ComfyManager
{
    [SerializeField] private ComfyUIManager uiManager;
    [SerializeField] private MenuBackground menuBackground;
    public UnityEvent OnRecieveBackground;

    private bool recievedBG;

    private void Start()
    {
        InitManager();
    }

    private void Update()
    {
        if (promptID == comfyWebsocket.promptID)
            uiManager.SetLoadingBar(comfyWebsocket.currentProgress, comfyWebsocket.maxProgress);
    }

    public void QueueBGPrompt()
    {
        promptCtr.QueuePrompt(uiManager.GetPrompt());
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        if (base.OnRecieveImage(promptID, texture) && !recievedBG)
        {
            menuBackground.SetupBackground();
            OnRecieveBackground?.Invoke();
            recievedBG = true;
            Debug.Log("GOT BG!");
            return true;
        }

        return false;
    }

    private void OnDisable()
    {
        OnRecieveBackground = null;
    }
}