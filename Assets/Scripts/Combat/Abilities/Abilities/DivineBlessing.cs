using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Divine Blessing")]
public class DivineBlessing : BaseAbility
{
    public override void InitAbility()
    {
    }

    public override void OnAbilityUse(BaseStats singleTarget, List<BaseStats> targetList)
    {
        // remove status effects
        singleTarget.Cleanse(StatusEffect.StatusType.Type.Debuff);
        AudioManager.Instance.PlayOneShot(Sound.SoundName.DivineBlessing);
    }

    public override void OnAbilityEnd(BaseStats singleTarget, List<BaseStats> targetList)
    {
    }
}