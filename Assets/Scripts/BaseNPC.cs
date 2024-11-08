using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static DialogueManager;

public class BaseNPC : MonoBehaviour, IInteractable
{
    private PlayerController player;

    [SerializeField] private SimpleAnimation keycodeUI;
    [SerializeField] private List<Dialogue> dialogues;
    [SerializeField] private List<PopupDialogue> popupDialogues;

    public UnityEvent InteractEvent;

    private int dialogueIndex;

    public float minPitch, maxPitch;

    private void Start()
    {
        player = PlayerController.Instance;
        foreach (PopupDialogue dialogue in popupDialogues)
        {
            if (dialogue.playOnAwake)
                PlayerController.Instance.dialogueManager.ShowDialoguePopup(dialogue, minPitch, maxPitch);
        }
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

    public Dialogue GetCurrentDialogue()
    {
        if (dialogueIndex >= dialogues.Count)
            return null;

        return dialogues[dialogueIndex];
    }

    public void IncrementIndex(int amount)
    {
        dialogueIndex += amount;
    }

    public Dialogue GetNextDialogue()
    {
        if (dialogueIndex + 1 > dialogues.Count - 1)
        {
            player.LeaveNPC();
        }
        else
            IncrementIndex(1);

        Dialogue dialogue = dialogues[dialogueIndex];
        dialogue.SetIsShown(true);
        dialogues[dialogueIndex] = dialogue;

        return dialogues[dialogueIndex];
    }

    public Dialogue GetDialogueFromIndex(int index)
    {
        dialogueIndex = index;

        Dialogue dialogue = dialogues[dialogueIndex];
        dialogue.SetIsShown(true);
        dialogues[dialogueIndex] = dialogue;

        return dialogues[dialogueIndex];
    }

    public PopupDialogue? GetDialoguePopupFromIndex(int index)
    {
        if (popupDialogues[index].isShown)
            return null;

        PopupDialogue dialoguePopup = popupDialogues[index];
        dialoguePopup.SetIsShown(true);
        popupDialogues[index] = dialoguePopup;

        return popupDialogues[index];
    }
}