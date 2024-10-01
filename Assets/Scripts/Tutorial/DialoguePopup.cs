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

    public void ShowDialoguePopup(DialogueManager.PopupDialogue dialogue)
    {
        currentDialogue = dialogue;
        SetCanvasEnabled(true);
        animator.SetTrigger("show");
        characterIcon.sprite = dialogue.speakerIcon;
        dialogueText.ShowMessage(dialogue.speakerName, dialogue.dialogue);
    }

    public void HidePopup()
    {
        StartCoroutine(HideRoutine());
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
        if (currentDialogue.questDestination != null)
            questPointer.Show(currentDialogue.questDestination);
        SetCanvasEnabled(false);
    }
}