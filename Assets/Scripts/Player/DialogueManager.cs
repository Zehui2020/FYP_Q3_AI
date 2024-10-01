using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    [System.Serializable]
    public struct Dialogue
    {
        public string speaker;
        public string dialogue;
        public List<string> playerChoices;
        public Transform questDestination;
    }

    [SerializeField] private TypewriterEffect npcDialogue;
    [SerializeField] private QuestPointer questPointer;

    public void ShowDialogue(Dialogue dialogue)
    {

    }
}