using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Divine Blessing")]
public class DivineBlessing : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        // remove status effects
        self.Cleanse(StatusEffect.StatusType.Type.Debuff);
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}