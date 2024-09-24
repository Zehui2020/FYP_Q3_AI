using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Shopkeeper_AI_Manager : MonoBehaviour
{
    public GameObject Shop_Chat_Canvas;

    //For conversing with the Shopkeeper
    public TextMeshProUGUI Shopkeeper_Chat_Output;
    //For bargaining with the Shopkeeper
    public TextMeshProUGUI shopkeeper_Bargain_Output;

    public TMP_InputField user_Input;
    public GameObject AI_LoadingUI;

    public string AI_CharacterContext;
    public string introPrompt;

    public string AI_Example_Output_1;
    public string AI_Example_Output_2;

    public string AI_Exit_Text;

    private PlayerController playerController;
    private Rigidbody2D playerRB;

    //User Input
    private string userPrompt;
    //AI Previous Conversation
    private string previousContext;

    //AI Interface Address
    private string llamaDirectory = @"C:\llama.cpp";
    //AI Model Address
    private string modelDirectory = @"C:\llama.cpp\models\LLM\llama-2-7b-chat.Q4_K_M.gguf";
    //Full Prompt to feed into AI
    private string AI_Gen_Prompt;

    //AI Chat Bools
    private bool inConvo;
    private bool introFinished;
    private bool convoStartedAgain;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
