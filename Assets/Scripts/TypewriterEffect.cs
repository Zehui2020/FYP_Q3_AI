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

    public UnityEvent OnFinishTyping;

    private Coroutine TypeRoutine;

    public void SetSpeakerName(string speaker)
    {
        if (speakerName == null)
            return;

        speakerName.text = speaker;
    }

    public void ShowMessage(string speaker, string message)
    {
        if (TypeRoutine != null)
            StopCoroutine(TypeRoutine);

        dialogueText.text = string.Empty;
        if (speakerName != null)
            speakerName.text = speaker;
        TypeRoutine = StartCoroutine(TypeWriterTMP(message));
    }

    private IEnumerator TypeWriterTMP(string message)
    {
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