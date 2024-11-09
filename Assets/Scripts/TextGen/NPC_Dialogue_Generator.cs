using System.Collections;
using UnityEngine;
using System.Diagnostics;
using System.Text.RegularExpressions;
using UnityEngine.Events;
using Unity.VisualScripting.Antlr3.Runtime.Tree;

public class NPC_Dialogue_Generator : MonoBehaviour
{
    [Header("NPC Manager")]
    [SerializeField] private TutorialGuide npc;
    private NPCData NPC_Data;

    [Header("Sentiment Analysis")]
    public TextAnalysis AI_Sentiment_Analysis;

    private Process process;

    //User Input
    private string userPrompt;
    //AI Previous Conversation
    private string previousContext;

    //AI Chat Bools
    public bool isGenerating = false;

    private bool introFinished;
    private bool hasIntroduced;
    private bool analyseText;
    private bool EndTextFinished;

    public UnityEvent<string> OnFinishGeneratingResponse;

    public void InitAIManager(NPCData npc)
    {
        introFinished = false;
        analyseText = false;
        hasIntroduced = false;
        EndTextFinished = false;
        NPC_Data = npc;

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

    public void AI_Chat_Response(DialogueOptionData option)
    {
        DialogueManager.Dialogue dialogue = new();

        if (option.OptionTitle == "Goodbye.")
        {
            dialogue.speakerType = NPC_Data.speakerType;
            dialogue.speakerName = NPC_Data.npcName;
            dialogue.speakerIcon = NPC_Data.npcSprite;
            dialogue.dialogue = "Best of luck out there adventurer!";

            PlayerController.Instance.dialogueManager.ShowDialogue(dialogue, npc.minPitch, npc.maxPitch);

            npc.OnLeaveConvo();

            return;
        }

        dialogue.speakerType = NPC_Data.speakerType;
        dialogue.speakerName = NPC_Data.npcName;
        dialogue.speakerIcon = NPC_Data.npcSprite;
        dialogue.dialogue = "Generating...";

        PlayerController.Instance.dialogueManager.ShowDialogue(dialogue, npc.minPitch, npc.maxPitch);

        string promptTitle;
        string promptContent;
        string additionalPrompts = string.Empty;

        if (option != null)
        {
            promptTitle = "Here is the player's input:";
            promptContent = option.OptionTitle;
            additionalPrompts += ". For additional context: " + option.WorldContext + ". ";

            if (!string.IsNullOrEmpty(previousContext))
                additionalPrompts += "Your previous response was : " + "~" + previousContext + "~";
        }
        else
        {
            promptTitle = "Here is the player's input:";
            promptContent = "The player remains silent.";

            if (!string.IsNullOrEmpty(previousContext))
                additionalPrompts += "Your previous response was : " + "~" + previousContext + "~";
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
        isGenerating = true;

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

        UnityEngine.Debug.Log("Sent: \n" + command);

        while (!process.HasExited)
        {
            yield return null;
        }

        if (process.HasExited && AI_Output.Contains("</result>"))
        {
            isGenerating = false;
            previousContext = ExtractContent(AI_Output);
            if (!EndConvo)
                OnFinishGeneratingResponse?.Invoke(previousContext);
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