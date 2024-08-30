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

    [SerializeField] private List<Item> itemsToDisplay = new List<Item>();

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

        animator.SetTrigger("FadeIn");

        yield return new WaitForSeconds(itemsToDisplay[0].alertDuration);

        animator.SetTrigger("FadeOut");
    }

    public void RemoveItem()
    {
        itemsToDisplay.RemoveAt(0);

        if (itemsToDisplay.Count > 0)
            ShowItem();
    }
}