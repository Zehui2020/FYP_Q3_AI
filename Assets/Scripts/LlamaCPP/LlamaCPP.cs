using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using TMPro;

public class LlamaCPP : MonoBehaviour
{
    public TextMeshProUGUI cmd_Output;

    void Start()
    {
        string llamaDirectory = @"C:\Users\rtzk2\llama.cpp";
        string modelDirectory = @"C:\Users\rtzk2\llama.cpp\models\Trail_3\llama-2-7b-chat.Q4_K_M.gguf";

        string AI_Gen_Prompt = "[INST] <<SYS>> You are a writer and your primary job is to write concise descriptions for game items. In this environment, do not address the user and do not show XML tags other than these ones below: <result></result> Here are a few examples of what your output should look like: <result>This is a sacred sword from times of old, held by a warrior named Link. With this sword, he has fell many dragons and the mighty Ganandolf.</result> <result>A magical sword wielded by evil warriors known as the Sith. It consists of a plasma blade, powered by a kyber crystal. The sound of its hum in a silent room signals the beginning of the end of your life.</result> Here is a request to write a description for a game item: <</SYS>> {Give me a fire sword.} [/INST]";

        string accessModel = $"llama-cli -m 'C:\\Users\\rtzk2\\llama.cpp\\models\\Trail_3 (TheBloke)\\llama-2-7b-chat.Q4_K_M.gguf' -if";

        //string command = $"cd {llamaDirectory} && llama-cli --model {modelDirectory}";
        string command = $"cd {llamaDirectory} && llama-cli -if -m {modelDirectory}";

        OpenCommandPrompt(command);
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

        cmd_Output.text = output;
        //UnityEngine.Debug.Log("Command Prompt Output: " + output);
    }
}
