using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC_UI_Manager : MonoBehaviour
{

    [Header("Dialogue Canvas")]
    [SerializeField] private GameObject NPC_Dialogue_Canvas;

    [Header("User Input")]
    [SerializeField] private TMP_InputField user_Input;

    [Header("Output Effect")]
    [SerializeField] private TypewriterEffect NPCOutput;

    [Header("NPC's Name")]
    [SerializeField] private string NPCName;

    [Header("Dialogue Options")]
    [SerializeField] private List<Button> DialogueOptions;

    public bool isInteracting = false;
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitUIManager()
    {
        player = PlayerController.Instance;
    }

    public void ShowUI()
    {
        NPC_Dialogue_Canvas.gameObject.SetActive(true);
        //shopUIAnimator.gameObject.SetActive(true);
        //shopUIAnimator.SetTrigger("enter");

    }

    public void HideUI()
    {
        NPC_Dialogue_Canvas.gameObject.SetActive(false);
        //shopUIAnimator.SetTrigger("exit");
    }

    public void SetNPCOutput(string output)
    {
        StartCoroutine(SetOutputRoutine(output));
    }

    private IEnumerator SetOutputRoutine(string output)
    {
        while (!isInteracting)
            yield return null;

        NPCOutput.gameObject.SetActive(true);
        NPCOutput.ShowMessage(NPCName, output);
    }

    public void OnLeaveNPCConvo()
    {
        NPCOutput.gameObject.SetActive(false);
        user_Input.text = string.Empty;
        isInteracting = false;
    }

    public string GetUserInput()
    {
        return user_Input.text;
    }

    public void TurnOffDialogueTree()
    {

    }

    public void TurnOnDialogueTree()
    {

    }
}
