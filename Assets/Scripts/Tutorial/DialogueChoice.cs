using DesignPatterns.ObjectPool;
using TMPro;
using UnityEngine;

public class DialogueChoice : PooledObject
{
    private DialogueManager.Dialogue.DialogueChoice currentChoice;
    public event System.Action<DialogueManager.Dialogue.DialogueChoice> OnSelectEvent;

    [SerializeField] private TextMeshProUGUI choiceText;

    public void InitChoice(DialogueManager.Dialogue.DialogueChoice dialogueChoice)
    {
        currentChoice = dialogueChoice;
        choiceText.text = dialogueChoice.choice;
    }

    public void SelectChoice()
    {
        OnSelectEvent?.Invoke(currentChoice);
    }

    public void ReturnToPool()
    {
        OnSelectEvent = null;
        Release();
        gameObject.SetActive(false);
    }
}