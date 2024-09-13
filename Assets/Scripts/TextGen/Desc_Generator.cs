using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TMPro;
using System.Xml.Linq;

public class Desc_Generator : MonoBehaviour
{
    public TextMeshProUGUI cmd_Output_Desc;
    public TextMeshProUGUI cmd_Output_Stats;

    public TMP_InputField user_Input_Weapon;
    public TMP_InputField user_Input_Armor;

    private string userPrompt;

    private string llamaDirectory = @"C:\llama.cpp";
    private string modelDirectory = @"C:\llama.cpp\models\Trail_3\llama-2-7b-chat.Q4_K_M.gguf";
    private string AI_Gen_Prompt;

    public List<List<string>> WeaponStats = new List<List<string>>();

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
            string fullCommand_WeaponDesc = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";
            cmd_Output_Desc.text = ExtractContent(OpenCommandPrompt(fullCommand_WeaponDesc), "result");
            //Generate_Weapon_Stats();
        }
    }
    /*
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
                "<baseAtk></baseAtk>, " +
                "<rndEffect></rndEffect>, " +
                "<critRate></critRate>, " +
                "<critDmg></critDmg>. " +

                "Here is an example of what your output should look like:" +
                "<result>" +
                "Weapon Type:  <weaponType>Single word classification</weaponType> " +
                "Base Attack:  <baseAtk>Any integer from 5 to 20</baseAtk> " +
                "Random Effect:  <rndEffect>Any integer from 1 to 10</rndEffect> " +
                "Critical Hit Rate:  <critRate>Any integer from 5 to 25</critRate> " +
                "Critical Hit Damage:  <critDmg>Any float from 1.2 to 2.0</critDmg> " +
                "</result> " +

                "Here is a request to write a set of statistics for a game weapon: <</SYS>> {" + userPrompt + "} [/INST]" + '"';
            string fullCommand_WeaponStats = $"cd {llamaDirectory} && llama-cli -m {modelDirectory} --no-display-prompt -p {AI_Gen_Prompt}";
            string WeaponStats_Output = OpenCommandPrompt(fullCommand_WeaponStats);

            string WeaponStats_WeaponType = ExtractContent(WeaponStats_Output, "weaponType");
            string WeaponStats_BaseAtk = ExtractContent(WeaponStats_Output, "baseAtk");
            string WeaponStats_RndEffect = ExtractContent(WeaponStats_Output, "rndEffect");
            string WeaponStats_CritRate = ExtractContent(WeaponStats_Output, "critRate");
            string WeaponStats_CritDmg = ExtractContent(WeaponStats_Output, "critDmg");
            WeaponStats.Add(new List<string>());
            WeaponStats[0].Add(WeaponStats_WeaponType);
            WeaponStats[0].Add(WeaponStats_BaseAtk);
            WeaponStats[0].Add(WeaponStats_RndEffect);
            WeaponStats[0].Add(WeaponStats_CritRate);
            WeaponStats[0].Add(WeaponStats_CritDmg);

            cmd_Output_Stats.text =
                WeaponStats_WeaponType + "\n" +
                WeaponStats_BaseAtk + "\n" +
                WeaponStats_RndEffect + "\n" +
                WeaponStats_CritRate + "\n" +
                WeaponStats_CritDmg;
            UnityEngine.Debug.Log(WeaponStats[0].Count);
            //cmd_Output_Stats.text = ExtractContent(WeaponStats_Output, "weaponType");
        }
    }
    */
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

    string ExtractContent(string text, string tag)
    {
        /*
        var doc = XDocument.Parse(xml);
        var elements = doc.Descendants(tag);

        List<string> results = new List<string>();
        foreach (var element in elements)
        {
            results.Add(element.Value.Trim());
        }

        return string.Join("\n", results);
        */

        string pattern = "<" + tag + ">(.*?)</" + tag + ">";
        Match match = Regex.Match(text, pattern);

        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty;

    }
}
