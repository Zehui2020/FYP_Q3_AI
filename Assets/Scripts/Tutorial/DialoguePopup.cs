using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialoguePopup : MonoBehaviour
{
    [SerializeField] private Canvas popupCanvas;
    [SerializeField] private Animator animator;
    [SerializeField] private Image characterIcon;
    [SerializeField] private TypewriterEffect dialogueText;
    [SerializeField] private QuestPointer questPointer;

    private DialogueManager.PopupDialogue currentDialogue;

    private Coroutine HideCoroutine;

    public void ShowDialoguePopup(DialogueManager.PopupDialogue? dialogue)
    {
        if (!dialogue.HasValue)
            return;

        if (HideCoroutine != null)
        {
            StopCoroutine(HideCoroutine);
            animator.ResetTrigger("hide");
            SetCanvasEnabled(true);
            HideCoroutine = null;
        }

        currentDialogue = dialogue.Value;
        SetCanvasEnabled(true);
        animator.SetTrigger("show");
        characterIcon.sprite = currentDialogue.speakerIcon;
        dialogueText.ShowMessage(currentDialogue.speakerName, currentDialogue.dialogue);
        currentDialogue.onDialogueDone.InvokeEvent();

        if (currentDialogue.questDestination != null)
            questPointer.Show(currentDialogue.questDestination);
        else
            questPointer.Hide();
    }

    public void HidePopup()
    {
        HideCoroutine = StartCoroutine(HideRoutine());
    }

    public void HidePopupImmediately()
    {
        if (!popupCanvas.enabled)
            return;

        if (HideCoroutine != null)
        {
            StopCoroutine(HideCoroutine);
            HideCoroutine = null;
        }

        animator.ResetTrigger("hide");
        SetCanvasEnabled(false);
    }

    public void SetCanvasEnabled(bool enable)
    {
        popupCanvas.enabled = enable;
    }

    private IEnumerator HideRoutine()
    {
        yield return new WaitForSeconds(2f);
        animator.SetTrigger("hide");
        yield return new WaitForSeconds(0.3f);
        SetCanvasEnabled(false);
        HideCoroutine = null;
    }
}