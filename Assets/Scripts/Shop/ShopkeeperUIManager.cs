using DesignPatterns.ObjectPool;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopkeeperUIManager : MonoBehaviour
{
    [SerializeField] private Animator shopUIAnimator;

    //For conversing with the Shopkeeper
    [SerializeField] private TMP_InputField user_Input;
    [SerializeField] private TypewriterEffect shopkeeperOutput;

    [Header("Coin Counter")]
    [SerializeField] private RectTransform coinCounterRectTransform;
    [SerializeField] private TextMeshProUGUI coinCounter;

    [Header("Shop Item")]
    [SerializeField] private Transform shopItemParent;
    [SerializeField] private RectTransform shopItemParentRect;

    [Header("Rerolling")]
    [SerializeField] private TextMeshProUGUI rerollCost;
    [SerializeField] private List<RectTransform> rerollRects;

    public bool isInteracting = false;
    private List<ShopItem> shopItems = new();
    private PlayerController player;

    public void InitUIManager()
    {
        player = PlayerController.Instance;
    }

    public void ShowUI()
    {
        shopUIAnimator.gameObject.SetActive(true);
        shopUIAnimator.SetTrigger("enter");
    }

    public void HideUI()
    {
        shopUIAnimator.SetTrigger("exit");
    }

    public void SetShopkeeperOutput(string output)
    {
        StartCoroutine(SetOutputRoutine(output));
    }

    private IEnumerator SetOutputRoutine(string output)
    {
        while (!isInteracting)
            yield return null;

        shopkeeperOutput.gameObject.SetActive(true);
        shopkeeperOutput.ShowMessage("Constance", output, 0);
    }

    public void OnLeaveShopkeeper()
    {
        shopkeeperOutput.gameObject.SetActive(false);
        user_Input.text = string.Empty;
        isInteracting = false;
    }

    public void SetCoinCounter()
    {
        coinCounter.text = player.gold.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(coinCounterRectTransform);
    }

    public void SpawnShopItem(ShopItemData shopItemData)
    {
        ShopItem shopItem = ObjectPool.Instance.GetPooledObject("ShopItem", false) as ShopItem;
        shopItem.transform.SetParent(shopItemParent);
        shopItem.gameObject.SetActive(true);
        shopItem.InitShopItem(player, shopItemData);
        LayoutRebuilder.ForceRebuildLayoutImmediate(shopItemParentRect);

        shopItems.Add(shopItem);
    }

    public void ApplyDiscount(float modifier)
    {
        foreach (ShopItem shopItem in shopItems)
            shopItem.SetDiscount(modifier);
    }

    public void OnReroll(int cost)
    {
        foreach (ShopItem item in shopItems)
            item.ReleaseItem();

        shopItems.Clear();
        SetRerollCost(cost);
    }

    public void SetRerollCost(int cost)
    {
        rerollCost.text = cost.ToString();
        foreach (RectTransform rect in rerollRects)
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
    }

    public string GetUserInput()
    {
        return user_Input.text;
    }

    private void Update()
    {
        SetCoinCounter();
    }
}