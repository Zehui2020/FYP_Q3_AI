using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour, IInteractable
{
    private NPC_UI_Manager uiManager;
    private NPC_Dialogue_Generator aiManager;
    private TextAnalysis textAnalysis;

    private PlayerController player;

    [SerializeField] private SimpleAnimation keycodeE;
    public bool isDebugging;

    // Start is called before the first frame update
    private void Awake()
    {
        uiManager = GetComponent<NPC_UI_Manager>();
        aiManager = GetComponent<NPC_Dialogue_Generator>();
        textAnalysis = GetComponent<TextAnalysis>();
    }

    private IEnumerator Start()
    {
        if (!isDebugging)
        {
            aiManager.InitAIManager();
            Debug.Log("Starting Canis NPC");
        }

        player = PlayerController.Instance;
        uiManager.InitUIManager();

        yield return new WaitForSeconds(1f);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnEnterRange()
    {
        keycodeE.Show();
    }

    public bool OnInteract()
    {
        uiManager.ShowUI();
        uiManager.isInteracting = true;
        aiManager.EnterNPCDialogue();
        //player.ChangeState(PlayerController.PlayerStates.State);
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeE.Hide();
    }

    public void OnLeaveConvo()
    {
        //aiManager.OnLeaveConvo();
        uiManager.OnLeaveNPCConvo();
        aiManager.AI_Chat_Exit();
        player.ChangeState(PlayerController.PlayerStates.Movement);
    }
}
