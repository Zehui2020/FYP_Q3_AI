using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField] private PlayerPrefs playerPrefs;
    [SerializeField] private bool queueOnStart = false;
    private bool finishedGenerating = false;

    public UnityEvent OnFinishAllAbilities;

    private void Start()
    {
        InitManager();
        if (queueOnStart && !playerPrefs.offImageGen)
            QueueItems();
    }

    public void QueueItems()
    {
        if (currentPromptIndex >= itemPrompts.Count && !finishedGenerating)
        {
            OnFinishAllAbilities?.Invoke();
            finishedGenerating = true;
            Debug.Log("Start Generating Items");
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
        Debug.Log("GOT ABILIOTY!");

        QueueItems();

        return true;
    }
}