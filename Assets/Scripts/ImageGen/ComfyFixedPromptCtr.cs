using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ComfyFixedPromptCtr;

public class ComfyFixedPromptCtr : MonoBehaviour
{
    [System.Serializable]
    public struct Prompt
    {
        [TextArea(3, 10)] public string Pprompt;
        public string fileName;
    }

    [SerializeField] private List<Prompt> prompts = new();
    [SerializeField] private List<string> filenames = new();
    int fileNameCounter = 0;

    [SerializeField] private ComfyPromptCtr promptCtr;

    private void Start()
    {
        fileNameCounter = 0;
        QueuePrompt();

        DontDestroyOnLoad(gameObject);
    }

    public void QueuePrompt()
    {
        if (fileNameCounter > prompts.Count - 1)
            return;

        promptCtr.QueuePrompt(prompts[fileNameCounter].Pprompt);
        filenames.Add(prompts[fileNameCounter].fileName);
    }

    public string GetFileName()
    {
        string name = filenames[fileNameCounter];
        fileNameCounter++;
        return name;
    }
}