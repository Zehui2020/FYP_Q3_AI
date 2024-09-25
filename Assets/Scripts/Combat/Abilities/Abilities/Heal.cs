using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Heal")]
public class Heal : BaseAbility
{
    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        target.Heal(25);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}