using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DialogueTree")]
public class NPC_Dialogue_Tree : ScriptableObject
{
    public int restLevel;
    public NPCData npcData;
    public List<DialogueOptionData> dialogueOptionDatas = new();
}