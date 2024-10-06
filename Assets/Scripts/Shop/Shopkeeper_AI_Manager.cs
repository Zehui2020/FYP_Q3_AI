using System.Collections;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Events;

public class Shopkeeper_AI_Manager : MonoBehaviour
{
    [SerializeField] private ShopkeeperData shopkeeperData;
    [SerializeField] private ShopkeeperUIManager shopkeeperUIManager;
    [SerializeField] private bool isDebugging;

    [Header("Sentiment Analysis")]
    //Sentiment Analysis
    public TextAnalysis AI_Sentiment_Analysis;
    private string previousContext;

    private bool introFinished;
    private bool hasIntroduced;

    private bool analyseText;

    public UnityEvent OnFinishGenerating;

    public void InitAIManager()
    {
        introFinished = false;
        analyseText = false;
        hasIntroduced = false;
        AI_Sentiment_Analysis.OnAnalysisEnabled();

        AI_Chat_Introduction();
    }

    private string GetFinalPromptString(string promptTitle, string promptContent, string additionalPrompts)
    {
        if (isDebugging)
            return string.Empty;

        string AI_Gen_Prompt =
            '"' +
            "[INST] <<SYS>> You are the voice of a Shopkeeper in a video game. " +
            "This is the NPC's backstory:  " +
            "~" + shopkeeperData.AI_CharacterContext + "~ " +
            "In this environment, address the user as Adventurer, " +
            "keep your responses less than 50 words, " +
            "your responses should be purely dialogue, " +
            "do not depict actions, avoid writing content like *nods*, *walks over*, *leans in* " +
            "and do not show XML tags other than these ones: <result></result>" +

            "Here are a few examples of what your output should look like: " +
            "<result>" + shopkeeperData.AI_Example_Output_1 + "</result> " +
            "<result>" + shopkeeperData.AI_Example_Output_2 + "</result> " +

            //additionalPrompts+
            promptTitle + " <</SYS>> {" + promptContent + "} [/INST]" + '"';

        return $"cd {shopkeeperData.llamaDirectory} && llama-cli -m {shopkeeperData.modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";
    }

    public void AI_Chat_Introduction()
    {
        if (hasIntroduced || isDebugging)
            return;

        string prompt;

        if (!introFinished)
        {
            prompt = GetFinalPromptString("Here is your first prompt:", shopkeeperData.introductionPrompt, string.Empty);
            StartCoroutine(OpenCommandPrompt(prompt));
            introFinished = true;
        }
        else
        {
            prompt = GetFinalPromptString("Here is the input:",
            "The same player/customer came back for another conversation. You've already met them before, address them with familiarity.", string.Empty);
            StartCoroutine(OpenCommandPrompt(prompt));
        }

        hasIntroduced = true;
        analyseText = true;
    }

    public void AI_Chat_Response()
    {
        if (isDebugging)
            return;

        string user_Input = shopkeeperUIManager.GetUserInput();

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
    }

    public void AI_Chat_End()
    {
        if (isDebugging)
            return;

        string user_Input = shopkeeperUIManager.GetUserInput();

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

        // Ensure the final output is updated
        do
        {
            if (process.HasExited && AI_Output.Contains("</result>"))
            {
                //Bug: e.Data and AI_Output can contain AI_Text_Generation
                //     but the UpdateChatboxOutput(ExtractContent(AI_Output)) could end up running with a null string.
                previousContext = ExtractContent(AI_Output);
                AI_ChatUpdated = true;
                shopkeeperUIManager.SetShopkeeperOutput(previousContext);
                OnFinishGenerating?.Invoke();
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

    public void OnLeaveShop()
    {
        introFinished = false;
        analyseText = false;
        hasIntroduced = false;
    }
}