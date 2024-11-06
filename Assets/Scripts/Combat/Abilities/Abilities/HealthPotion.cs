using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Consumables/Health Potion")]
public class HealthPotion : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        singleTarget.Heal((int)(singleTarget.maxHealth * abilityStrength / 100));
        AudioManager.Instance.PlayOneShot(Sound.SoundName.HealthPotion);
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
    }
}