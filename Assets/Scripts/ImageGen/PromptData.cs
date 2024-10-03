using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PromptData")]
public class PromptData : ScriptableObject
{
    // For world space button
    public List<string> themePrompts = new();

    // For BG generation
    public enum BGType
    {
        Parallax,
        Static
    }

    [System.Serializable]
    public struct BGPrompt
    {
        public enum Type
        { 
            Foreground,
            Middleground,
            Background,
            TotalTypes
        }

        public Type type;
        public BGType bgType;

        public string keywords;

        [TextArea(3, 100)]
        public string prompt;

        public string referenceImage;
    }
    public List<BGPrompt> foregroundBGPrompts = new();
    public List<BGPrompt> middleGroundBGPrompts = new();
    public List<BGPrompt> backgroundBGPrompts = new();

    // For tileset generation
    [System.Serializable]
    public struct PromptChecker
    {
        public string foundPrompts;
        public string endPrompt;
        public string controlnetImage;
    }
    public List<PromptChecker> promptCheckers = new();

    [TextArea(3, 100)]
    [SerializeField] private string parallaxBGJSON;
    [TextArea(3, 100)]
    [SerializeField] private string staticBGJSON;

    public List<string> GetButtonPromptList()
    {
        return themePrompts;
    }

    public BGPrompt GetBGPrompt(BGPrompt.Type type, string keywords)
    {
        switch (type)
        {
            case BGPrompt.Type.Foreground:
                return GetBGPromptsWithKeywords(foregroundBGPrompts, keywords);
            case BGPrompt.Type.Middleground:
                return GetBGPromptsWithKeywords(middleGroundBGPrompts, keywords);
            case BGPrompt.Type.Background:
                return GetBGPromptsWithKeywords(backgroundBGPrompts, keywords);
        }

        return new();
    }

    private BGPrompt GetBGPromptsWithKeywords(List<BGPrompt> list, string keywords)
    {
        List<BGPrompt> finalList = new();
        string[] keywordArray = keywords.Split(", "); 

        foreach (BGPrompt prompt in list)
        {
            foreach (string keyword in keywordArray)
            {
                if (!prompt.keywords.Contains(keyword))
                    continue;

                finalList.Add(prompt);
                break;
            }
        }

        if (finalList.Count == 0)
            return new();

        int randNum = Random.Range(0, finalList.Count);
        return finalList[randNum];
    }

    public string GetPromptJSON(BGType bgType)
    {
        switch (bgType)
        {
            case BGType.Parallax:
                return parallaxBGJSON;
            case BGType.Static:
                return staticBGJSON;
        }

        return string.Empty;
    }
}