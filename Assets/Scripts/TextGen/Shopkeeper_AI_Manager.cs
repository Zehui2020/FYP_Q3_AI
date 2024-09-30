using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Shopkeeper_AI_Manager : MonoBehaviour
{
    public GameObject Shop_Chat_Canvas;

    //For conversing with the Shopkeeper
    public TextMeshProUGUI Shopkeeper_Chat_Output;

    public TMP_InputField user_Input;
    public GameObject AI_LoadingUI;

    public string AI_CharacterContext;
    public string introPrompt;

    public string AI_Example_Output_1;
    public string AI_Example_Output_2;

    [Header("Shimba's Shop UI")]
    //Shimba's Shop UI
    public GameObject Shop_Page_Canvas;
    public GameObject Shop_Catalog_Parent;
    public GameObject Shop_AI_Image;
    public GameObject Shop_AI_Blocker;

    [Header("Sentiment Analysis")]
    //Sentiment Analysis
    public TextAnalysis AI_Sentiment_Analysis;

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

    private bool analyseText;

    // Start is called before the first frame update
    void Start()
    {
        inConvo = false;
        introFinished = false;
        convoStartedAgain = false;

        analyseText = false;
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
            "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
            "This is the NPC's backstory:  " +
            "~" + AI_CharacterContext + "~ " +
            "In this environment, address the user as Adventurer, " +
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
    }

    public void AI_Chat_Response()
    {
        if (convoStartedAgain)
        {
            AI_Gen_Prompt =
                '"' +
                "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
                "This is the NPC's backstory:  " +
                "~" + AI_CharacterContext + "~ " +
                "In this environment, address the user as Adventurer, " +
                "keep your responses less than 100 words, " +
                "your responses should be purely dialogue, " +
                "do not depict actions, avoid writing content like *nods*, *walks over*, *leans in* " +
                "and do not show XML tags other than these ones: <result></result>" +

                "Here are a few examples of what your output should look like: " +
                "<result>" + AI_Example_Output_1 + "</result> " +
                "<result>" + AI_Example_Output_2 + "</result> " +
                "Here is the input: <</SYS>> {The same player/customer came back for another conversation. You've already met them before, address them with familiarity.} [/INST]" +
                '"';

            if (!string.IsNullOrEmpty(AI_Gen_Prompt))
            {
                string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

                StartCoroutine(OpenCommandPrompt(fullCommand_AIChat));
                UnityEngine.Debug.Log("Talking to NPC again...");
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
                        "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
                        "This is the NPC's backstory:  " +
                        "~" + AI_CharacterContext + "~ " +
                        "In this environment, address the user as Adventurer, " +
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
                        "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
                        "This is the NPC's backstory:  " +
                        "~" + AI_CharacterContext + "~ " +
                        "In this environment, address the user as Adventurer, " +
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
                        "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
                        "This is the NPC's backstory:  " +
                        "~" + AI_CharacterContext + "~ " +
                        "In this environment, address the user as Adventurer, " +
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
                        "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
                        "This is the NPC's backstory:  " +
                        "~" + AI_CharacterContext + "~ " +
                        "In this environment, address the user as Adventurer, " +
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
            "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
            "This is the NPC's backstory:  " +
            "~" + AI_CharacterContext + "~ " +
            "In this environment, address the user as Adventurer, " +
            "keep your responses less than 100 words, " +
            "your responses should be purely dialogue, " +
            "do not depict actions, avoid writing content like *nods*, *walks over*, *leans in* " +
            "and do not show XML tags other than these ones: <result></result>" +

            "Here are a few examples of what your output should look like: " +
            "<result>" + AI_Example_Output_1 + "</result> " +
            "<result>" + AI_Example_Output_2 + "</result> " +

            "Your previous response was: " + "~" + previousContext + "~" +
            "Now this is your prompt: <</SYS>> {Bid the player farewell after they say to you: " + userPrompt + "} [/INST]" + '"';
        }
        else
        {
            AI_Gen_Prompt =
            '"' +
            "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
            "This is the NPC's backstory:  " +
            "~" + AI_CharacterContext + "~ " +
            "In this environment, address the user as Adventurer, " +
            "keep your responses less than 100 words, " +
            "your responses should be purely dialogue, " +
            "do not depict actions, avoid writing content like *nods*, *walks over*, *leans in* " +
            "and do not show XML tags other than these ones: <result></result>" +

            "Here are a few examples of what your output should look like: " +
            "<result>" + AI_Example_Output_1 + "</result> " +
            "<result>" + AI_Example_Output_2 + "</result> " +

            "Your previous response was: " + "~" + previousContext + "~" +
            "Now this is your prompt: <</SYS>> {Bid the player farewell.} [/INST]" + '"';
        }

        string fullCommand_AIChat = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

        StartCoroutine(OpenCommandPrompt(fullCommand_AIChat));

        user_Input.text = "";

        inConvo = false;
        convoStartedAgain = true;

        UnityEngine.Debug.Log("NPC bids farewell...");
    }

    public void AI_Chat_Return()
    {
        inConvo = false;
        Shopkeeper_Chat_Output.text = "";
        Shop_Chat_Canvas.SetActive(false);

        Shop_Page_Canvas.SetActive(false);

        playerController.enabled = true;
    }

    public void SetAnalyseText()
    {
        analyseText = true;
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
            StartInfo = startInfo,
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

                //StartCoroutine(UpdateChatboxOutput(AI_Output));
            }
        };

        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
            {
                //Note: These are actually errors. This is just to distinguish the Text Generation from the Statistics Output
                //UnityEngine.Debug.Log(e.Data);
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

                Shopkeeper_Chat_Output.text = ExtractContent(AI_Output);
                previousContext = ExtractContent(AI_Output);

                AI_LoadingUI.SetActive(false);
                AI_ChatUpdated = true;
            }
        }
        while (!AI_ChatUpdated);

        if (analyseText)
        {
            AI_Sentiment_Analysis.SendPredictionText(ExtractContent(AI_Output));
            analyseText = false;
        }

        //Handle Shimba's UI
        //Block_AI_IMG(false);
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

    //Handle Shimba's AI Image UI
    private void Block_AI_IMG(bool variable)
    {
        if (variable)
        {
            Shop_AI_Image.SetActive(false);
            Shop_AI_Blocker.SetActive(true);
        }
        else
        {
            Shop_AI_Image.SetActive(true);
            Shop_AI_Blocker.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            if (!inConvo)
            {
                Shop_Chat_Canvas.SetActive(true);

                //{
                //Shimba's UI
                Shop_Page_Canvas.SetActive(true);
                //Shop_Catalog_Parent.SetActive(false);

                //Block_AI_IMG(true);
                
                //}

                playerController = collision.GetComponent<PlayerController>();
                playerRB = collision.GetComponent<Rigidbody2D>();

                playerController.enabled = false;
                playerRB.velocity = Vector3.zero;

                inConvo = true;
            }
        }
    }
}
