using UnityEngine;

public class ShopItemData : ScriptableObject
{
    public enum ShopItemType
    {
        Item,
        Ability,
        Weapon
    }

    public ShopItemType shopItemType;
    public int shopCost;

    public Sprite spriteIcon;
    public Material itemOutlineMaterial;

    [TextArea(3, 10)]
    public string title;
    [TextArea(3, 10)]
    public string description;
    [TextArea(3, 10)]
    public string simpleDescription;
}