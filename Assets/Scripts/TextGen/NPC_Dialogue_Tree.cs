using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPC_Dialogue_Tree : MonoBehaviour
{
    [Header("Dialogue Option Data")]
    public DialogueOptionData OptionData;

    [Header("Option Text")]
    [SerializeField] private TextMeshProUGUI OptionText;

    [Header("Option Text")]
    public NPC_Dialogue_Generator ai_TextGen;

    private bool promptUsed;

    // Start is called before the first frame update
    void Start()
    {
        //OptionText.text = OptionData.OptionTitle;
        promptUsed = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTitle(string Header)
    {
        OptionText.text = Header + OptionData.OptionTitle;
    }

    public void PromptResponse()
    {
        if (!promptUsed)
        {
            ai_TextGen.AI_Dialogue_Tree_Response(OptionData.WorldContext, OptionData.OptionTitle);
            promptUsed = true;
        }
    }
}
