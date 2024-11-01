using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DesignPatterns.ObjectPool;
using UnityEngine.UI;
using UnityEngine.Events;

public class Chest : MonoBehaviour, IInteractable
{
    [System.Serializable]
    public struct ChestType
    {
        public enum Type
        {
            Normal,
            Ability,
            Large,
            Damage,
            Healing,
            Utility,
            Legendary,
            FixedItem
        }
        public Type type;
        public int commonItemRate;
        public int uncommonItemRate;
        public int legendaryItemRate;
    }
    public ChestType chestType;
    public UnityEvent OnChestOpen;

    [SerializeField] private ScriptableObject fixedItemToSpawn;

    [SerializeField] private LayerMask defaultLayer;
    [SerializeField] private GameObject minimapIcon;

    [SerializeField] private SimpleAnimation keycodeUI;

    [SerializeField] private GameObject canvas;
    [SerializeField] private RectTransform costRect;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private int cost;
    [SerializeField] private ItemStats itemStats;
    private bool isOpened = false;

    [SerializeField] private Animator uiAnimator;

    public bool OnInteract()
    {
        if (isOpened)
            return false;

        if (PlayerController.Instance.gold < cost)
        {
            uiAnimator.SetTrigger("interactFailed");
            return false;
        }

        OnChestOpen?.Invoke();

        if (chestType.type == ChestType.Type.FixedItem)
        {
            if (fixedItemToSpawn is BaseAbility baseAbility)
            {
                PlayerController.Instance.abilityController.SpawnAbilityPickUp(baseAbility, transform);
            }
            else if (fixedItemToSpawn is Item itemToGive)
            {
                ItemPickup item = ObjectPool.Instance.GetPooledObject("ItemPickup", true) as ItemPickup;
                item.transform.position = transform.position;
                item.InitPickup(itemToGive);
            }

            isOpened = true;
            OnLeaveRange();
        }
        else
        {
            int randNum = Random.Range(0, 100);

            if (randNum < chestType.legendaryItemRate)
                SpawnItem(Item.Rarity.Legendary);
            else if (randNum < chestType.legendaryItemRate + chestType.uncommonItemRate)
                SpawnItem(Item.Rarity.Uncommon);
            else
                SpawnItem(Item.Rarity.Common);
        }

        PlayerController.Instance.chestUnlockCount++;
        PlayerController.Instance.RemoveGold(cost);
        LayoutRebuilder.ForceRebuildLayoutImmediate(costRect);
        keycodeUI.Hide();

        return true;
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
            List<BaseAbility> abilities = new();

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
                case ChestType.Type.Ability:
                    abilities = ItemManager.Instance.GetAbilitiesFrom(rarity);
                    break;
            }

            Debug.Log("Rarity: " + rarity);

            if (items.Count == 0 && abilities.Count == 0)
            {
                Debug.Log("MISSING ITEM!");
                return;
            }

            int randNum;
            if (chestType.type == ChestType.Type.Ability)
            {
                randNum = Random.Range(0, abilities.Count);
                PlayerController.Instance.abilityController.SpawnAbilityPickUp(abilities[randNum], transform);
                isOpened = true;
                OnLeaveRange();
                continue;
            }
            randNum = Random.Range(0, items.Count);
            ItemPickup item = ObjectPool.Instance.GetPooledObject("ItemPickup", true) as ItemPickup;
            item.transform.position = transform.position;
            item.InitPickup(items[randNum]);
        }

        isOpened = true;
        OnLeaveRange();
    }

    public void OnEnterRange()
    {
        if (isOpened)
            return;

        if (PlayerController.Instance.gold >= cost)
            keycodeUI.Show();

        canvas.gameObject.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(costRect);

        int layer = Mathf.RoundToInt(Mathf.Log(defaultLayer.value) / Mathf.Log(2));
        gameObject.layer = layer;
        minimapIcon.SetActive(true);
    }

    private void Update()
    {
        if (cost <= 0)
        {
            costText.text = "Free!";
            return;
        }

        if (PlayerController.Instance.gold < cost)
            costText.text = "<color=red>" + cost.ToString() + "</color>";
        else
            costText.text = cost.ToString();
    }

    public void OnLeaveRange()
    {
        keycodeUI.Hide();
        canvas.gameObject.SetActive(false);
    }

    public void SetCost(int newCost)
    {
        cost = newCost;
        costText.text = newCost.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(costRect);
    }
}