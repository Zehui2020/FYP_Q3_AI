using UnityEngine;

public class Item : ScriptableObject
{
    public ItemStats itemStats;

    public enum Rarity { Common, Uncommon, Legendary, Special };
    public Rarity itemRarity;

    public enum ItemType { 
        HDHUD, RitualSickle, KnuckleDuster, JaggedDagger, CrudeKnife, FrazzledWire, IcyCrampon, GasolineTank, OverloadedCapacitor, AdrenalineShot,
        MetalBat, HiddenKunai, SpikedChestplate, EmergencyTransceiver, LeadPlunger, Dynamight, AncientGavel, BottleOfSurprises, FeatheredCape,
        BloodBag, TatteredVoucher, AztecTotem, InterestContract, RebateToken, NRGBar, DazeGrenade, PocketBarrier, ChargedDefibrillators,
        VampiricStake, BloodFungi, SentientAlgae
    };
    public ItemType itemType;

    public enum ItemCatagory { Damage, Healing, Utility };
    public ItemCatagory itemCatagory;

    public Sprite spriteIcon;
    [TextArea(3, 10)]
    public string title;
    [TextArea(3, 10)]
    public string description;  
    public int itemStack;
    public float alertDuration;
    public Material itemOutlineMaterial;

    public virtual void Initialize() { }

    public virtual void IncrementStack() { itemStack++; }

    public virtual void DecrementStack() { itemStack--; }

    public void SetCount(int newCount) { itemStack = newCount; }
}