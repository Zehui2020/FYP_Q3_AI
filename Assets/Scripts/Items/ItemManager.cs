using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public List<Item> itemList;
    public List<Item> allItems;
    [SerializeField] private ItemPickupAlert itemPickupAlert;

    [SerializeField] private ItemStats itemStats;
    [SerializeField] private ItemUI itemUIPrefab;
    [SerializeField] private Transform itemUIParent;
    private List<ItemUI> itemUIs = new List<ItemUI>();

    private ImageSaver imageSaver;

    private void Awake()
    {
        Instance = this;
        itemStats.ResetStats();
    }

    public void InitItemManager()
    {
        itemList = new();
        imageSaver = GetComponent<ImageSaver>();

        foreach (Item item in allItems)
        {
            item.itemStack = 0;
            item.spriteIcon = imageSaver.GetSpriteFromLocalDisk(item.itemType.ToString());
        }
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
}