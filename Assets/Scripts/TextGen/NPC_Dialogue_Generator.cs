using System.Collections;
using UnityEngine;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine.Events;

public class NPC_Dialogue_Generator : MonoBehaviour
{
    [Header("NPC Manager")]
    [SerializeField] private TutorialGuide npc;

    [Header("NPC Scriptable Object")]
    [SerializeField] private NPCData NPC_Data;

    [Header("Sentiment Analysis")]
    public TextAnalysis AI_Sentiment_Analysis;

    private Process process;

    //User Input
    private string userPrompt;
    //AI Previous Conversation
    private string previousContext;

    //AI Chat Bools
    private bool introFinished;
    private bool hasIntroduced;
    private bool analyseText;

    private bool EndTextFinished;

    public UnityEvent OnStartGeneratingResponse;
    public UnityEvent OnFinishGeneratingResponse;

    public void InitAIManager()
    {
        introFinished = false;
        analyseText = false;
        hasIntroduced = false;
        EndTextFinished = false;

        AI_Chat_Introduction();
    }

    public void EnterNPCDialogue()
    {
        if (introFinished && !hasIntroduced)
        {
            AI_Chat_Introduction();
        }
    }
    private string GetFinalPromptString(string promptTitle, string promptContent, string additionalPrompts)
    {

        string AI_Gen_Prompt =
            '"' +
            "[INST] <<SYS>> You are the voice of a character. " +
            "This is your character's backstory:  " +
            "~" + NPC_Data.AI_CharacterContext + "~ " +
            "In this environment, address the user as Adventurer, " +
            "keep your responses less than 30 words, " +
            "and do not show XML tags other than these ones: <result></result>" +

            "Here are a few examples of what your output should look like: " +
            "<result>" + NPC_Data.AI_Example_Output_1 + "</result> " +
            "<result>" + NPC_Data.AI_Example_Output_2 + "</result> " +
            //additionalPrompts+
            promptTitle + " <</SYS>> {" + promptContent + "} [/INST]" + '"';

        return $"cd {NPC_Data.llamaDirectory} && llama-cli -m {NPC_Data.modelDirectory} --no-display-prompt -p {AI_Gen_Prompt} -ngl 20000000 -t 5";
    }

    public void AI_Chat_Introduction()
    {
        if (hasIntroduced)
            return;

        string prompt;

        if (!introFinished)
        {
            prompt = GetFinalPromptString("Here is your first prompt:", NPC_Data.introductionPrompt, string.Empty);
            StartCoroutine(OpenCommandPrompt(prompt, false));
            introFinished = true;
        }
        else
        {
            prompt = GetFinalPromptString("Here is the input:",
            "The same adventurer/player came back for another conversation. You've already met them before, address them with familiarity.", string.Empty);
            StartCoroutine(OpenCommandPrompt(prompt, false));
        }

        hasIntroduced = true;
    }
    public void AI_Dialogue_Tree_Response(string AdditionalContext, string premadePrompt)
    {
        string AI_Gen_Prompt =
            '"' +
            "[INST] <<SYS>> You are the voice of a character. " +
            "This is your character's backstory:  " +
            "~" + NPC_Data.AI_CharacterContext + "~ " +
            "This is additional world lore:  " +
            "~" + AdditionalContext + "~ " +
            "In this environment, address the user as Adventurer, " +
            "keep your responses less than 30 words, " +
            "and do not show XML tags other than these ones: <result></result>" +

            "Here are a few examples of what your output should look like: " +
            "<result>" + NPC_Data.AI_Example_Output_1 + "</result> " +
            "<result>" + NPC_Data.AI_Example_Output_2 + "</result> " +
            "Here is the player's input" + " <</SYS>> {" + premadePrompt + "} [/INST]" + '"';

        string finalprompt = $"cd {NPC_Data.llamaDirectory} && llama-cli -m {NPC_Data.modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

        StartCoroutine(OpenCommandPrompt(finalprompt, false));
    }

    public void AI_Chat_Response(string user_Input)
    {
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
        StartCoroutine(OpenCommandPrompt(prompt, false));
    }

    public void AI_Chat_End(string user_Input)
    {
        string promptTitle = "Now this is your prompt:";
        string promptContent;
        string additionalPrompts = "Your previous response was: " + "~" + previousContext + "~";

        if (!string.IsNullOrEmpty(user_Input))
            promptContent = "Bid the player farewell after they say to you: " + user_Input;
        else
            promptContent = "Bid the player farewell.";

        string prompt = GetFinalPromptString(promptTitle, promptContent, additionalPrompts);
        StartCoroutine(OpenCommandPrompt(prompt, true));
    }

    public void AI_Chat_Exit()
    {
        introFinished = true;
        hasIntroduced = false;
        analyseText = false;
        EndTextFinished = false;
    }

    IEnumerator OpenCommandPrompt(string command, bool EndConvo)
    {
        string AI_Output = "";

        ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", $"/k {command}")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        process = new Process
        {
            StartInfo = startInfo,
        };

        process.OutputDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                AI_Output += e.Data + "\n";
        };

        string speedString = string.Empty;
        process.ErrorDataReceived += (sender, e) =>
        {
            if (e.Data != null)
                speedString += e.Data.ToString() + "\n";
        };

        process.Start();
        process.BeginErrorReadLine();
        process.BeginOutputReadLine();

        UnityEngine.Debug.Log("Sent!");

        while (!process.HasExited)
        {
            yield return null;
        }

        if (process.HasExited && AI_Output.Contains("</result>"))
        {
            previousContext = ExtractContent(AI_Output);
            if (!EndConvo)
                OnFinishGeneratingResponse?.Invoke();
        }

        if (analyseText)
        {
            AI_Sentiment_Analysis.SendPredictionText(ExtractContent(AI_Output));
            analyseText = false;
        }

        UnityEngine.Debug.Log("Result: " + AI_Output);
        UnityEngine.Debug.Log(speedString);

        if (process != null)
        {
            process.Kill();
            process = null;
        }

        if (analyseText)
        {
            AI_Sentiment_Analysis.SendPredictionText(ExtractContent(AI_Output));
            analyseText = false;
        }

        if (EndConvo)
        {
            yield return new WaitUntil(() => EndTextFinished);
            yield return new WaitForSeconds(5);
            npc.OnLeaveConvo();
            OnFinishGeneratingResponse?.Invoke();
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
}