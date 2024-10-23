using UnityEngine;

[CreateAssetMenu(fileName = "DialogueOptionData")]
public class DialogueOptionData : ScriptableObject
{
    [TextArea(3, 10)] public string OptionTitle;
    [TextArea(3, 10)] public string AI_Prompt;
}