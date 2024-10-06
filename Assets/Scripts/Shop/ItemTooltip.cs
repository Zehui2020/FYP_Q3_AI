using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemTooltip : MonoBehaviour
{
    public static ItemTooltip Instance;

    [SerializeField] private PlayerPrefs playerPrefs;

    [Header("UI Elements")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemTitle;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private List<RectTransform> itemUIRects;
    [SerializeField] private Animator animator;
    private RectTransform tooltipRect;

    private Coroutine showRoutine;
    private bool isShowing = false;

    private void Awake()
    {
        Instance = this;
        tooltipRect = GetComponent<RectTransform>();
    }

    public void ShowTooltip(ShopItemData shopItem)
    {
        showRoutine = StartCoroutine(ShowRoutine(shopItem));
    }
    private IEnumerator ShowRoutine(ShopItemData shopItem)
    {
        yield return new WaitForSeconds(1f);
        Display(shopItem);
    }
    private void Display(ShopItemData shopItem)
    {
        itemIcon.sprite = shopItem.spriteIcon;
        itemIcon.material = shopItem.itemOutlineMaterial;
        itemTitle.text = shopItem.title;
        itemDescription.text = playerPrefs.detailedDescription ? shopItem.description : shopItem.simpleDescription;

        foreach (RectTransform itemUIRect in itemUIRects)
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemUIRect);

        isShowing = true;
        animator.SetBool("isShowing", isShowing);
    }

    public void HideTooltip()
    {
        if (showRoutine == null)
            return;

        StopCoroutine(showRoutine);
        showRoutine = null;
        isShowing = false;

        animator.SetBool("isShowing", isShowing);
    }

    private void Update()
    {
        if (!isShowing)
            return;

        Vector2 position = Input.mousePosition;

        float tooltipWidth = tooltipRect.rect.width;
        float tooltipHeight = tooltipRect.rect.height;

        float screenRightEdge = position.x + (tooltipWidth * (1 - tooltipRect.pivot.x));
        float screenLeftEdge = position.x - (tooltipWidth * tooltipRect.pivot.x);

        if (screenRightEdge > Screen.width)
            position.x = Screen.width - (tooltipWidth * (1 - tooltipRect.pivot.x));
        else if (screenLeftEdge < 0)
            position.x = tooltipWidth * tooltipRect.pivot.x;

        transform.position = position;
    }
}