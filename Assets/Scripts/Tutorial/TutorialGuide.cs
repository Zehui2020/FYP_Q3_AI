using System.Collections;
using UnityEngine;

public class TutorialGuide : BaseNPC
{
    [SerializeField] private NPC_Dialogue_Tree tree;

    private NPC_Dialogue_Generator aiManager;
    private TextAnalysis textAnalysis;
    public bool isTutorialGuide;

    // Start is called before the first frame update
    private void Awake()
    {
        aiManager = GetComponent<NPC_Dialogue_Generator>();
        textAnalysis = GetComponent<TextAnalysis>();

        aiManager.OnFinishGeneratingResponse.AddListener(SetupDialogues);
    }

    public override void InitNPC()
    {
        base.InitNPC();

        if (!isTutorialGuide)
            aiManager.InitAIManager(tree.npcData);
    }

    public override NPC_Dialogue_Generator GetDialogueGenerator()
    {
        return aiManager;
    }

    private void SetupDialogues(string introduction)
    {
        DialogueManager.Dialogue dialogue = new();

        dialogue.speakerType = tree.npcData.speakerType;
        dialogue.speakerName = tree.npcData.npcName;
        dialogue.speakerIcon = tree.npcData.npcSprite;
        dialogue.speakerIcon = tree.npcData.npcSprite;
        dialogue.dialogue = introduction;

        foreach (DialogueOptionData option in tree.dialogueOptionDatas)
        {
            DialogueManager.Dialogue.DialogueChoice choice = new();

            choice.choice = option.OptionTitle;
            choice.nextDialogueIndex = -1;
            choice.optionData = option;

            dialogue.playerChoices.Add(choice);
        }

        dialogues.Add(dialogue);

        aiManager.OnFinishGeneratingResponse.RemoveListener(SetupDialogues);
        aiManager.OnFinishGeneratingResponse.AddListener(ShowDialogue);
    }
    private void ShowDialogue(string response)
    {
        DialogueManager.Dialogue dialogue = new();

        dialogue.speakerType = tree.npcData.speakerType;
        dialogue.speakerName = tree.npcData.npcName;
        dialogue.speakerIcon = tree.npcData.npcSprite;
        dialogue.dialogue = response;

        foreach (DialogueOptionData option in tree.dialogueOptionDatas)
        {
            DialogueManager.Dialogue.DialogueChoice choice = new();

            choice.choice = option.OptionTitle;
            choice.nextDialogueIndex = -1;
            choice.optionData = option;

            dialogue.playerChoices.Add(choice);
        }

        player.dialogueManager.ShowDialogue(dialogue, minPitch, maxPitch);
    }

    public override bool OnInteract()
    {
        if (dialogues.Count == 0)
        {
            player.dialogueManager.SetNPC(this);
            player.dialogueManager.ShowDialoguePopup(0);
            player.dialogueManager.lockShowNextDialogue = true;
        }
        else
        {
            base.OnInteract();
            //aiManager.EnterNPCDialogue();
        }
        return true;
    }

    public void OnLeaveConvo()
    {
        aiManager.AI_Chat_Exit();
        player.ChangeState(PlayerController.PlayerStates.Movement);
        player.dialogueManager.lockShowNextDialogue = false;
    }
}