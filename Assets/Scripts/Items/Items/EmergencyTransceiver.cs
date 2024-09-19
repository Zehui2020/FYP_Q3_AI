using UnityEngine;

[CreateAssetMenu(menuName = "Items/EmergencyTransceiver")]
public class EmergencyTransceiver : Item
{
    [SerializeField] private float buffDuration;
    [SerializeField] private float stackBuffDuration;

    [SerializeField] private float baseBuffMultiplier;

    public override void Initialize()
    {
        base.Initialize();
        itemStats.transceiverBuffDuration += buffDuration;
        itemStats.transceiverBuffMultiplier += baseBuffMultiplier;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        itemStats.transceiverBuffDuration += stackBuffDuration;
    }
}