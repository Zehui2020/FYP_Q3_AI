using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;

public class LlamaCPP : MonoBehaviour
{
    public TextMeshProUGUI cmd_Output;
    public TMP_InputField user_Input;

    private string userPrompt;

    private string llamaDirectory = @"C:\llama.cpp";
    private string modelDirectory = @"C:\llama.cpp\models\Trail_3\llama-2-7b-chat.Q4_K_M.gguf";
    private string AI_Gen_Prompt;

    void Start()
    {

        //string AI_Gen_Prompt = '"' + "[INST] <<SYS>> You are a writer and your primary job is to write concise descriptions for game items. In this environment, do not address the user and do not show XML tags other than these ones below: <result></result> Here are a few examples of what your output should look like: <result>This is a sacred sword from times of old, held by a warrior named Link. With this sword, he has fell many dragons and the mighty Ganandolf.</result> <result>A magical sword wielded by evil warriors known as the Sith. It consists of a plasma blade, powered by a kyber crystal. The sound of its hum in a silent room signals the beginning of the end of your life.</result> Here is a request to write a description for a game item: <</SYS>> {Give me a fire sword.} [/INST]" + '"';

        //string command = $"cd {llamaDirectory} && llama-cli --model {modelDirectory}";
        //string command = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";

        //OpenCommandPrompt(command);
    }
    
    public void ReadUserPrompt()
    {
        if (!string.IsNullOrEmpty(user_Input.text))
        {
            userPrompt = user_Input.text;
            AI_Gen_Prompt = 
                '"' + 
                "[INST] <<SYS>> You are a writer and your primary job is to write concise descriptions for game items. " +
                "In this environment, do not address the user and do not show XML tags other than these ones below: <result></result> " +
                
                "Here are a few examples of what your output should look like: " +
                "<result>This is a sacred sword from times of old, held by a warrior named Link. " +
                "With this sword, he has fell many dragons and the mighty Ganandolf.</result> " +

                "<result>A magical sword wielded by evil warriors known as the Sith. " +
                "It consists of a plasma blade, powered by a kyber crystal. " +
                "The sound of its hum in a silent room signals the beginning of the end of your life.</result> " +
                "Here is a request to write a description for a game item: <</SYS>> {" + userPrompt + "} [/INST]" + '"';
            string fullCommand = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";
            OpenCommandPrompt(fullCommand);
        }
    }

    void OpenCommandPrompt(string command)
    {
        // Start Command Prompt and execute a command
        ProcessStartInfo startInfo = new ProcessStartInfo("cmd.exe", $"/c {command}");
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.CreateNoWindow = true;

        Process process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        cmd_Output.text = ExtractContent(output);
        //UnityEngine.Debug.Log("Command Prompt Output: " + output);
    }

    string ExtractContent(string text)
    {
        // Regular expression to match content between <result> tags
        string pattern = "<result>(.*?)</result>";
        Match match = Regex.Match(text, pattern);

        // Check if the match is successful and return the content
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty; // Return empty if no match is found
    }
}
