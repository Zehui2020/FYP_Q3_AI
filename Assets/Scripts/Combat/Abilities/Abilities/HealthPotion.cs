using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Health Potion")]
public class HealthPotion : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats self, BaseStats target)
    {
        target.Heal((int)(self.maxHealth * abilityStrength / 100));
    }

    public override void OnAbilityEnd(BaseStats self, BaseStats target)
    {
    }
}