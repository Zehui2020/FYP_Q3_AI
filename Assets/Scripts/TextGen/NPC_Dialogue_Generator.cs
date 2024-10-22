using System.Collections;
using UnityEngine;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;
using System.IO;
using System.Text;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.Events;

public class NPC_Dialogue_Generator : MonoBehaviour
{
    /*
    [Header("Placeholder UI Stuff")]
    public GameObject AI_Chat_Canvas;
    public TextMeshProUGUI chatbox_Output;
    public TMP_InputField user_Input;
    public GameObject AI_LoadingUI;
    */
    [Header("NPC Dialogue UI")]
    [SerializeField] private NPC_UI_Manager npc_UI_Manager;

    [Header("NPC Scriptable Object")]
    [SerializeField] private NPCData NPC_Data;

    [Header("Sentiment Analysis")]
    public TextAnalysis AI_Sentiment_Analysis;

    //public string AI_CharacterContext;
    //public string introPrompt;

    //public string AI_Example_Output_1;
    //public string AI_Example_Output_2;

    //public string AI_Exit_Text;

    private PlayerController playerController;
    private Rigidbody2D playerRB;

    //User Input
    private string userPrompt;
    //AI Previous Conversation
    private string previousContext;

    //AI Chat Bools
    private bool introFinished;
    private bool hasIntroduced;
    private bool analyseText;

    public UnityEvent OnFinishGeneratingResponse;

    //private bool convoStartedAgain;
    //private bool inConvo;

    /*
    //AI Interface Address
    private string llamaDirectory = @"C:\llama.cpp";
    //AI Model Address
    private string modelDirectory = @"C:\llama.cpp\models\LLM\llama-2-7b-chat.Q4_K_M.gguf";
    //Full Prompt to feed into AI
    private string AI_Gen_Prompt;
    */

    /*
    //Command Prompt Stuff
    private Process AI_Chat_Process;
    private StringBuilder AI_Output_Builder = new StringBuilder();
    */

