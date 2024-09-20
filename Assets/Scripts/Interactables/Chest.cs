using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DesignPatterns.ObjectPool;

public class Chest : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public struct ChestType
    {
        public enum Type
        {
            Normal,
            Large,
            Damage,
            Healing,
            Utility,
            Legendary
        }
        public Type type;
        public int commonItemRate;
        public int uncommonItemRate;
        public int legendaryItemRate;
    }
    public ChestType chestType;

    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private int cost;
    [SerializeField] private ItemStats itemStats;
    private bool isOpened = false;

    public void OnInteract()
    {
        if (isOpened || PlayerController.Instance.gold < cost)
            return;

        int randNum = Random.Range(0, 100);

        if (randNum < chestType.legendaryItemRate)
            SpawnItem(Item.Rarity.Legendary);
        else if (randNum < chestType.legendaryItemRate + chestType.uncommonItemRate)
            SpawnItem(Item.Rarity.Uncommon);
        else
            SpawnItem(Item.Rarity.Common);

        PlayerController.Instance.chestUnlockCount++;
        PlayerController.Instance.gold -= cost;
    }

    private void SpawnItem(Item.Rarity rarity)
    {
        int itemSpawnCount;
        if (PlayerController.Instance.chestUnlockCount <= 0 && itemStats.voucherRewardCount > 0)
            itemSpawnCount = 1 + itemStats.voucherRewardCount;
        else
            itemSpawnCount = 1;

        for (int i = 0; i < itemSpawnCount; i++)
        {
            List<Item> items = new();

            switch (chestType.type)
            {
                case ChestType.Type.Damage:
                    items = ItemManager.Instance.GetItemsFrom(Item.ItemCatagory.Damage, rarity);
                    break;
                case ChestType.Type.Healing:
                    items = ItemManager.Instance.GetItemsFrom(Item.ItemCatagory.Healing, rarity);
                    break;
                case ChestType.Type.Utility:
                    items = ItemManager.Instance.GetItemsFrom(Item.ItemCatagory.Utility, rarity);
                    break;
                case ChestType.Type.Normal:
                case ChestType.Type.Large:
                case ChestType.Type.Legendary:
                    items = ItemManager.Instance.GetItemsFrom(rarity);
                    break;
            }

            Debug.Log("Rarity: " + rarity);

            if (items.Count == 0)
            {
                Debug.Log("MISSING ITEM!");
                return;
            }

            int randNum = Random.Range(0, items.Count);
            ItemPickup item = ObjectPool.Instance.GetPooledObject("ItemPickup", true) as ItemPickup;
            item.transform.position = transform.position;
            item.InitPickup(items[randNum]);
        }

        isOpened = true;
    }

    public void OnEnterRange()
    {
        costText.text = cost.ToString();
        canvas.gameObject.SetActive(true);
    }

    public void OnLeaveRange()
    {
        canvas.gameObject.SetActive(false);
    }
}