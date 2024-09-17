using System.Collections;
using UnityEngine;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;

public class NPC_Dialogue_Generator : MonoBehaviour
{
    public GameObject AI_Chat_Canvas;

    public TextMeshProUGUI chatbox_Output;
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
        }
        if (convoStartedAgain)
        {
            AI_Chat_Response();
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
            "<result>" + AI_Example_Output_1 + "</result> " +
            "<result>" + AI_Example_Output_2 + "</result> " +

            "Here is your first prompt: <</SYS>> {" + introPrompt + "} [/INST]" + '"';

        string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

        StartCoroutine(OpenCommandPrompt(fullCommand_AIChat));
        UnityEngine.Debug.Log("Introducing...");
        introFinished = true;
        /*
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
        */
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
                "Here is the input: <</SYS>> {The player came back for another conversation. You've already spoken to them, speak to them again.} [/INST]" +
                '"';

            if (!string.IsNullOrEmpty(AI_Gen_Prompt))
            {
                string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

                StartCoroutine(OpenCommandPrompt(fullCommand_AIChat));
                UnityEngine.Debug.Log("Talking to NPC again...");

                /*
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
                */
            }

            convoStartedAgain = false;
        }
        else
        {
            if (!string.IsNullOrEmpty(user_Input.text))
            {
                if (string.IsNullOrEmpty(previousContext))
                {
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

                        "Here is the player's input: <</SYS>> {" + userPrompt + "} [/INST]" + 
                        '"';
                }
                else
                {
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
            }
            else
            {
                if (string.IsNullOrEmpty(previousContext))
                {
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

                        "Here is the prompt: <</SYS>> {The player remains silent.} [/INST]" + 
                        '"';
                }
                else
                {
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
                        "Here is the player's input: <</SYS>> {The player remains silent.} [/INST]" +
                        '"';
                }
            }

            if (!string.IsNullOrEmpty(AI_Gen_Prompt))
            {
                string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

                StartCoroutine(OpenCommandPrompt(fullCommand_AIChat));

                user_Input.text = "";

                UnityEngine.Debug.Log("Continuing to speak with NPC...");
            }
        }
    }

    public void AI_Chat_End()
    {
        if (!string.IsNullOrEmpty(user_Input.text))
        {
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
            "Here is your prompt: <</SYS>> {Bid the player farewell after they say to you: " + userPrompt + "} [/INST]" + '"';
        }
        else
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
        }

        string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

        StartCoroutine(OpenCommandPrompt(fullCommand_AIChat));

        user_Input.text = "";

        inConvo = false;
        convoStartedAgain = true;

        UnityEngine.Debug.Log("NPC bids farewell...");
        /*
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
        */
    }

    public void AI_Chat_Return()
    {
        inConvo = false;
        chatbox_Output.text = "";
        AI_Chat_Canvas.SetActive(false);
        playerController.enabled = true;
    }

    IEnumerator OpenCommandPrompt(string command)
    {
        string AI_Output = "";
        bool AI_ChatUpdated = false;
        ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", $"/c {command}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        Process process = new Process
        {
            StartInfo = startInfo
        };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                AI_Output += e.Data + "\n";
                UnityEngine.Debug.Log("Receiving Data from AI: " + e.Data);
                UnityEngine.Debug.Log("Chatbox updating with Data: " + AI_Output);

                //Debug Log shows that this is only updated when the full Generation from the AI is complete.
                //Note: Ask Mr Tan for follow-up

                StartCoroutine(UpdateChatboxOutput(AI_Output));
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                //Note: These are actually errors. This is just to distinguish the Text Generation from the Statistics Output
                UnityEngine.Debug.Log(e.Data);
            }
        };

        process.Start();

        process.BeginErrorReadLine();

        process.BeginOutputReadLine();

        yield return new WaitUntil(() => process.HasExited);

        UnityEngine.Debug.Log("Process Finished: " + process.HasExited);

        // Ensure the final output is updated
        do
        {
            if (process.HasExited && AI_Output.Contains("</result>"))
            {
                //Bug: e.Data and AI_Output can contain AI_Text_Generation
                //     but the UpdateChatboxOutput(ExtractContent(AI_Output)) could end up running with a null string.
                UnityEngine.Debug.Log("Extracting Dialogue from Data: " + AI_Output);
                StartCoroutine(UpdateChatboxOutput(ExtractContent(AI_Output)));

                AI_LoadingUI.SetActive(false);
                AI_ChatUpdated = true;
            }
        }
        while (!AI_ChatUpdated);
    }

    IEnumerator UpdateChatboxOutput(string output)
    {
        chatbox_Output.text = output;
        previousContext = output;

        UnityEngine.Debug.Log("Chatbox should now show Data: " + output);

        UnityEngine.Debug.Log("Chatbox now showing: " + chatbox_Output.text);

        yield return null;
    }

    string ExtractContent(string text)
    {
        string pattern = "<result>(.*?)</result>";
        Match match = Regex.Match(text, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }
        else
        {
            return text;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!inConvo)
            {
                AI_Chat_Canvas.SetActive(true);

                playerController = collision.GetComponent<PlayerController>();
                playerRB = collision.GetComponent<Rigidbody2D>();

                playerController.enabled = false;
                playerRB.velocity = Vector3.zero;

                inConvo = true;
            }
        }
    }
}