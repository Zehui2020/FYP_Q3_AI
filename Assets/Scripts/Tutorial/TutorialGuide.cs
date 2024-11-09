using System.Collections;
using UnityEngine;

public class TutorialGuide : BaseNPC
{
    private NPC_Dialogue_Generator aiManager;
    private TextAnalysis textAnalysis;

    public bool isTutorialGuide;

    // Start is called before the first frame update
    private void Awake()
    {
        aiManager = GetComponent<NPC_Dialogue_Generator>();
        textAnalysis = GetComponent<TextAnalysis>();
    }

    public override void InitNPC()
    {
        base.InitNPC();

        if (!isTutorialGuide)
            aiManager.InitAIManager();
    }

    public override bool OnInteract()
    {
        aiManager.EnterNPCDialogue();
        return true;
    }

    public void OnLeaveConvo()
    {
        aiManager.AI_Chat_Exit();
        player.ChangeState(PlayerController.PlayerStates.Movement);
    }
}