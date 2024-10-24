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
    [SerializeField] private GameObject DialogueOptionsList;
    [SerializeField] private GameObject OptionPrefab;
    [SerializeField] private List<DialogueOptionData> DO_DataList;

    public bool isInteracting = false;
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(DialogueOptionsList.transform.childCount);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitUIManager()
    {
        player = PlayerController.Instance;
        SetupOptions();
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
        NPC_Dialogue_Canvas.gameObject.SetActive(false);
        user_Input.text = string.Empty;
        isInteracting = false;
    }

    public string GetUserInput()
    {
        return user_Input.text;
    }

    private void SetupOptions()
    {
        for (int i = 0; i < DO_DataList.Count; i++)
        {
            string HeaderText = (i+1).ToString() + ".) ";

            GameObject newOption = Instantiate(OptionPrefab);
            NPC_Dialogue_Tree newDT_Script = newOption.GetComponent<NPC_Dialogue_Tree>();

            newDT_Script.OptionData = DO_DataList[i];
            newDT_Script.SetTitle(HeaderText);

            newDT_Script.ai_TextGen = GetComponent<NPC_Dialogue_Generator>();
            newOption.transform.SetParent(DialogueOptionsList.transform);
        }
        SetOptionsListSize();
    }

    private void SetOptionsListSize()
    {
        RectTransform rectTransform = DialogueOptionsList.GetComponent<RectTransform>();
        float newHeight = 100 + ((DialogueOptionsList.transform.childCount - 1) * (100 + 12.5f));
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, newHeight);
    }

    public void TurnOffDialogueTree()
    {
        for (int i = 0; i < DialogueOptionsList.transform.childCount; i++)
        {
            Button buttonComponent = DialogueOptionsList.transform.GetChild(i).GetComponent<Button>();
            buttonComponent.interactable = false;
        }
    }

    public void TurnOnDialogueTree()
    {
        for (int i = 0; i < DialogueOptionsList.transform.childCount; i++)
        {
            Button buttonComponent = DialogueOptionsList.transform.GetChild(i).GetComponent<Button>();
            buttonComponent.interactable = true;
        }
    }
}
