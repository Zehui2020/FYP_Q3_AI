using System.Collections.Generic;
using UnityEngine;

public class ComfyAudioManager : ComfyManager
{
    [System.Serializable]
    public struct AudioPrompt
    {
        [TextArea(3, 10)] public string Pprompt;
        public string filename;
    }

    public List<AudioPrompt> audioPrompts = new();
    private int audioPromptIndex = 0;
    private bool finishedGenerating = false;

    [SerializeField] private bool queueOnStart;
    [SerializeField] private ComfyAudioCtr comfyAudioCtr;

    private void Start()
    {
        InitManager();
        comfyAudioCtr.OnRecieveAudio.AddListener(OnRecieveAudio);

        if (queueOnStart)
            QueueAudio();
    }

    public void QueueAudio()
    {
        if (audioPromptIndex >= audioPrompts.Count && !finishedGenerating)
        {
            finishedGenerating = true;
            Destroy(gameObject);
            return;
        }

        promptCtr.QueuePrompt(audioPrompts[audioPromptIndex].Pprompt);
        fileName = audioPrompts[audioPromptIndex].filename;
        GameData.Instance.EnqueueLoading(fileName + ".wav", false);
    }

    public void OnRecieveAudio(string promptID, AudioClip audio)
    {
        if (promptID.Equals(this.promptID))
        {
            SavWav.Save(fileName, audio);
            Debug.Log("Obtained: " + fileName);

            if (audioPromptIndex < audioPrompts.Count)
            {
                audioPromptIndex++;
                QueueAudio();
            }
        }
    }
}