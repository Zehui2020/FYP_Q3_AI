using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ComfyBGManager : ComfyManager
{
    [SerializeField] private ComfyUIManager uiManager;
    [SerializeField] private MenuBackground menuBackground;
    public UnityEvent OnRecieveBackground;

    private void Start()
    {
        InitManager();
    }

    private void Update()
    {
        uiManager.SetLoadingBar(comfyWebsocket.currentProgress, comfyWebsocket.maxProgress);
    }

    public void QueueBGPrompt()
    {
        promptCtr.QueuePrompt(uiManager.GetPrompt());
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        if (base.OnRecieveImage(promptID, texture))
        {
            menuBackground.SetupBackground();
            OnRecieveBackground?.Invoke();
            Debug.Log("CALLED");
            return true;
        }

        return false;
    }
}