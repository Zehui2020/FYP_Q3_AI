using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class ItemPickupAlert : MonoBehaviour
{
    [SerializeField] private RectTransform itemPickupAlertCanvas;
    [SerializeField] private Animator animator;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;

    [SerializeField] private List<Item> itemsToDisplay = new List<Item>();
    private Coroutine showItemRoutine;

    private Material outlineMaterial;

    public void DisplayAlert(Item item)
    {
        itemsToDisplay.Add(item);
        ShowItem();
    }

    private void ShowItem()
    {
        if (showItemRoutine == null)
            showItemRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        itemIcon.sprite = itemsToDisplay[0].spriteIcon;
        outlineMaterial = new Material(itemsToDisplay[0].itemOutlineMaterial);
        itemIcon.material = outlineMaterial;
        title.text = itemsToDisplay[0].title;
        description.text = itemsToDisplay[0].description;

        LayoutRebuilder.ForceRebuildLayoutImmediate(itemPickupAlertCanvas);

        animator.SetTrigger("FadeIn");

        yield return new WaitForSeconds(itemsToDisplay[0].alertDuration);

        animator.SetTrigger("FadeOut");
    }

    public void RemoveItem()
    {
        itemsToDisplay.RemoveAt(0);
        showItemRoutine = null;

        if (itemsToDisplay.Count > 0)
            ShowItem();
    }

    private void Update()
    {
        if (outlineMaterial == null)
            return;

        outlineMaterial.SetColor("_Color", itemIcon.color);
    }
}