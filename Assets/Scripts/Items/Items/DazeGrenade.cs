using UnityEngine;

[CreateAssetMenu(menuName = "Items/DazeGrenade")]
public class DazeGrenade : Item
{
    [SerializeField] private int baseRadius;
    [SerializeField] private int stackRadius;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.dazeGrenadeRadius += baseRadius;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.dazeGrenadeRadius += stackRadius;
    }
}