using UnityEngine;

[CreateAssetMenu(menuName = "Items/LeadPlunger")]
public class LeadPlunger : Item
{
    [SerializeField] private float minPlungeDist;
    [SerializeField] private float maxPlungeDist;
    [SerializeField] private float minPlungeMultiplier;
    [SerializeField] private float maxPlungeMultiplier;

    [SerializeField] private float stackPlungeDistReduction;
    [SerializeField] private float stackPlungeMaxMultiplier;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.minPlungeDist += minPlungeDist;
        itemStats.maxPlungeDist += maxPlungeDist;
        itemStats.minPlungeMultiplier += minPlungeMultiplier;
        itemStats.maxPlungeMultiplier += maxPlungeMultiplier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.maxPlungeDist -= stackPlungeDistReduction;
        itemStats.maxPlungeDist = Mathf.Clamp(itemStats.maxPlungeDist, itemStats.minPlungeDist, maxPlungeMultiplier);
        itemStats.maxPlungeMultiplier += stackPlungeMaxMultiplier;
    }
}