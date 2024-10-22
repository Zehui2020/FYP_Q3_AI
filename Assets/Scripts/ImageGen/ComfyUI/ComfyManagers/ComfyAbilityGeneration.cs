using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComfyAbilityGeneration : ComfyManager
{
    [System.Serializable]
    public struct ItemPrompt
    {
        [TextArea(3, 10)] public string Pprompt;
        public string controlNetImage;
        public BaseAbility.AbilityName filename;
    }

    public List<ItemPrompt> itemPrompts = new();
    private int currentPromptIndex = 0;

    [SerializeField] private bool queueOnStart = false;

    private void Start()
    {
        InitManager();
        if (queueOnStart)
            QueueItems();
    }

    public void QueueItems()
    {
        if (currentPromptIndex >= itemPrompts.Count)
            return;

        promptCtr.QueuePromptWithControlNet(itemPrompts[currentPromptIndex].Pprompt, itemPrompts[currentPromptIndex].controlNetImage);
        fileName = itemPrompts[currentPromptIndex].filename.ToString();
    }

    public override bool OnRecieveImage(string promptID, Texture2D texture)
    {
        if (!base.OnRecieveImage(promptID, texture))
            return false;

        currentPromptIndex++;
        QueueItems();
        return true;
    }
}