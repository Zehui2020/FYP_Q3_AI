using DesignPatterns.ObjectPool;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopItem : PooledObject
{
    public ShopItemData shopItem;
    [SerializeField] private List<RectTransform> itemUIRects;
    [SerializeField] private TextMeshProUGUI itemCost;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Animator itemAnimator;
    public UnityEvent OnPurchaseEvent;

    private PlayerController player;
    private int cost;
    private bool isPurchased = false;

    public void InitShopItem(PlayerController player, ShopItemData shopItem)
    {
        this.shopItem = shopItem;
        cost = shopItem.shopCost;
        itemCost.text = cost.ToString();
        this.player = player;

        itemIcon.sprite = shopItem.spriteIcon;
        itemIcon.material = shopItem.itemOutlineMaterial;
        foreach (RectTransform rect in itemUIRects)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void PurchaseItem()
    {
        if (player.gold < cost || isPurchased)
            return;

        OnPurchaseEvent.Invoke();
        player.RemoveGold(cost);
        player.GiveShopItem(shopItem);
        itemAnimator.SetTrigger("purchase");
        isPurchased = true;
    }

    public void SetDiscount(float modifier)
    {
        cost = Mathf.CeilToInt(shopItem.shopCost * modifier);
        itemCost.text = "<s><color=grey>" + shopItem.shopCost.ToString() + "</s></color> " + cost.ToString();
        foreach (RectTransform rect in itemUIRects)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public void OnHover()
    {
        ItemTooltip.Instance.ShowTooltip(shopItem);
        itemAnimator.SetTrigger("hover");
    }

    public void OnHoverExit()
    {
        ItemTooltip.Instance.HideTooltip();
        itemAnimator.SetTrigger("hoverExit");
    }

    public void ReleaseItem()
    {
        isPurchased = false;
        cost = 0;
        shopItem = null;
        itemAnimator.Rebind();

        Release();
        gameObject.SetActive(false);
    }
}