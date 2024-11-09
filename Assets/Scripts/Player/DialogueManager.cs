using DesignPatterns.ObjectPool;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using static DialogueManager;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public struct DialogueEvent
    {
        public bool playOnce;
        private bool isInvoked;
        public UnityEvent onDialogueDone;

        public void InvokeEvent()
        {
            if (isInvoked && playOnce)
                return;

            onDialogueDone?.Invoke();
            isInvoked = true;
        }

        public void ResetEvent()
        {
            isInvoked = false;
        }
    }

    [System.Serializable]
    public class Dialogue
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
            public DialogueOptionData optionData;
            public int nextDialogueIndex;
        }

        public bool showOnce;
        public bool isLoopingDialogue;
        [HideInInspector] public bool isShown;

        public SpeakerType speakerType;
        public string speakerName;
        public Sprite speakerIcon;
        public bool breakAfterDialogue;
        public DialogueEvent onDialogueDone;
        [TextArea(1, 10)] public string dialogue;
        public List<DialogueChoice> playerChoices = new();
        public Transform questDestination;

        public void SetIsShown(bool shown)
        {
            isShown = shown;
        }

        public void ResetDialogue()
        {
            isShown = false;
            onDialogueDone.ResetEvent();
        }
    }

    [System.Serializable]
    public struct PopupDialogue
    {
        public bool showOnce;
        public bool playOnAwake;
        public bool sequenceDependent;
        public float lingerDuration;
        [HideInInspector] public bool isShown;

        public string speakerName;
        public Sprite speakerIcon;
        public DialogueEvent onDialogueDone;
        [TextArea(1, 10)] public string dialogue;
        public Transform questDestination;

        public void SetIsShown(bool shown)
        {
            isShown = shown;
        }

        public void ResetDialoguePopup()
        {
            isShown = false;
            onDialogueDone.ResetEvent();
        }
    }

    public static DialogueManager Instance;

    [SerializeField] private DialoguePopup dialoguePopup;

    public TypewriterEffect npcDialogue;
    [SerializeField] private QuestPointer questPointer;

    [SerializeField] private Image playerPortrait;
    [SerializeField] private Image npcPortrait;
    [SerializeField] private Color showColor;
    [SerializeField] private Color hideColor;

    [SerializeField] private Canvas dialogueCanvas;
    [SerializeField] private RectTransform dialogueChoiceParent;
    [SerializeField] private GameObject dialogueBox;

    private List<DialogueChoice> dalogueChoices = new();
    [SerializeField] private BaseNPC currentNPC;

    private Dialogue currentDialogue;
    private bool canShowNextDialogue;
    public bool lockShowNextDialogue = false;

    private int previousIndex = 0;
    private Coroutine DelayRoutine;

    private void Awake()
    {
        Instance = this;
    }

    public void SetNPC(BaseNPC npc)
    {
        currentNPC = npc;
    }

    public void SetTalkingNPC(BaseNPC npc)
    {
        currentNPC = npc;
        dialogueCanvas.enabled = true;
        ShowDialogue(npc.GetCurrentDialogue(), npc.minPitch, npc.maxPitch);
        dialoguePopup.HidePopupImmediately();
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
        if (lockShowNextDialogue)
        {
            if (dalogueChoices.Count == 0 && !currentNPC.GetDialogueGenerator().isGenerating)
                npcDialogue.Skip();

            return;
        }

        if (!canShowNextDialogue)
        {
            if (dalogueChoices.Count == 0)
                npcDialogue.Skip();

            return;
        }

        if (currentDialogue.showOnce && currentDialogue.isShown)
            return;

        currentDialogue.onDialogueDone.InvokeEvent();

        if (currentDialogue.breakAfterDialogue)
        {
            if (!currentDialogue.isLoopingDialogue)
                currentNPC.IncrementIndex(1);

            PlayerController.Instance.LeaveNPC();
            return;
        }

        ShowDialogue(currentNPC.GetNextDialogue(), currentNPC.minPitch, currentNPC.maxPitch);
    }

    public void ShowDialogue(Dialogue dialogue, float min, float max)
    {
        if (dialogue == null)
            return;

        currentDialogue = dialogue;

        canShowNextDialogue = false;
        npcDialogue.SetSpeakerName(dialogue.speakerName, min, max);
        npcPortrait.sprite = dialogue.speakerIcon;

        // Show message
        if (dialogue.speakerType != Dialogue.SpeakerType.Player)
        {
            if (dialogue.dialogue == string.Empty)
            {
                dialogueBox.SetActive(false);
                return;
            }

            dialogueBox.SetActive(true);
            npcDialogue.ShowMessage(dialogue.speakerName, dialogue.dialogue, min, max, 0);
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
        else
            questPointer.Hide();
    }

    public void ShowDialoguePopup(int index)
    {
        if (currentNPC == null)
            return;

        PopupDialogue? dialogue = currentNPC.PeekDialoguePopupFromIndex(index);

        if (dialogue != null)
        {
            if (dialogue.Value.sequenceDependent && index - 1 == previousIndex)
            {
                dialogue = currentNPC.GetDialoguePopupFromIndex(index);
                dialoguePopup.ShowDialoguePopup(dialogue, currentNPC.minPitch, currentNPC.maxPitch);
                previousIndex = index;
            }
            else if (!dialogue.Value.sequenceDependent)
            {
                dialogue = currentNPC.GetDialoguePopupFromIndex(index);
                dialoguePopup.ShowDialoguePopup(dialogue, currentNPC.minPitch, currentNPC.maxPitch);
                previousIndex = index;
            }
        }
    }
    public void ShowDialoguePopupWithDelay(int index)
    {
        if (DelayRoutine != null)
            StopCoroutine(DelayRoutine);

        DelayRoutine = StartCoroutine(ShowDialoguePopupDelay(index));
    }
    private IEnumerator ShowDialoguePopupDelay(int index)
    {
        yield return new WaitForSeconds(10f);

        if (currentNPC == null)
        {
            DelayRoutine = null;
            yield break;
        }

        PopupDialogue? dialogue = currentNPC.PeekDialoguePopupFromIndex(index);

        if (dialogue != null)
        {
            if (dialogue.Value.sequenceDependent && index - 1 == previousIndex)
            {
                dialogue = currentNPC.GetDialoguePopupFromIndex(index);
                dialoguePopup.ShowDialoguePopup(dialogue, currentNPC.minPitch, currentNPC.maxPitch);
                previousIndex = index;
            }
            else if (!dialogue.Value.sequenceDependent)
            {
                dialogue = currentNPC.GetDialoguePopupFromIndex(index);
                dialoguePopup.ShowDialoguePopup(dialogue, currentNPC.minPitch, currentNPC.maxPitch);
                previousIndex = index;
            }
        }

        DelayRoutine = null;
    }

    public void ShowDialoguePopup(PopupDialogue dialogue, float min, float max)
    {
        dialoguePopup.ShowDialoguePopup(dialogue, min, max);
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
                if (currentChoice.nextDialogueIndex != -1 || currentChoice.optionData == null)
                    ShowDialogue(currentNPC.GetDialogueFromIndex(currentChoice.nextDialogueIndex), currentNPC.minPitch, currentNPC.maxPitch);
                else
                    currentNPC.GetDialogueGenerator().AI_Chat_Response(currentChoice.optionData);

                foreach (DialogueChoice dialogue in dalogueChoices)
                    dialogue.ReturnToPool();

                dalogueChoices.Clear();
            };
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(dialogueChoiceParent);
    }

    public void HideDialogue()
    {
        dialogueCanvas.enabled = false;
    }
}