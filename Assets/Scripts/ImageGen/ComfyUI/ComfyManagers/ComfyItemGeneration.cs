using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static PromptData;

public class ComfyItemGenration : ComfyManager
{
    [System.Serializable]
    public struct ItemPrompt
    {
        [TextArea(3, 10)] public string Pprompt;
        public string controlNetImage;
        public Item.ItemType filename;
    }

    public List<ItemPrompt> itemPrompts = new();
    private int currentPromptIndex = 0;

    [SerializeField] private bool queueOnStart = false;
    public UnityEvent OnFinishAllItems;

    private void Start()
    {
        InitManager();
        if (queueOnStart)
            QueueItems();
    }

    public void QueueItems()
    {
        if (currentPromptIndex >= itemPrompts.Count)
        {
            OnFinishAllItems?.Invoke();
            Destroy(gameObject);
            return;
        }

        promptCtr.QueuePromptWithControlNet(itemPrompts[currentPromptIndex].Pprompt, itemPrompts[currentPromptIndex].controlNetImage);
        fileName = itemPrompts[currentPromptIndex].filename.ToString();
        GameData.Instance.EnqueueLoading(fileName + "_Icon", false);
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