using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour
{
    [SerializeField] string leadingChar = "";

    [SerializeField] float timeBtwChars = 0.5f;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI speakerName;
    [SerializeField] private float minPitch = 0.6f;
    [SerializeField] private float maxPitch = 0.8f;

    public UnityEvent OnFinishTyping;
    private Coroutine TypeRoutine;
    private string messageToDisplay;

    public void SetSpeakerName(string speaker, float min, float max)
    {
        if (speakerName == null)
            return;

        speakerName.text = speaker;
        minPitch = min;
        maxPitch = max;
    }

    public void ShowMessage(string speaker, string message, float lingerDuration)
    {
        if (TypeRoutine != null)
            StopCoroutine(TypeRoutine);

        dialogueText.text = string.Empty;
        if (speakerName != null)
            speakerName.text = speaker;
        TypeRoutine = StartCoroutine(TypeWriterTMP(message, lingerDuration));
    }

    public void ShowMessage(string speaker, string message, float min, float max, float lingerDuration)
    {
        if (TypeRoutine != null)
            StopCoroutine(TypeRoutine);

        dialogueText.text = string.Empty;
        if (speakerName != null)
            speakerName.text = speaker;
        minPitch = min;
        maxPitch = max;
        TypeRoutine = StartCoroutine(TypeWriterTMP(message, lingerDuration));
    }

    public void Skip()
    {
        if (TypeRoutine != null)
            StopCoroutine(TypeRoutine);

        TypeRoutine = null;
        OnFinishTyping?.Invoke();
        dialogueText.text = messageToDisplay;
    }

    private IEnumerator TypeWriterTMP(string message, float lingerDuration)
    {
        messageToDisplay = message;

        for (int i = 0; i < message.Length; i++)
        {
            if (message[i] == '<')
            {
                string richTextTag = GetCompleteRichTextTag(ref i, message);
                dialogueText.text += richTextTag;
            }
            else
            {
                dialogueText.text += message[i];
            }

            AudioManager.Instance.RandomiseAudioPitch(Sound.SoundName.Dialog, minPitch, maxPitch);
            AudioManager.Instance.PlayOneShot(Sound.SoundName.Dialog);
            if (leadingChar != "")
            {
                dialogueText.text += leadingChar;
                yield return new WaitForSeconds(timeBtwChars);
                dialogueText.text = dialogueText.text.Substring(0, dialogueText.text.Length - leadingChar.Length);
            }
            else
            {
                yield return new WaitForSeconds(timeBtwChars);
            }
        }

        yield return new WaitForSeconds(lingerDuration);

        messageToDisplay = string.Empty;
        OnFinishTyping?.Invoke();
    }

    private string GetCompleteRichTextTag(ref int index, string message)
    {
        string completeTag = string.Empty;

        while (index < message.Length)
        {
            completeTag += message[index];

            if (message[index] == '>')
                return completeTag;

            index++;
        }

        return string.Empty;
    }
}