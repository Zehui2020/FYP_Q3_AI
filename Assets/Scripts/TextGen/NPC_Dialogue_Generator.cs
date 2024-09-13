using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;
using System.Xml.Linq;


public class NPC_Dialogue_Generator : MonoBehaviour
{
    public TextMeshProUGUI chatbox_Output;

    public TMP_InputField user_Input;

    private string userPrompt;

    private string llamaDirectory = @"C:\llama.cpp";
    private string modelDirectory = @"C:\llama.cpp\models\Trail_3\llama-2-7b-chat.Q4_K_M.gguf";
    private string AI_Gen_Prompt;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
