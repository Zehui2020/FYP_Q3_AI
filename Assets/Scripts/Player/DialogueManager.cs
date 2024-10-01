using DesignPatterns.ObjectPool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static DialogueManager;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public struct Dialogue
    {
        public enum SpeakerType
        {
            Player,
            NPC
        }

        [System.Serializable]
        public struct DialogueChoice
        {
            public string choice;
            public int nextDialogueIndex;
        }

        public SpeakerType speakerType;
        public string speakerName;
        public Sprite speakerIcon;
        public bool breakAfterDialogue;
        public UnityEvent onDialogueDone;
        [TextArea(1, 10)] public string dialogue;
        public List<DialogueChoice> playerChoices;
        public Transform questDestination;
    }

    [System.Serializable]
    public struct PopupDialogue
    {
        public string speakerName;
        public Sprite speakerIcon;
        [TextArea(1, 10)] public string dialogue;
        public Transform questDestination;
    }

    [SerializeField] private DialoguePopup dialoguePopup;

    [SerializeField] private TypewriterEffect npcDialogue;
    [SerializeField] private QuestPointer questPointer;

    [SerializeField] private Image playerPortrait;
    [SerializeField] private Image npcPortrait;
    [SerializeField] private Color showColor;
    [SerializeField] private Color hideColor;

    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private RectTransform dialogueChoiceParent;
    [SerializeField] private GameObject dialogueBox;

    private List<DialogueChoice> dalogueChoices = new();
    private BaseNPC currentNPC;

    private Dialogue currentDialogue;
    private bool canShowNextDialogue;

    public void SetTalkingNPC(BaseNPC npc)
    {
        currentNPC = npc;
        dialogueCanvas.enabled = true;
        ShowDialogue(npc.GetCurrentDialogue());
    }

    public void SetCanShowNextDialogue(bool canShow)
    {
        canShowNextDialogue = canShow;
    }

    public void CheckShowChoices()
    {
        if (currentDialogue.playerChoices.Count == 0)
            return;

        // Show player choices
        ShowDialogueChoices(currentDialogue);
        playerPortrait.color = showColor;
        npcPortrait.color = hideColor;
        canShowNextDialogue = false;
    }

    public void ShowNextDialogue()
    {
        if (!canShowNextDialogue)
            return;

        currentDialogue.onDialogueDone?.Invoke();

        if (currentDialogue.breakAfterDialogue)
        {
            PlayerController.Instance.LeaveNPC();
            currentNPC.IncrementIndex();
            return;
        }

        ShowDialogue(currentNPC.GetNextDialogue());
    }

    public void ShowDialogue(Dialogue dialogue)
    {
        currentDialogue = dialogue;
        canShowNextDialogue = false;

        // Show message
        if (dialogue.speakerType != Dialogue.SpeakerType.Player)
        {
            if (dialogue.dialogue == string.Empty)
            {
                dialogueBox.SetActive(false);
                return;
            }

            dialogueBox.SetActive(true);
            npcDialogue.ShowMessage(dialogue.speakerName, dialogue.dialogue);
            playerPortrait.color = hideColor;
            npcPortrait.color = showColor;
        }
        else
        {
            CheckShowChoices();
        }

        // Set pointer
        if (dialogue.questDestination != null)
            questPointer.Show(dialogue.questDestination);
    }

    public void ShowDialoguePopup(PopupDialogue popupDialogue)
    {
        dialoguePopup.ShowDialoguePopup(popupDialogue);
    }

    public void ShowDialogueChoices(Dialogue dialogue)
    {
        // Show player choices
        foreach (Dialogue.DialogueChoice choice in dialogue.playerChoices)
        {
            DialogueChoice dialogueChoice = ObjectPool.Instance.GetPooledObject("DialogueChoice", false) as DialogueChoice;
            dalogueChoices.Add(dialogueChoice);
            dialogueChoice.InitChoice(choice);
            dialogueChoice.transform.SetParent(dialogueChoiceParent);
            dialogueChoice.gameObject.SetActive(true);

            dialogueChoice.OnSelectEvent += (currentChoice) => 
            {
                ShowDialogue(currentNPC.GetDialogueFromIndex(currentChoice.nextDialogueIndex)); 
                foreach (DialogueChoice dialogue in dalogueChoices)
                    dialogue.ReturnToPool();
            };
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueChoiceParent);
    }

    public void HideDialogue()
    {
        dialogueCanvas.enabled = false;
    }
}