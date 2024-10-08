using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using static BaseAbility;

public class ItemPickupAlert : MonoBehaviour
{
    [SerializeField] private RectTransform itemPickupAlertCanvas;
    [SerializeField] private Animator animator;
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private TextMeshProUGUI description;
    [SerializeField] private PlayerPrefs playerPrefs;

    [SerializeField] private List<Item> itemsToDisplay = new();
    [SerializeField] private List<BaseAbility> abilitiesToDisplay = new();
    private Coroutine showItemRoutine;

    private Material outlineMaterial;

    private int lastShowedAlert;

    public void DisplayAlert(Item item)
    {
        itemsToDisplay.Add(item);
        ShowItem();
    }

    public void DisplayAlert(BaseAbility ability)
    {
        abilitiesToDisplay.Add(ability);
        ShowItem();
    }

    private void ShowItem()
    {
        if (showItemRoutine == null)
            showItemRoutine = StartCoroutine(ShowRoutine());
    }

    private IEnumerator ShowRoutine()
    {
        if (abilitiesToDisplay.Count > 0)
        {
            if (playerPrefs.detailedDescription)
                description.text = abilitiesToDisplay[0].description;
            else
                description.text = abilitiesToDisplay[0].simpleDescription;

            itemIcon.sprite = abilitiesToDisplay[0].spriteIcon;
            outlineMaterial = new Material(abilitiesToDisplay[0].itemOutlineMaterial);
            itemIcon.material = outlineMaterial;
            title.text = abilitiesToDisplay[0].title;
            lastShowedAlert = 1;
        }
        else if (itemsToDisplay.Count > 0)
        {
            if (playerPrefs.detailedDescription)
                description.text = itemsToDisplay[0].description;
            else
                description.text = itemsToDisplay[0].simpleDescription;

            itemIcon.sprite = itemsToDisplay[0].spriteIcon;
            outlineMaterial = new Material(itemsToDisplay[0].itemOutlineMaterial);
            itemIcon.material = outlineMaterial;
            title.text = itemsToDisplay[0].title;
            lastShowedAlert = 0;
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(itemPickupAlertCanvas);

        animator.SetTrigger("FadeIn");

        if (lastShowedAlert == 0)
            yield return new WaitForSeconds(itemsToDisplay[0].alertDuration);
        else
            yield return new WaitForSeconds(3);

        animator.SetTrigger("FadeOut");
    }

    public void RemoveItem()
    {
        if (lastShowedAlert == 0)
            itemsToDisplay.RemoveAt(0);
        else
            abilitiesToDisplay.RemoveAt(0);

        showItemRoutine = null;

        if (itemsToDisplay.Count > 0 || abilitiesToDisplay.Count > 0)
            ShowItem();
    }

    private void Update()
    {
        if (outlineMaterial == null)
            return;

        outlineMaterial.SetColor("_Color", itemIcon.color);
    }
}