using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ItemPickupAlert : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private int defaultFontSize;

    private List<Item> itemsToDisplay = new List<Item>();

    public void DisplayAlert(Item item)
    {
        itemsToDisplay.Add(item);
        ShowItem();
    }

    private void ShowItem()
    {
        StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        itemIcon.sprite = itemsToDisplay[0].spriteIcon;
        title.text = itemsToDisplay[0].title;
        description.text = itemsToDisplay[0].description;

        // Check for overflow
        description.ForceMeshUpdate();
        if (description.isTextOverflowing)
            description.enableAutoSizing = true;

        animator.SetTrigger("show");

        yield return new WaitForSeconds(itemsToDisplay[0].alertDuration);

        animator.SetTrigger("hide");
    }

    public void RemoveItem()
    {
        itemsToDisplay.RemoveAt(0);
        description.enableAutoSizing = false;
        description.fontSize = defaultFontSize;

        if (itemsToDisplay.Count > 0)
            ShowItem();
    }
}