using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public List<Item> itemList;
    public List<Item> allItems;
    public List<BaseAbility> allAbilities;
    [SerializeField] private ItemPickupAlert itemPickupAlert;

    [SerializeField] private ItemStats itemStats;
    [SerializeField] private ItemUI itemUIPrefab;
    [SerializeField] private Transform itemUIParent;
    [SerializeField] private RectTransform itemInventory;
    private List<ItemUI> itemUIs = new List<ItemUI>();

    private ImageSaver imageSaver;
    private GameData gameData;

    private void Awake()
    {
        Instance = this;

        #if UNITY_EDITOR
        EditorApplication.playModeStateChanged += (mode) => 
        {
            if (mode == PlayModeStateChange.ExitingPlayMode)
            {
                itemStats.ResetStats();
                ResetItemStacks();
            };
        };
        #endif
    }

    public void ResetItemStacks()
    {
        foreach (Item item in allItems)
            item.SetCount(0);
    }

    public void InitItemManager()
    {
        gameData = GameData.Instance;
        itemList = new();
        imageSaver = GetComponent<ImageSaver>();

        foreach (Item item in allItems)
            item.spriteIcon = imageSaver.GetSpriteFromLocalDisk(item.itemType.ToString());

        foreach (BaseAbility ability in allAbilities)
            ability.spriteIcon = imageSaver.GetSpriteFromLocalDisk(ability.abilityName.ToString());

        // Setup items & abilties from previous level
        foreach (Item item in gameData.items)
        {
            ItemUI newItemUI = Instantiate(itemUIPrefab, itemUIParent);
            newItemUI.SetupItemUI(item);
            itemUIs.Add(newItemUI);
            LayoutRebuilder.ForceRebuildLayoutImmediate(itemInventory);
        }
        itemList = gameData.items;

        LayoutRebuilder.ForceRebuildLayoutImmediate(itemInventory);
    }

    public void AddItem(Item itemToAdd)
    {
        if (FindItemInList(itemToAdd) == null)
        {
            itemToAdd.Initialize();
            itemList.Add(itemToAdd);
        }
        else
        {
            itemToAdd.IncrementStack();
        }

        OnPickupItem(itemToAdd);
    }

    public void AddAbility(BaseAbility ability)
    {
        PlayerController.Instance.abilityController.HandleAbilityPickUp(ability, ability.abilityCharges);
    }

    public void OnPickupItem(Item item)
    {
        itemPickupAlert.DisplayAlert(item);

        foreach (ItemUI itemUI in itemUIs)
        {
            if (itemUI.item.itemType == item.itemType)
            {
                itemUI.AddItemCount();
                return;
            }
        }

        ItemUI newItemUI = Instantiate(itemUIPrefab, itemUIParent);
        newItemUI.SetupItemUI(item);
        itemUIs.Add(newItemUI);
        LayoutRebuilder.ForceRebuildLayoutImmediate(itemInventory);
    }

    public void DecreaseStack(Item itemToRemove)
    {
        if (FindItemInList(itemToRemove) != null)
        {
            itemToRemove.DecrementStack();
            CheckItemStack(itemToRemove);
        }
    }

    public Item FindItemByName(Item.ItemType type)
    {
        foreach (Item item in itemList)
        {
            if (item.itemType == type)
                return item;
        }

        return null;
    }

    private Item FindItemInList(Item itemToFind)
    {
        foreach (Item item in itemList)
        {
            if (item.itemType == itemToFind.itemType)
                return item;
        }

        return null;
    }

    private void CheckItemStack(Item itemToCheck)
    {
        if (itemToCheck.itemStack <= 0)
            itemList.Remove(itemToCheck);
    }

    public List<Item> GetItemsFrom(Item.ItemCatagory itemCatagory, Item.Rarity rarity)
    {
        List<Item> items = new();

        foreach (Item item in allItems)
        {
            if (item.itemCatagory.Equals(itemCatagory) && item.itemRarity.Equals(rarity))
                items.Add(item);
        }

        return items;
    }

    public List<Item> GetItemsFrom(Item.Rarity rarity)
    {
        List<Item> items = new();

        foreach (Item item in allItems)
        {
            if (item.itemRarity.Equals(rarity))
                items.Add(item);
        }

        return items;
    }

    public List<BaseAbility> GetAbilitiesFrom(Item.Rarity rarity)
    {
        List<BaseAbility> abilities = new();

        foreach (BaseAbility ability in allAbilities)
        {
            if (ability.abilityRarity.Equals(rarity))
                abilities.Add(ability);
        }

        return abilities;
    }

    // For dev console

    public void GiveItem(string itemName, string amount)
    {
        foreach (Item item in allItems)
        {
            if (!item.itemType.ToString().Equals(itemName))
                continue;

            for (int i = 0; i < int.Parse(amount); i++)
                AddItem(item);
        }
    }

    public void GiveAllItems()
    {
        foreach (Item item in allItems)
            AddItem(item);
    }

    public void GiveAbility(string itemName)
    {
        foreach (BaseAbility ability in allAbilities)
        {
            if (!ability.abilityName.ToString().Equals(itemName))
                continue;

            AddAbility(ability);
        }
    }
}