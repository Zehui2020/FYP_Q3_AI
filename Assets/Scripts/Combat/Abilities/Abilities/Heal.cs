using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Heal")]
public class Heal : BaseAbility
{
    public override void OnUseAbility(BaseStats self, BaseStats target)
    {
        target.Heal(25);
    }
}