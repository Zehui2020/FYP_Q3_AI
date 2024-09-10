using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;
using System.Xml.Linq;
using Unity.VisualScripting;


public class NPC_Dialogue_Generator : MonoBehaviour
{
    public GameObject AI_Chat_Canvas;

    public TextMeshProUGUI chatbox_Output;
    public TMP_InputField user_Input;

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
    private string modelDirectory = @"C:\llama.cpp\models\Trail_3\llama-2-7b-chat.Q4_K_M.gguf";
    //Full Prompt to feed into AI
    private string AI_Gen_Prompt;

    private bool inConvo;
    private bool introFinished;
    private bool convoStartedAgain;

    // Start is called before the first frame update
    void Start()
    {
        //AI_Chat_Introduction();
        inConvo = false;
        introFinished = false;
        convoStartedAgain = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void AI_Chat_Initiate()
    {
        if (!introFinished)
        {
            AI_Chat_Introduction();
            UnityEngine.Debug.Log("Intro Starting...");
        }
        if (convoStartedAgain)
        {
            AI_Chat_Response();
            UnityEngine.Debug.Log("Convo Starting...");
        }
    }

    private void AI_Chat_Introduction()
    {
        AI_Gen_Prompt =
            '"' +
            "[INST] <<SYS>> You are the voice of a NPC in a video game. " +
            "This is the NPC's backstory:  " +
            "~" + AI_CharacterContext + "~ " +
            "In this environment, address the user as Lone One, " +
            "keep your responses less than 100 words, " +
            "your responses should be purely dialogue, " +
            "do not depict actions, avoid writing content like *nods*, *walks over*, *leans in* " +
            "and do not show XML tags other than these ones: <result></result>" +

            "Here are a few examples of what your output should look like: " +
            "<result>" + AI_Example_Output_1 +"</result> " +
            "<result>" + AI_Example_Output_2 + "</result> " +

            "Here is your first prompt: <</SYS>> {" + introPrompt + "} [/INST]" + '"';

        string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

        string AI_Output = OpenCommandPrompt(fullCommand_AIChat);

        string Display_Output;

        if (!string.IsNullOrEmpty(ExtractContent(AI_Output)))
        {
            Display_Output = ExtractContent(AI_Output);
        }
        else
        {
            Display_Output = AI_Output;
        }

        chatbox_Output.text = Display_Output;
        previousContext = Display_Output;

        introFinished = true;
    }

    public void AI_Chat_Response()
    {
        if (convoStartedAgain)
        {
            AI_Gen_Prompt =
                '"' +
                "[INST] <<SYS>> You are the voice of a NPC in a video game. " +
                "This is the NPC's backstory:  " +
                "~" + AI_CharacterContext + "~ " +
                "In this environment, address the user as Lone One, " +
                "keep your responses less than 100 words, " +
                "your responses should be purely dialogue, " +
                "do not depict actions, avoid writing content like *nods*, *walks over*, *leans in* " +
                "and do not show XML tags other than these ones: <result></result>" +

                "Here are a few examples of what your output should look like: " +
                "<result>" + AI_Example_Output_1 + "</result> " +
                "<result>" + AI_Example_Output_2 + "</result> " +
                "Here is the input: <</SYS>> {The player come back around yet again, looking for another conversation.} [/INST]" +
                '"';

            if (!string.IsNullOrEmpty(AI_Gen_Prompt))
            {
                string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

                string AI_Output = OpenCommandPrompt(fullCommand_AIChat);
                //UnityEngine.Debug.Log("Full Command: " + fullCommand_AIChat);

                string Display_Output;

                if (!string.IsNullOrEmpty(ExtractContent(AI_Output)))
                {
                    Display_Output = ExtractContent(AI_Output);
                }
                else
                {
                    Display_Output = AI_Output;
                }

                chatbox_Output.text = Display_Output;
                previousContext = Display_Output;
            }

            convoStartedAgain = false;
        }
        else
        {
            if (!string.IsNullOrEmpty(user_Input.text))
            {
                if (string.IsNullOrEmpty(previousContext))
                {
                    //UnityEngine.Debug.Log("Previous Context: NIL");

                    userPrompt = user_Input.text;

                    AI_Gen_Prompt =
                        '"' +
                        "[INST] <<SYS>> You are the voice of a NPC in a video game. " +
                        "This is the NPC's backstory:  " +
                        "~" + AI_CharacterContext + "~ " +
                        "In this environment, address the user as Lone One, " +
                        "keep your responses less than 100 words, " +
                        "your responses should be purely dialogue, " +
                        "do not depict actions, avoid writing content like *nods*, *walks over*, *leans in* " +
                        "and do not show XML tags other than these ones: <result></result>" +

                        "Here are a few examples of what your output should look like: " +
                        "<result>" + AI_Example_Output_1 + "</result> " +
                        "<result>" + AI_Example_Output_2 + "</result> " +

                        "Here is the player's input: <</SYS>> {" + userPrompt + "} [/INST]" + '"';
                }
                else
                {
                    //UnityEngine.Debug.Log("Previous Context: " + previousContext);

                    userPrompt = user_Input.text;

                    AI_Gen_Prompt =
                        '"' +
                        "[INST] <<SYS>> You are the voice of a NPC in a video game. " +
                        "This is the NPC's backstory:  " +
                        "~" + AI_CharacterContext + "~ " +
                        "In this environment, address the user as Lone One, " +
                        "keep your responses less than 100 words, " +
                        "your responses should be purely dialogue, " +
                        "do not depict actions, avoid writing content like *nods*, *walks over*, *leans in* " +
                        "and do not show XML tags other than these ones: <result></result>" +

                        "Here are a few examples of what your output should look like: " +
                        "<result>" + AI_Example_Output_1 + "</result> " +
                        "<result>" + AI_Example_Output_2 + "</result> " +

                        "Your previous response was : " + "~" + previousContext + "~" +
                        "Here is the player's input: <</SYS>> {" + userPrompt + "} [/INST]" +
                        '"';
                }

                if (!string.IsNullOrEmpty(AI_Gen_Prompt))
                {
                    string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

                    string AI_Output = OpenCommandPrompt(fullCommand_AIChat);
                    //UnityEngine.Debug.Log("Full Command: " + fullCommand_AIChat);

                    string Display_Output;

                    if (!string.IsNullOrEmpty(ExtractContent(AI_Output)))
                    {
                        Display_Output = ExtractContent(AI_Output);
                    }
                    else
                    {
                        Display_Output = AI_Output;
                    }

                    chatbox_Output.text = Display_Output;
                    previousContext = Display_Output;
                }
            }
        }
    }

    public void AI_Chat_End()
    {
        AI_Gen_Prompt =
            '"' +
            "[INST] <<SYS>> You are the voice of a NPC in a video game. " +
            "This is the NPC's backstory:  " +
            "~" + AI_CharacterContext + "~ " +
            "In this environment, address the user as Lone One, " +
            "keep your responses less than 100 words, " +
            "your responses should be purely dialogue, " +
            "do not depict actions, avoid writing content like *nods*, *walks over*, *leans in* " +
            "and do not show XML tags other than these ones: <result></result>" +

            "Here are a few examples of what your output should look like: " +
            "<result>" + AI_Example_Output_1 + "</result> " +
            "<result>" + AI_Example_Output_2 + "</result> " +

            "Your previous response was : " + "~" + previousContext + "~" +
            "Here is your prompt: <</SYS>> {Bid the player farewell.} [/INST]" + '"';

        string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

        string AI_Output = OpenCommandPrompt(fullCommand_AIChat);

        string Display_Output;

        if (!string.IsNullOrEmpty(ExtractContent(AI_Output)))
        {
            Display_Output = ExtractContent(AI_Output);
        }
        else
        {
            Display_Output = AI_Output;
        }

        chatbox_Output.text = Display_Output + "\n\n " + AI_Exit_Text;
        previousContext = Display_Output;

        inConvo = false;
        convoStartedAgain = true;
    }

    public void AI_Chat_Return()
    {
        inConvo = false;
        chatbox_Output.text = "";
        AI_Chat_Canvas.SetActive(false);
        playerController.enabled = true;
    }

    string OpenCommandPrompt(string command)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", $"/c {command}");
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        //UnityEngine.Debug.Log("Player Prompt Count: " + promptCount);
        //UnityEngine.Debug.Log("Output: " + output);

        return output;
        //cmd_Output.text = ExtractContent(output);
    }

    string ExtractContent(string text)
    {
        string pattern = "<result>(.*?)</result>";
        Match match = Regex.Match(text, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UnityEngine.Debug.Log("Waiting...");

            if (!inConvo)
            {
                UnityEngine.Debug.Log("UI Up...");

                AI_Chat_Canvas.SetActive(true);

                playerController = collision.GetComponent<PlayerController>();
                playerRB = collision.GetComponent<Rigidbody2D>();

                playerController.enabled = false;
                playerRB.velocity = Vector3.zero;

                inConvo = true;
            }
            /*
            if (Input.GetKeyUp(KeyCode.F))
            {
                
            }
            */
        }
    }
}