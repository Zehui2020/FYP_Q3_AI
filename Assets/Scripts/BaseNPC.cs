using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BaseNPC : MonoBehaviour, IInteractable
{
    private PlayerController player;

    [SerializeField] private SimpleAnimation keycodeUI;
    [SerializeField] private List<DialogueManager.Dialogue> dialogues;
    [SerializeField] private List<DialogueManager.PopupDialogue> popupDialogues;

    public UnityEvent InteractEvent;

    private int dialogueIndex;

    private void Start()
    {
        player = PlayerController.Instance;
    }

    public void OnEnterRange()
    {
        keycodeUI.Show();
    }

    public virtual bool OnInteract()
    {
        keycodeUI.Hide();
        InteractEvent?.Invoke();
        player.TalkToNPC(this);
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
    }

    public DialogueManager.Dialogue GetCurrentDialogue()
    {
        return dialogues[dialogueIndex];
    }

    public void IncrementIndex()
    {
        dialogueIndex++;
    }

    public DialogueManager.Dialogue GetNextDialogue()
    {
        if (dialogueIndex + 1 > dialogues.Count - 1)
        {
            player.LeaveNPC();
        }
        else
            IncrementIndex();

        return dialogues[dialogueIndex];
    }

    public DialogueManager.Dialogue GetDialogueFromIndex(int index)
    {
        dialogueIndex = index;
        return dialogues[dialogueIndex];
    }

    public DialogueManager.PopupDialogue GetDialoguePopupFromIndex(int index)
    {
        return popupDialogues[index];
    }
}