using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : MonoBehaviour, IInteractable
{
    private ShopkeeperUIManager uiManager;
    private Shopkeeper_AI_Manager aiManager;
    private TextAnalysis textAnalysis;

    [SerializeField] private List<ShopItemData> allShopItems;
    [SerializeField] private float shopItemAmount;

    [SerializeField] private SimpleAnimation keycodeE;
    [SerializeField] private bool isDebugging;

    private PlayerController player;

    [SerializeField] private int rerollCost;
    [SerializeField] private int incrementRerollCost;
    [SerializeField] private float incrementModifier;

    private void Awake()
    {
        uiManager = GetComponent<ShopkeeperUIManager>();
        aiManager = GetComponent<Shopkeeper_AI_Manager>();
        textAnalysis = GetComponent<TextAnalysis>();

        textAnalysis.OnMoodChanged += OnMoodChanged;
    }

    private IEnumerator Start()
    {
        if (!isDebugging)
            aiManager.InitAIManager();

        player = PlayerController.Instance;
        uiManager.InitUIManager();

        yield return new WaitForSeconds(1f);

        SetupShop();
    }

    public void OnEnterRange()
    {
        keycodeE.Show();
    }

    public bool OnInteract()
    {
        uiManager.ShowUI();
        uiManager.isInteracting = true;
        uiManager.SetRerollCost(rerollCost);
        player.ChangeState(PlayerController.PlayerStates.Shop);
        return true;
    }

    public void OnLeaveRange()
    {
        keycodeE.Hide();
    }

    public void OnLeaveShopkeeper()
    {
        aiManager.OnLeaveShop();
        uiManager.OnLeaveShopkeeper();
        player.ChangeState(PlayerController.PlayerStates.Movement);
    }

    public void Reroll()
    {
        if (player.gold < rerollCost)
            return;

        player.gold -= rerollCost;
        rerollCost += incrementRerollCost;
        incrementRerollCost = Mathf.CeilToInt(incrementRerollCost * incrementModifier);
        uiManager.OnReroll(rerollCost);
        SetupShop();
    }

    public void SetupShop()
    {
        int items = Mathf.FloorToInt(shopItemAmount / 2f);

        for (int i = 0; i < items; i++)
        {
            ShopItemData shopItem = GetRandomShopItemOfType(ShopItemData.ShopItemType.Item);
            uiManager.SpawnShopItem(shopItem);
        }

        for (int i = 0; i < shopItemAmount - items; i++)
        {
            ShopItemData shopItem = GetRandomShopItemOfType(ShopItemData.ShopItemType.Ability);
            uiManager.SpawnShopItem(shopItem);
        }
    }

    public ShopItemData GetRandomShopItemOfType(ShopItemData.ShopItemType type)
    {
        List<ShopItemData> shopItems = new();

        foreach (ShopItemData item in allShopItems)
        {
            if (item.shopItemType.Equals(type) && !shopItems.Contains(item))
                shopItems.Add(item);
        }

        int randNum = Random.Range(0, shopItems.Count);
        return shopItems[randNum];
    }

    public void OnMoodChanged(float modifier)
    {
        uiManager.ApplyDiscount(modifier);
    }
}