    // Start is called before the first frame update
    /*
    void Start()
    {
        //AI_Chat_Introduction();
        //inConvo = false;
        //convoStartedAgain = false;

        introFinished = false;
        hasIntroduced = false;
        analyseText = false;
    }
    */
    public void InitAIManager()
    {
        introFinished = false;
        analyseText = false;
        hasIntroduced = false;
        //AI_Sentiment_Analysis.OnAnalysisEnabled();

        AI_Chat_Introduction();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*
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
    */

    private string GetFinalPromptString(string promptTitle, string promptContent, string additionalPrompts)
    {

        string AI_Gen_Prompt =
            '"' +
            "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
            "This is the NPC's backstory:  " +
            "~" + NPC_Data.AI_CharacterContext + "~ " +
            "In this environment, address the user as Adventurer, " +
            "keep your responses less than 30 words, " +
            "and do not show XML tags other than these ones: <result></result>" +

            "Here are a few examples of what your output should look like: " +
            "<result>" + NPC_Data.AI_Example_Output_1 + "</result> " +
            "<result>" + NPC_Data.AI_Example_Output_2 + "</result> " +
            //additionalPrompts+
            promptTitle + " <</SYS>> {" + promptContent + "} [/INST]" + '"';

        return $"cd {NPC_Data.llamaDirectory} && llama-cli -m {NPC_Data.modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";
    }

    public void AI_Chat_Introduction()
    {
        if (hasIntroduced)
            return;

        string prompt;

        if (!introFinished)
        {
            prompt = GetFinalPromptString("Here is your first prompt:", NPC_Data.introductionPrompt, string.Empty);
            StartCoroutine(OpenCommandPrompt(prompt));
            introFinished = true;
        }
        else
        {
            prompt = GetFinalPromptString("Here is the input:",
            "The same adventurer/player came back for another conversation. You've already met them before, address them with familiarity.", string.Empty);
            StartCoroutine(OpenCommandPrompt(prompt));
        }

        hasIntroduced = true;
        //analyseText = true;
    }

    /*
    private void AI_Chat_Introduction()
    {
        AI_Gen_Prompt =
            '"' +
            "[INST] <<SYS>> You are the voice of a NPC in a video game. " +
            "This is the NPC's backstory:  " +
            "~" + AI_CharacterContext + "~ " +
            "In this environment, address the user as Lone One, " +
            "keep your responses less than 50 words, " +
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
        
        //string AI_Output = OpenCommandPrompt(fullCommand_AIChat);

        //string Display_Output;

        //if (!string.IsNullOrEmpty(ExtractContent(AI_Output)))
        //{
        //    Display_Output = ExtractContent(AI_Output);
        //}
        //else
        //{
        //    Display_Output = AI_Output;
        //}

        //chatbox_Output.text = Display_Output;
        //previousContext = Display_Output;
        
    }
    */
    /*
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
                "keep your responses less than 50 words, " +
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


                //string AI_Output = OpenCommandPrompt(fullCommand_AIChat);
                ////UnityEngine.Debug.Log("Full Command: " + fullCommand_AIChat);

                //string Display_Output;

                //if (!string.IsNullOrEmpty(ExtractContent(AI_Output)))
                //{
                //    Display_Output = ExtractContent(AI_Output);
                //}
                //else
                //{
                //    Display_Output = AI_Output;
                //}

                //chatbox_Output.text = Display_Output;
                //previousContext = Display_Output;

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
                        "keep your responses less than 50 words, " +
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
                        "keep your responses less than 50 words, " +
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
                        "keep your responses less than 50 words, " +
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
                        "keep your responses less than 50 words, " +
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
    */

    public void AI_Chat_Response()
    {
        string user_Input = npc_UI_Manager.GetUserInput();

        string promptTitle;
        string promptContent;
        string additionalPrompts = string.Empty;

        if (!string.IsNullOrEmpty(user_Input))
        {
            promptTitle = "Here is the player's input:";
            promptContent = user_Input;

            if (!string.IsNullOrEmpty(previousContext))
                additionalPrompts = "Your previous response was : " + "~" + previousContext + "~";
        }
        else
        {
            promptTitle = "Here is the player's input:";
            promptContent = "The player remains silent.";

            if (!string.IsNullOrEmpty(previousContext))
                additionalPrompts = "Your previous response was : " + "~" + previousContext + "~";
        }

        string prompt = GetFinalPromptString(promptTitle, promptContent, additionalPrompts);
        StartCoroutine(OpenCommandPrompt(prompt));

        //analyseText = true;
    }

    public void AI_Chat_End()
    {
        string user_Input = npc_UI_Manager.GetUserInput();

        string promptTitle = "Now this is your prompt:";
        string promptContent;
        string additionalPrompts = "Your previous response was: " + "~" + previousContext + "~";

        if (!string.IsNullOrEmpty(user_Input))
            promptContent = "Bid the player farewell after they say to you: " + user_Input;
        else
            promptContent = "Bid the player farewell.";

        string prompt = GetFinalPromptString(promptTitle, promptContent, additionalPrompts);
        StartCoroutine(OpenCommandPrompt(prompt));
    }

    /*
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
            "keep your responses less than 50 words, " +
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
            "keep your responses less than 50 words, " +
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

        //string AI_Output = OpenCommandPrompt(fullCommand_AIChat);

        //string Display_Output;

        //if (!string.IsNullOrEmpty(ExtractContent(AI_Output)))
        //{
        //    Display_Output = ExtractContent(AI_Output);
        //}
        //else
        //{
        //    Display_Output = AI_Output;
        //}

        //chatbox_Output.text = Display_Output + "\n\n " + AI_Exit_Text;
        //previousContext = Display_Output;

    }
    public void AI_Chat_Return()
    {
        //inConvo = false;
        chatbox_Output.text = "";
        AI_Chat_Canvas.SetActive(false);
        //playerController.enabled = true;
    }
    */

    public void AI_Chat_Exit()
    {
        introFinished = true;
        hasIntroduced = false;
        analyseText = false;
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

        /*
        AI_Chat_Process = new Process
        {
            StartInfo = startInfo
        };
        
        AI_Chat_Process.Start();
        UnityEngine.Debug.Log("Generating...");
        yield return StartCoroutine(ReadAIOutput(AI_Chat_Process.StandardOutput));

        yield return new WaitUntil(() => AI_Chat_Process.HasExited);
        AI_LoadingUI.SetActive(false);
        UnityEngine.Debug.Log("Done!");

        if (AI_Output_Builder.Length > 0)
        {
            string finalOutput = AI_Output_Builder.ToString();
            UnityEngine.Debug.Log("Final Output: " + finalOutput);
            chatbox_Output.text = ExtractContent(finalOutput);
            previousContext = ExtractContent(finalOutput);
        }

        AI_Chat_Process.Dispose();
        */

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
                previousContext = ExtractContent(AI_Output);
                AI_ChatUpdated = true;
                npc_UI_Manager.SetNPCOutput(previousContext);
                OnFinishGeneratingResponse?.Invoke();
            }
        }
        while (!AI_ChatUpdated);
        {
            yield return null;
        }

        if (analyseText)
        {
            AI_Sentiment_Analysis.SendPredictionText(ExtractContent(AI_Output));
            analyseText = false;
        }

    }
    /*
    private IEnumerator ReadAIOutput(StreamReader streamReader)
    {
        while (!streamReader.EndOfStream)
        {
            //string output = streamReader.ReadLine();
            string output = "";
            if (output != null)
            {
                AI_Output_Builder.Append(output + "\n");
                UnityEngine.Debug.Log("Receiving Data from AI: " + output);
            }

            // Yielding to the next frame to avoid freezing
            yield return null;
        }
    }
    
    IEnumerator UpdateChatboxOutput(string output)
    {
        chatbox_Output.text = output;
        previousContext = output;

        //UnityEngine.Debug.Log("Chatbox should now show Data: " + output);
        //UnityEngine.Debug.Log("Chatbox now showing: " + chatbox_Output.text);

        yield return null;
    }
    */
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
    /*
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
    */
}