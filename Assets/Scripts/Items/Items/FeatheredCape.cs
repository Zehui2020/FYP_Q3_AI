using UnityEngine;

[CreateAssetMenu(menuName = "Items/FeatheredCape")]
public class FeatheredCape : Item
{
    [SerializeField] private int jumpCount;

    public override void Initialize()
    {
        base.Initialize();
        PlayerController.Instance.AddJumpCount(jumpCount);
        PlayerController.Instance.AddWallJumpCount(jumpCount);
    }

    public override void IncrementStack()
    {
        base.IncrementStack();
        PlayerController.Instance.AddJumpCount(jumpCount);
        PlayerController.Instance.AddWallJumpCount(jumpCount);
    }
}