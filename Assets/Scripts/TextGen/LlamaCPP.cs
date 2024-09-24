using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;

public class LlamaCPP : MonoBehaviour
{
    public TextMeshProUGUI cmd_Output_Desc;
    public TextMeshProUGUI cmd_Output_Stats;

    public TMP_InputField user_Input_Weapon;
    public TMP_InputField user_Input_Armor;

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
    
    public void Generate_Weapon_Desc()
    {
        if (!string.IsNullOrEmpty(user_Input_Weapon.text))
        {
            userPrompt = user_Input_Weapon.text;
            AI_Gen_Prompt = 
                '"' + 
                "[INST] <<SYS>> You are a writer and your primary job is to write concise descriptions for game weapons specifically. " +
                "In this environment, do not address the user and do not show XML tags other than these ones below: <result></result> " +
                
                "Here are a few examples of what your output should look like: " +
                "<result>This is a sacred sword from times of old, held by a warrior named Link. " +
                "With this sword, he has fell many dragons and the mighty Ganandolf.</result> " +

                "<result>A magical sword wielded by evil warriors known as the Sith. " +
                "It consists of a plasma blade, powered by a kyber crystal. " +
                "The sound of its hum in a silent room signals the beginning of the end of your life.</result> " +

                "In this environment, keep the description less than 50 words. " +
                /*
                "If you are asked for an object that is not a conventional, hand-held, medieval-era weapon, return this response:" +
                "<result>Please enter a prompt for a weapon.</result> " +

                "If you are asked for an object that is modern-themed, science-fiction-themed or futuristic-themed, return this response:" +
                "<result>Please enter a prompt for a weapon.</result> " +

                "If you are asked for a modern-themed weapon like a bomb, a type of gun like a Pistol, Revolver, Rifle, Shotgun, Launcher etc or a laser sword, return this response:" +
                "<result>Please enter a prompt for a weapon.</result> " +
                */
                "Here is a request to write a description for a game item: <</SYS>> {" + userPrompt + "} [/INST]" + '"';
            string fullCommand = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";
            cmd_Output_Desc.text = ExtractContent_ResultTag(OpenCommandPrompt(fullCommand));
        }
    }

    public void Generate_Weapon_Stats()
    {
        if (!string.IsNullOrEmpty(user_Input_Weapon.text))
        {
            userPrompt = user_Input_Weapon.text;
            AI_Gen_Prompt =
                '"' +
                "[INST] <<SYS>> You are a writer and your primary job is to generate statistics for weapons. " +
                "In this environment, do not address the user and do not show XML tags other than these ones: " +
                "<result></result>,  " +
                "<weaponType></weaponType>, " +
                "<result></result>" +

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

        UnityEngine.Debug.Log("Command Prompt Output: " + userPrompt);

        return output;
        //cmd_Output.text = ExtractContent(output);
    }

    string ExtractContent_ResultTag(string text)
    {
        string pattern = "<result>(.*?)</result>";
        Match match = Regex.Match(text, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty;
    }
}
