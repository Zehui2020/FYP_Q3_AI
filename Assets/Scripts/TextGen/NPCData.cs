using UnityEngine;

[CreateAssetMenu(fileName = "NPCData")]
public class NPCData : ScriptableObject
{
    [TextArea(3, 10)] public string llamaDirectory;
    [TextArea(3, 10)] public string modelDirectory;

    [TextArea(3, 10)] public string AI_Example_Output_1;
    [TextArea(3, 10)] public string AI_Example_Output_2;
    [TextArea(3, 10)] public string AI_CharacterContext;

    [TextArea(3, 10)] public string introductionPrompt;
}