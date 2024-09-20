using UnityEngine;

[CreateAssetMenu(menuName = "Items/AztecTotem")]
public class AztecTotem : Item
{
    [SerializeField] private int extraLife;

    public override void Initialize()
    {
        base.Initialize();
        PlayerController.Instance.extraLives += extraLife;
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        PlayerController.Instance.extraLives += extraLife;
    }
}